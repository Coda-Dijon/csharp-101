using FoodTruck.Application;
using FoodTruck.Domaine;
using FoodTruck.Infrastructure.PostgreSql;
using Microsoft.AspNetCore.Http.HttpResults;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Scalar.AspNetCore;

// Spring Boot : @SpringBootApplication + SpringApplication.run(...)
var builder = WebApplication.CreateBuilder(args);

// Program.cs joue deux rôles dans l'hexagone :
//   - la « composition root » : c'est ici, et seulement ici, que les ports sont branchés sur leurs adaptateurs ;
//   - l'adaptateur HTTP : les routes de la Minimal API, plus bas, traduisent HTTP en appels au cœur.

// L'INJECTION DE DÉPENDANCES : intégrée au framework, comme le conteneur de Spring.

// SINGLETON : une instance pour toute l'application. La DataSource tient le pool de connexions,
// elle est thread-safe et coûteuse à créer : on la partage.
builder.Services.AddSingleton(services =>
    NpgsqlDataSource.Create(services.GetRequiredService<IConfiguration>().GetConnectionString("FoodTruck")
                            ?? throw new InvalidOperationException("Chaîne de connexion « FoodTruck » manquante")));

// SCOPED : une instance par requête HTTP, jetée à la fin de la requête.
// La durée de vie naturelle d'un repository (avec Entity Framework Core, celle du DbContext).
builder.Services.AddScoped<IRepositoryDesPlats, PlatsRepository>();
builder.Services.AddScoped<IRepositoryDesClients, ClientsRepository>();
builder.Services.AddScoped<IRepositoryDesCommandes, CommandesRepository>();

// TRANSIENT : une nouvelle instance à chaque injection. Le cas d'usage n'a aucun état : rien à partager.
builder.Services.AddTransient<PrendreUneCommande>();

// Les erreurs au format Problem Details (RFC 9457)
builder.Services.AddProblemDetails();

// OPENAPI : le document est généré par le framework (Spring : springdoc-openapi)
builder.Services.AddOpenApi();

// OPENTELEMETRY : traces, métriques et logs, envoyés seulement si un collecteur est configuré
if (builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"] is not null)
{
    builder.Logging.AddOpenTelemetry(logs => logs.IncludeFormattedMessage = true);
    builder.Services.AddOpenTelemetry()
        .ConfigureResource(resource => resource.AddService("food-truck"))
        .WithTracing(traces => traces.AddAspNetCoreInstrumentation().AddNpgsql())
        .WithMetrics(metrics => metrics.AddAspNetCoreInstrumentation())
        .UseOtlpExporter();
}

var app = builder.Build();

// LE PIPELINE DE MIDDLEWARES : chaque requête les traverse dans cet ordre
app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();               // /openapi/v1.json
    app.MapScalarApiReference();    // /scalar : l'interface pour explorer et appeler l'API
}

// LES ROUTES : une lambda par route (Spring : @RestController + @GetMapping / @PostMapping).
// Les paramètres de la lambda viennent de la route, de la query string, du body JSON ou de l'injection.
app.MapGet("/", () => "🧟 Bienvenue au Winchester, le food truck des morts-vivants. Le menu est sur /menu.")
    .ExcludeFromDescription();

app.MapGet("/menu", async (IRepositoryDesPlats plats, string? categorie) =>
        TypedResults.Ok(await plats.MenuAsync(categorie)))
    .WithSummary("Le menu du jour, filtrable par catégorie");

// 🛠 À toi de coder : 200 avec le plat, ou 404 s'il n'existe pas.
//    Le type de retour Results<Ok<Plat>, NotFound> documente les deux réponses possibles (dans l'OpenAPI aussi).
app.MapGet("/plats/{id:int}", async Task<Results<Ok<Plat>, NotFound>> (int id, IRepositoryDesPlats plats) =>
    {
        // >>> SOLUTION
        var plat = await plats.TrouverAsync(id);
        return plat is null ? TypedResults.NotFound() : TypedResults.Ok(plat);
        // <<< SOLUTION
    })
    .WithSummary("Un plat du menu");

// La route ne fait que traduire : le travail est dans le cas d'usage (Application/PrendreUneCommande.cs)
app.MapPost("/commandes", async Task<Results<Created<Commande>, NotFound, ProblemHttpResult>> (
        NouvelleCommande demande, PrendreUneCommande prendreUneCommande) =>
        await prendreUneCommande.ExecuterAsync(demande) switch
        {
            CommandeServie servie => TypedResults.Created($"/commandes/{servie.Commande.Id}", servie.Commande),
            CommandeRefusee refusee => Program.Probleme(refusee.Refus),
            _ => TypedResults.NotFound()
        })
    .WithSummary("Passer une commande");

app.MapGet("/commandes/{id:int}", async Task<Results<Ok<Commande>, NotFound>> (int id, IRepositoryDesCommandes commandes) =>
        await commandes.TrouverAsync(id) is { } commande ? TypedResults.Ok(commande) : TypedResults.NotFound())
    .WithSummary("Une commande déjà passée");

app.MapGet("/stats/chiffre-affaires", async (IRepositoryDesCommandes commandes) =>
        TypedResults.Ok(await commandes.ChiffreDAffairesParTypeAsync()))
    .WithSummary("Le chiffre d'affaires en dents, par type de zombie");

app.Run();

// Program est public pour les tests d'intégration (WebApplicationFactory<Program>),
// et accueille la traduction des refus en HTTP, que les tests appellent aussi directement.
public partial class Program
{
    // 🛠 À toi de coder : traduire un refus de la cuisine en réponse HTTP (RFC 9457, « Problem Details »).
    //    ClientEncoreVivant -> 422, titre « Client encore vivant »
    //    RegimeNonRespecte  -> 422, titre « Régime non respecté »
    //    RuptureDeStock     -> 409, titre « Rupture de stock »
    public static ProblemHttpResult Probleme(Decision refus)
    {
        // >>> SOLUTION
        return refus switch
        {
            ClientEncoreVivant v => TypedResults.Problem(
                title: "Client encore vivant", detail: $"{v.Nom} respire encore : pas de cerveau pour lui.", statusCode: 422),
            RegimeNonRespecte r => TypedResults.Problem(
                title: "Régime non respecté", detail: $"{r.Nom} est vegan : pas de {r.Categorie}.", statusCode: 422),
            RuptureDeStock r => TypedResults.Problem(
                title: "Rupture de stock", detail: $"Il ne reste que {r.Restant} {r.Plat}.", statusCode: 409),
            _ => throw new ArgumentOutOfRangeException(nameof(refus), refus, "Ce n'est pas un refus")
        };
        // <<< SOLUTION
    }
}
