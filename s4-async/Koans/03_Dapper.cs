namespace Asynchrone;

// 🎯 Objectif : écrire moins de plomberie avec Dapper, le micro-ORM le plus utilisé en .NET.
//
// Java : JdbcTemplate (Spring) ou jOOQ pour du SQL écrit à la main ; JPA / Hibernate pour un ORM complet
// C#   : Dapper pour du SQL écrit à la main ; Entity Framework Core pour un ORM complet
//
// Dapper ajoute des méthodes d'extension (séance 3) sur la connexion : QueryAsync<T>, ExecuteAsync...
// Il mappe chaque colonne vers le paramètre du constructeur (ou la propriété) qui porte le même nom.
public class AProposDeDapper(BaseDeDonnees db) : AvecLaBase(db)
{
    private const string SelectPlats =
        "SELECT id, nom, categorie, prix_en_dents AS PrixEnDents, stock FROM plats";

    [Fact]
    public async Task QueryAsync_mappe_les_lignes_vers_des_records()
    {
        await using var connexion = await Source.OpenConnectionAsync();

        // Java : jdbcTemplate.query(sql, (rs, i) -> new Plat(rs.getInt("id"), rs.getString("nom"), ...))
        var plats = (await connexion.QueryAsync<Plat>($"{SelectPlats} ORDER BY prix_en_dents DESC")).ToList();

        Check.That(plats.Count).IsEqualTo(___);
        Check.That(plats[0].Nom).IsEqualTo(___);
    }

    [Fact]
    public async Task Les_parametres_sont_un_objet_anonyme()
    {
        await using var connexion = await Source.OpenConnectionAsync();

        var tartare = await connexion.QuerySingleOrDefaultAsync<Plat>(
            $"{SelectPlats} WHERE nom = @nom", new { nom = "Tartare de neurones" });
        var burger = await connexion.QuerySingleOrDefaultAsync<Plat>(
            $"{SelectPlats} WHERE nom = @nom", new { nom = "Burger végétarien" });

        Check.That(tartare!.Stock).IsEqualTo(___);
        Check.That(burger is null).IsEqualTo(___); // pas de ligne : null, pas d'exception
    }

    [Fact]
    public async Task Concatener_du_SQL_ouvre_la_porte_aux_injections()
    {
        await using var connexion = await Source.OpenConnectionAsync();
        var saisie = "x' OR '1'='1"; // ce qu'un petit malin tape dans le champ « nom du plat »

        var concatene = await connexion.QueryAsync<string>($"SELECT nom FROM plats WHERE nom = '{saisie}'");
        var parametre = await connexion.QueryAsync<string>("SELECT nom FROM plats WHERE nom = @saisie", new { saisie });

        Check.That(concatene.Count()).IsEqualTo(___); // WHERE nom = 'x' OR '1'='1' : toute la table !
        Check.That(parametre.Count()).IsEqualTo(___);  // aucun plat ne s'appelle vraiment comme ça
    }

    [Fact]
    public async Task ExecuteAsync_renvoie_le_nombre_de_lignes_touchees()
    {
        await using var connexion = await Source.OpenConnectionAsync();

        var reapprovisionnes = await connexion.ExecuteAsync(
            "UPDATE plats SET stock = stock + 1 WHERE categorie = @categorie", new { categorie = "Cerveau" });
        var nouvelId = await connexion.ExecuteScalarAsync<int>(
            "INSERT INTO clients (nom, type) VALUES (@nom, @type) RETURNING id", new { nom = "Liz", type = "Coureur" });

        Check.That(reapprovisionnes).IsEqualTo(___);
        Check.That(nouvelId).IsEqualTo(___);
    }

    [Fact]
    public async Task Une_transaction_annulee_ne_laisse_aucune_trace()
    {
        await using var connexion = await Source.OpenConnectionAsync();

        // Java : connection.setAutoCommit(false) ... connection.rollback(), ou @Transactional
        await using (var transaction = await connexion.BeginTransactionAsync())
        {
            await connexion.ExecuteAsync("UPDATE plats SET stock = 0", transaction: transaction);
            await transaction.RollbackAsync();
        }

        var stock = await connexion.ExecuteScalarAsync<int>("SELECT stock FROM plats WHERE nom = 'Cervelle nature'");
        Check.That(stock).IsEqualTo(___);
    }

    [Fact]
    public async Task Les_tuples_recoivent_les_colonnes_dans_l_ordre()
    {
        await using var connexion = await Source.OpenConnectionAsync();

        var chiffreDAffaires = (await connexion.QueryAsync<(string Type, int Dents)>(
            """
            SELECT c.type, sum(p.prix_en_dents * co.quantite)::int
            FROM commandes co JOIN clients c ON c.id = co.client_id JOIN plats p ON p.id = co.plat_id
            GROUP BY c.type ORDER BY 2 DESC
            """)).ToList();

        Check.That(chiffreDAffaires[0].Type).IsEqualTo(___);
        Check.That(chiffreDAffaires.Sum(l => l.Dents)).IsEqualTo(___);
    }

    // 🛠 À toi de coder : le nom du plat le plus commandé (en quantité totale), toutes commandes confondues.
    private async Task<string> PlatLePlusPopulaireAsync()
    {
        throw new NotImplementedException("🧟 À toi de jouer !");
    }

    // 🛠 À toi de coder : les noms des clients qui n'ont jamais commandé, par ordre alphabétique.
    private async Task<List<string>> ClientsSansCommandeAsync()
    {
        throw new NotImplementedException("🧟 À toi de jouer !");
    }

    [Fact]
    public async Task On_sait_ecrire_ses_requetes_avec_Dapper()
    {
        Check.That(await PlatLePlusPopulaireAsync()).IsEqualTo("Cervelle nature");
        Check.That(await ClientsSansCommandeAsync()).ContainsExactly("Ed", "Shaun");
    }
}
