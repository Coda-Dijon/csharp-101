using FoodTruck.Infrastructure.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Ecosysteme;

// 🎯 Objectif : le conteneur d'injection de dépendances intégré à .NET, celui qu'utilise ASP.NET Core.
//
// Spring : @Component / @Service + @Autowired (ou injection par constructeur), scopes singleton / prototype / request
// .NET   : on enregistre les services dans un IServiceCollection, avec une durée de vie, puis on les reçoit
//          par le constructeur. Pas d'annotation, pas de scan du classpath : tout est déclaré dans Program.cs.
//
//   Spring                      .NET
//   singleton (par défaut)      AddSingleton    une instance pour toute l'application
//   request                     AddScoped       une instance par « scope » : par requête HTTP dans ASP.NET Core
//   prototype                   AddTransient    une nouvelle instance à chaque injection
//
// Comment choisir ?
//   - sans état, ou un état partagé et thread-safe, coûteux à créer (pool, cache, client HTTP)  -> singleton
//   - lié à une requête : un repository, un DbContext, l'utilisateur courant, une transaction   -> scoped
//   - léger et sans état, quand on ne veut rien partager                                         -> transient
//   Règle d'or : un service ne doit dépendre que de services qui vivent AU MOINS aussi longtemps que lui.
public class AProposDeLInjectionDeDependances
{
    private sealed class Compteur
    {
        public Guid Id { get; } = Guid.NewGuid();
    }

    [Fact]
    public void Un_singleton_est_partage_par_tout_le_monde()
    {
        var services = new ServiceCollection().AddSingleton<Compteur>().BuildServiceProvider();

        using var requete1 = services.CreateScope();
        using var requete2 = services.CreateScope();
        var a = requete1.ServiceProvider.GetRequiredService<Compteur>();
        var b = requete2.ServiceProvider.GetRequiredService<Compteur>();

        Check.That(a.Id == b.Id).IsEqualTo(___);
    }

    [Fact]
    public void Un_service_scoped_vit_le_temps_d_une_requete()
    {
        var services = new ServiceCollection().AddScoped<Compteur>().BuildServiceProvider();

        using var requete1 = services.CreateScope();
        using var requete2 = services.CreateScope();
        var a1 = requete1.ServiceProvider.GetRequiredService<Compteur>();
        var a2 = requete1.ServiceProvider.GetRequiredService<Compteur>();
        var b = requete2.ServiceProvider.GetRequiredService<Compteur>();

        Check.That(a1.Id == a2.Id).IsEqualTo(___);  // même requête
        Check.That(a1.Id == b.Id).IsEqualTo(___);  // autre requête
    }

    [Fact]
    public void Un_service_transient_est_neuf_a_chaque_fois()
    {
        var services = new ServiceCollection().AddTransient<Compteur>().BuildServiceProvider();

        var a = services.GetRequiredService<Compteur>();
        var b = services.GetRequiredService<Compteur>();

        Check.That(a.Id == b.Id).IsEqualTo(___);
    }

    private sealed class Caisse(Compteur compteur)
    {
        public Compteur Compteur { get; } = compteur;
    }

