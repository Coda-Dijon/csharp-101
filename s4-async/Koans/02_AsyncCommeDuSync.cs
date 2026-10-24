namespace Asynchrone;

// 🎯 Objectif : interroger PostgreSQL en async, et constater qu'avec async / await,
//    du code asynchrone se lit (et s'écrit) comme du code synchrone.
//
// Java : JDBC (Connection, PreparedStatement, ResultSet) est bloquant ; pour de l'asynchrone il faut
//        CompletableFuture.supplyAsync(...).thenCompose(...), R2DBC, ou des virtual threads (Java 21).
// C#   : ADO.NET (ici Npgsql, le driver PostgreSQL) propose une version Async de chaque opération.
//        Pendant qu'on attend PostgreSQL, le thread est rendu : il sert d'autres requêtes.
public class AProposDeLAsyncCommeDuSync(BaseDeDonnees db) : AvecLaBase(db)
{
    [Fact]
    public async Task On_ouvre_une_connexion_et_on_pose_une_question()
    {
        // Java : try (Connection c = dataSource.getConnection(); PreparedStatement ps = c.prepareStatement(...))
        await using var connexion = await Source.OpenConnectionAsync();
        await using var commande = new NpgsqlCommand("SELECT count(*) FROM plats", connexion);

        var nombre = (long)(await commande.ExecuteScalarAsync())!; // count(*) est un bigint : un long en C#

        Check.That(nombre).IsEqualTo(___);
    }

    [Fact]
    public async Task On_lit_les_resultats_ligne_par_ligne()
    {
        await using var commande = Source.CreateCommand(
            "SELECT nom FROM plats WHERE categorie = 'Chou-fleur' ORDER BY id");
        await using var lecteur = await commande.ExecuteReaderAsync();

        var noms = new List<string>();
        while (await lecteur.ReadAsync())          // Java : while (resultSet.next())
            noms.Add(lecteur.GetString(0));        // Java : resultSet.getString(1), les colonnes commencent à 1 !

        Check.That(string.Join(", ", noms)).IsEqualTo(___);
    }

    // Trois petites requêtes, que l'on va enchaîner
    private async Task<int> IdDuClientAsync(string nom)
    {
        await using var commande = Source.CreateCommand("SELECT id FROM clients WHERE nom = $1");
        commande.Parameters.Add(new NpgsqlParameter { Value = nom });
        return (int)(await commande.ExecuteScalarAsync())!;
    }

    private async Task<string> PlatFavoriAsync(int clientId)
    {
        await using var commande = Source.CreateCommand(
            """
            SELECT p.nom FROM commandes c JOIN plats p ON p.id = c.plat_id
            WHERE c.client_id = $1
            GROUP BY p.nom ORDER BY sum(c.quantite) DESC LIMIT 1
            """);
        commande.Parameters.Add(new NpgsqlParameter { Value = clientId });
        return (string)(await commande.ExecuteScalarAsync())!;
    }

    private async Task<int> StockAsync(string plat)
    {
        await using var commande = Source.CreateCommand("SELECT stock FROM plats WHERE nom = $1");
        commande.Parameters.Add(new NpgsqlParameter { Value = plat });
        return (int)(await commande.ExecuteScalarAsync())!;
    }

    [Fact]
    public async Task Plusieurs_requetes_d_affilee_s_ecrivent_comme_du_code_synchrone()
    {
        // Chaque étape a besoin du résultat de la précédente.
        // Aucun callback, aucun thenCompose : on lit de haut en bas, comme du code bloquant... sans rien bloquer.
        var idDeBob = await IdDuClientAsync("Bob");
        var favori = await PlatFavoriAsync(idDeBob);
        var stock = await StockAsync(favori);

        Check.That(favori).IsEqualTo(___);
        Check.That(stock).IsEqualTo(___);
    }

    [Fact]
    public async Task Les_boucles_et_les_conditions_marchent_aussi()
    {
        // Un await dans un foreach, dans un if : impensable avec des callbacks, banal avec await
        var stocks = new Dictionary<string, int>();
        foreach (var nom in new[] { "Bob", "Alice", "Ed" })
        {
            var id = await IdDuClientAsync(nom);
            if (nom == "Ed") continue; // Ed n'a jamais rien commandé

            var favori = await PlatFavoriAsync(id);
            stocks[nom] = await StockAsync(favori);
        }

        Check.That(stocks.Count).IsEqualTo(___);
        Check.That(stocks["Alice"]).IsEqualTo(___); // Alice préfère aussi la Cervelle nature
    }

