using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;

namespace Ecosysteme;

// Testcontainers démarre un vrai PostgreSQL dans Docker pour les tests, puis le jette à la fin.
// C'est la même bibliothèque qu'en Java (org.testcontainers:postgresql), avec la même API.
//
// Une seule base pour toute la collection « PostgreSQL » : le conteneur démarre une fois (quelques secondes),
// et les tests de la collection s'exécutent l'un après l'autre.
public sealed class BaseDeDonnees : IAsyncLifetime
{
    private readonly PostgreSqlContainer _conteneur = new PostgreSqlBuilder("postgres:17-alpine").Build();

    public NpgsqlDataSource Source { get; private set; } = null!;
    public string ChaineDeConnexion => _conteneur.GetConnectionString();

    public async ValueTask InitializeAsync()
    {
        try
        {
            await _conteneur.StartAsync();
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(
                "🐳 Impossible de démarrer PostgreSQL : Docker est-il lancé ? (docker info)", e);
        }

        Source = NpgsqlDataSource.Create(ChaineDeConnexion);
        await ReinitialiserAsync();
    }

    // Rejoue le script du FoodTruck : les tables repartent de zéro, avec les données d'origine
    public async Task ReinitialiserAsync()
    {
        var script = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "sql", "init.sql"));
        await using var commande = Source.CreateCommand(script);
        await commande.ExecuteNonQueryAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (Source is not null) await Source.DisposeAsync();
        await _conteneur.DisposeAsync();
    }
}

[CollectionDefinition("PostgreSQL")]
public sealed class CollectionPostgreSql : ICollectionFixture<BaseDeDonnees>;

// Chaque classe de tests qui hérite d'AvecLaBase repart d'une base propre avant chaque test
[Collection("PostgreSQL")]
public abstract class AvecLaBase(BaseDeDonnees db) : IAsyncLifetime
{
    protected NpgsqlDataSource Source => db.Source;
    protected string ChaineDeConnexion => db.ChaineDeConnexion;

    public async ValueTask InitializeAsync() => await db.ReinitialiserAsync();

    public virtual ValueTask DisposeAsync() => ValueTask.CompletedTask;
}

// Les mêmes tests, avec en plus l'API démarrée en mémoire (créée au premier appel, jetée après le test)
public abstract class AvecLApi(BaseDeDonnees db) : AvecLaBase(db)
{
    private FoodTruckEnMemoire? _api;

    protected HttpClient Api => (_api ??= new FoodTruckEnMemoire(ChaineDeConnexion)).CreateClient();

    public override async ValueTask DisposeAsync()
    {
        if (_api is not null) await _api.DisposeAsync();
    }
}

// L'API du FoodTruck démarrée en mémoire, branchée sur le PostgreSQL du conteneur.
// Spring : @SpringBootTest + MockMvc (ou TestRestTemplate)
public sealed class FoodTruckEnMemoire(string chaineDeConnexion) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.UseSetting("ConnectionStrings:FoodTruck", chaineDeConnexion);
    }
}