    [Fact]
    public void Un_singleton_ne_doit_pas_capturer_un_service_scoped()
    {
        // Le piège de la « captive dependency » : un singleton qui garde pour toujours un service
        // censé vivre le temps d'une requête (une connexion, un panier...). ValidateScopes le détecte.
        var services = new ServiceCollection()
            .AddScoped<Compteur>()
            .AddSingleton<Caisse>()
            .BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true }); // actif par défaut en Development

        var erreur = Record.Exception(() => services.GetRequiredService<Caisse>());

        Check.That(erreur is InvalidOperationException).IsEqualTo(___);
    }

    [Fact]
    public void Les_durees_de_vie_du_FoodTruck()
    {
        // Les vrais enregistrements de FoodTruck/Program.cs (l'API n'a pas besoin de la base pour démarrer)
        using var api = new FoodTruckEnMemoire("Host=localhost;Database=foodtruck");
        using var requete1 = api.Services.CreateScope();
        using var requete2 = api.Services.CreateScope();
        T Dans<T>(IServiceScope requete) where T : notnull => requete.ServiceProvider.GetRequiredService<T>();

        // Le port est branché sur l'adaptateur PostgreSQL
        Check.That(Dans<IRepositoryDesPlats>(requete1) is PlatsRepository).IsEqualTo(___);

        // NpgsqlDataSource : singleton
        Check.That(ReferenceEquals(Dans<NpgsqlDataSource>(requete1), Dans<NpgsqlDataSource>(requete2)))
            .IsEqualTo(___);

        // Les repositories : scoped
        Check.That(ReferenceEquals(Dans<IRepositoryDesPlats>(requete1), Dans<IRepositoryDesPlats>(requete1)))
            .IsEqualTo(___);
        Check.That(ReferenceEquals(Dans<IRepositoryDesPlats>(requete1), Dans<IRepositoryDesPlats>(requete2)))
            .IsEqualTo(___);

        // Le cas d'usage : transient
        Check.That(ReferenceEquals(Dans<PrendreUneCommande>(requete1), Dans<PrendreUneCommande>(requete1)))
            .IsEqualTo(___);
    }

    public sealed class OptionsDuFoodTruck
    {
        public string Nom { get; set; } = "";
        public int LimiteParCommande { get; set; }
    }

    [Fact]
    public void La_configuration_se_lit_dans_une_classe_typee()
    {
        // Spring : @ConfigurationProperties(prefix = "food-truck") ; .NET : IOptions<T>
        // La configuration vient d'appsettings.json, des variables d'environnement, de la ligne de commande...
        // ici, d'un dictionnaire en mémoire. Les sections sont séparées par « : » (FoodTruck__Nom en variable d'environnement)
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FoodTruck:Nom"] = "Le Winchester",
                ["FoodTruck:LimiteParCommande"] = "5"
            })
            .Build();

        var services = new ServiceCollection()
            .Configure<OptionsDuFoodTruck>(configuration.GetSection("FoodTruck"))
            .BuildServiceProvider();
        var options = services.GetRequiredService<IOptions<OptionsDuFoodTruck>>().Value;

        Check.That(options.Nom).IsEqualTo(___);
        Check.That(options.LimiteParCommande).IsEqualTo(___); // converti depuis la chaîne "5"
    }

    public interface IHorloge
    {
        DateTime Maintenant { get; }
    }

    public sealed class HorlogeDuBunker : IHorloge
    {
        public DateTime Maintenant => DateTime.UtcNow;
    }

    public sealed class Cuisinier(IHorloge horloge)
    {
        public string Annonce() => horloge.Maintenant.Hour < 12 ? "Petit-déjeuner servi" : "Dîner servi";
    }

    // 🛠 À toi de coder : enregistre les services du food truck.
    //    - IHorloge, implémentée par HorlogeDuBunker : une seule pour toute l'application
    //    - Cuisinier : un par requête
    private static IServiceCollection Enregistrer(IServiceCollection services)
    {
        throw new NotImplementedException("🧟 À toi de jouer !");
    }

    [Fact]
    public void On_sait_enregistrer_ses_services()
    {
        var services = Enregistrer(new ServiceCollection());
        var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });

        using var requete = provider.CreateScope();
        Check.That(requete.ServiceProvider.GetRequiredService<Cuisinier>().Annonce()).EndsWith("servi");
        Check.That(services.Single(s => s.ServiceType == typeof(IHorloge)).Lifetime).IsEqualTo(ServiceLifetime.Singleton);
        Check.That(services.Single(s => s.ServiceType == typeof(Cuisinier)).Lifetime).IsEqualTo(ServiceLifetime.Scoped);
    }
}
