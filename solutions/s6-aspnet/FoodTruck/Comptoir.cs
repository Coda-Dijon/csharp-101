using Dapper;
using Npgsql;

namespace FoodTruck;

// L'accès aux données, avec Dapper (≈ JdbcTemplate) : du SQL écrit à la main, mappé vers les records.
// Toutes les méthodes sont async : pendant que PostgreSQL répond, aucun thread n'attend.
public sealed class Comptoir(NpgsqlDataSource source)
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

    public async Task<Plat?> PlatAsync(int id)
    {
        await using var connexion = await source.OpenConnectionAsync();
        return await connexion.QuerySingleOrDefaultAsync<Plat>($"{SelectPlats} WHERE id = @id", new { id });
    }

    public async Task<Client?> ClientAsync(int id)
    {
        await using var connexion = await source.OpenConnectionAsync();
        return await connexion.QuerySingleOrDefaultAsync<Client>(
            "SELECT id, nom, type, est_vivant AS EstVivant FROM clients WHERE id = @id", new { id });
    }

    public async Task<Commande?> CommandeAsync(int id)
    {
        await using var connexion = await source.OpenConnectionAsync();
        return await connexion.QuerySingleOrDefaultAsync<Commande>(
            "SELECT id, client_id AS ClientId, plat_id AS PlatId, quantite, passee_le AS PasseeLe FROM commandes WHERE id = @id",
            new { id });
    }

    // Décrémente le stock et enregistre la commande dans une même transaction.
    // Renvoie null si le stock a été vidé entre-temps par une autre commande.
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
            """
            INSERT INTO commandes (client_id, plat_id, quantite) VALUES (@ClientId, @PlatId, @Quantite)
            RETURNING id, client_id AS ClientId, plat_id AS PlatId, quantite, passee_le AS PasseeLe
            """,
            demande, transaction);

        await transaction.CommitAsync();
        return commande;
    }

    public async Task<Dictionary<string, int>> ChiffreDAffairesParTypeAsync()
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