    [Fact]
    public async Task Try_catch_attrape_les_erreurs_asynchrones()
    {
        var passeParLeCatch = false;
        string? codeSql = null;

        try
        {
            await using var commande = Source.CreateCommand("SELECT * FROM cerveaux_frais");
            await commande.ExecuteNonQueryAsync();
        }
        catch (PostgresException erreur) // l'exception remonte au await, comme une exception synchrone
        {
            passeParLeCatch = true;
            codeSql = erreur.SqlState;
        }

        Check.That(passeParLeCatch).IsEqualTo(___);
        Check.That(codeSql).IsEqualTo("42P01"); // undefined_table : la table n'existe pas
    }

    [Fact]
    public async Task Sans_await_on_ne_tient_qu_une_promesse()
    {
        // Oublier le await : le code compile, mais on manipule la Task (≈ CompletableFuture), pas le résultat
        object resultat = IdDuClientAsync("Bob");

        Check.That(resultat is Task<int>).IsEqualTo(___);
        Check.That(resultat is int).IsEqualTo(___);

        Check.That(await (Task<int>)resultat).IsEqualTo(___); // await « déballe » la Task
    }

    [Fact]
    public async Task Des_requetes_independantes_peuvent_partir_en_parallele()
    {
        // On lance les deux requêtes SANS les attendre, puis on attend les deux.
        // La DataSource prête une connexion différente à chacune (Java : CompletableFuture.allOf)
        async Task<long> Compter(string table)
        {
            await using var commande = Source.CreateCommand($"SELECT count(*) FROM {table}");
            return (long)(await commande.ExecuteScalarAsync())!;
        }

        var tachePlats = Compter("plats");       // la requête part...
        var tacheClients = Compter("clients");   // ...et celle-ci aussi, sans attendre la première
        long[] resultats = await Task.WhenAll(tachePlats, tacheClients);

        Check.That(resultats[0]).IsEqualTo(___);
        Check.That(resultats[1]).IsEqualTo(___);
    }

    [Fact]
    public async Task Une_connexion_ne_traite_qu_une_requete_a_la_fois()
    {
        await using var connexion = await Source.OpenConnectionAsync();

        async Task<object?> Compter(string table)
        {
            await using var commande = new NpgsqlCommand($"SELECT count(*) FROM {table}", connexion);
            return await commande.ExecuteScalarAsync();
        }

        // Deux requêtes en même temps sur LA MÊME connexion : refusé (en JDBC non plus, une Connection n'est pas thread-safe)
        var erreur = await Record.ExceptionAsync(() => Task.WhenAll(Compter("plats"), Compter("clients")));

        Check.That(erreur is NpgsqlOperationInProgressException).IsEqualTo(___);
    }

    // 🛠 À toi de coder : la fiche d'un client, en enchaînant les requêtes dont tu as besoin.
    //    - client inconnu                       -> null
    //    - NombreDeCommandes, TotalEnDents      -> 0 s'il n'a rien commandé
    //    - PlatFavori                           -> le plat le plus commandé (en quantité), null s'il n'a rien commandé
    //    Écris-la comme du code synchrone : un await par requête, des if, des variables.
    private async Task<FicheClient?> FicheAsync(string nom)
    {
        throw new NotImplementedException("🧟 À toi de jouer !");
    }

    [Fact]
    public async Task On_sait_enchainer_des_requetes_asynchrones()
    {
        Check.That(await FicheAsync("Bob")).IsEqualTo(new FicheClient("Bob", 3, "Cervelle nature", 56));
        Check.That(await FicheAsync("Alice")).IsEqualTo(new FicheClient("Alice", 2, "Cervelle nature", 76));
        Check.That(await FicheAsync("Ed")).IsEqualTo(new FicheClient("Ed", 0, null, 0));
        Check.That(await FicheAsync("Fulci")).IsNull();
    }
}

public sealed record FicheClient(string Nom, int NombreDeCommandes, string? PlatFavori, int TotalEnDents);
