using FoodTruck;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Scalar.AspNetCore;

// Spring Boot : @SpringBootApplication + SpringApplication.run(...)
var builder = WebApplication.CreateBuilder(args);

// L'INJECTION DE DÉPENDANCES : intégrée au framework, comme le conteneur de Spring
// Une seule source de connexions (avec son pool) pour toute l'application...
builder.Services.AddSingleton(services =>
    NpgsqlDataSource.Create(services.GetRequiredService<IConfiguration>().GetConnectionString("FoodTruck")
                            ?? throw new InvalidOperationException("Chaîne de connexion « FoodTruck » manquante")));
// ...et un Comptoir par requête HTTP
builder.Services.AddScoped<Comptoir>();

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

app.MapGet("/", () => "🧟 Bienvenue au Winchester, le food truck des morts-vivants. Le menu est sur /menu.")
    .ExcludeFromDescription();
app.MapFoodTruck();

app.Run();

// Rend Program visible pour les tests d'intégration (WebApplicationFactory<Program>)
public partial class Program;
