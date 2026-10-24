using Dapper;
using FoodTruck.Domaine;
using Npgsql;

namespace FoodTruck.Infrastructure.PostgreSql;

// Les ADAPTATEURS PostgreSQL : ils implémentent les ports du domaine avec Dapper (≈ JdbcTemplate).
// Toutes les méthodes sont async : pendant que PostgreSQL répond, aucun thread n'attend.
// Remplacer Dapper par Entity Framework Core ne toucherait qu'à ce dossier.

public sealed class PlatsRepository(NpgsqlDataSource source) : IRepositoryDesPlats
{
    private const string SelectPlats =
        "SELECT id, nom, categorie, prix_en_dents AS PrixEnDents, stock FROM plats";

    public async Task<IReadOnlyList<Plat>> MenuAsync(string? categorie = null)
    {
        await using var connexion = await source.OpenConnectionAsync();
        var plats = await connexion.QueryAsync<Plat>(
            $"{SelectPlats} WHERE @categorie::text IS NULL OR categorie = @categorie ORDER BY id",
            new { categorie });
        return plats.AsList();
    }

    public async Task<Plat?> TrouverAsync(int id)
    {
        await using var connexion = await source.OpenConnectionAsync();
        return await connexion.QuerySingleOrDefaultAsync<Plat>($"{SelectPlats} WHERE id = @id", new { id });
    }
}

public sealed class ClientsRepository(NpgsqlDataSource source) : IRepositoryDesClients
{
    public async Task<Client?> TrouverAsync(int id)
    {
        await using var connexion = await source.OpenConnectionAsync();
        return await connexion.QuerySingleOrDefaultAsync<Client>(
            "SELECT id, nom, type, est_vivant AS EstVivant FROM clients WHERE id = @id", new { id });
    }
}

public sealed class CommandesRepository(NpgsqlDataSource source) : IRepositoryDesCommandes
{
    private const string ColonnesCommande =
        "id, client_id AS ClientId, plat_id AS PlatId, quantite, passee_le AS PasseeLe";

    public async Task<Commande?> TrouverAsync(int id)
    {
        await using var connexion = await source.OpenConnectionAsync();
        return await connexion.QuerySingleOrDefaultAsync<Commande>(
            $"SELECT {ColonnesCommande} FROM commandes WHERE id = @id", new { id });
    }

    public async Task<Commande?> EnregistrerAsync(NouvelleCommande demande)
    {
        await using var connexion = await source.OpenConnectionAsync();
        await using var transaction = await connexion.BeginTransactionAsync();

        // le « AND stock >= @Quantite » protège des commandes concurrentes : jamais de stock négatif
        var servis = await connexion.ExecuteAsync(
            "UPDATE plats SET stock = stock - @Quantite WHERE id = @PlatId AND stock >= @Quantite",
            demande, transaction);
        if (servis == 0) return null;

        var commande = await connexion.QuerySingleAsync<Commande>(
            $"""
            INSERT INTO commandes (client_id, plat_id, quantite) VALUES (@ClientId, @PlatId, @Quantite)
            RETURNING {ColonnesCommande}
            """,
            demande, transaction);

        await transaction.CommitAsync();
        return commande;
    }

    public async Task<IReadOnlyDictionary<string, int>> ChiffreDAffairesParTypeAsync()
    {
        await using var connexion = await source.OpenConnectionAsync();
        var lignes = await connexion.QueryAsync<(string Type, int Total)>(
            """
            SELECT c.type, sum(p.prix_en_dents * co.quantite)::int
            FROM commandes co
            JOIN clients c ON c.id = co.client_id
            JOIN plats p ON p.id = co.plat_id
            GROUP BY c.type
            """);
        return lignes.ToDictionary(l => l.Type, l => l.Total);
    }
}
