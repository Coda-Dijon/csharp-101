using System.Text.Json;

namespace Asynchrone;

// 🎯 Objectif : tout assembler. Un camion de livraison vient d'arriver au Winchester :
//    on met à jour le stock dans PostgreSQL, puis on écrit l'inventaire dans un fichier JSON.
//    Des fichiers, une base, des await les uns après les autres : et pourtant le code se lit d'une traite.
public class LArrivage(BaseDeDonnees db) : AvecLaBase(db)
{
    private readonly DirectoryInfo _dossier = Directory.CreateTempSubdirectory("winchester-");

    public override ValueTask DisposeAsync()
    {
        _dossier.Delete(recursive: true);
        return ValueTask.CompletedTask;
    }

    private static readonly Livraison[] Camion =
    [
        new("Cervelle nature", 5),
        new("Tartare de neurones", 8),
        new("Cornetto", 3),            // ce n'est pas au menu !
        new("Chou-fleur vapeur", 10)
    ];

    // 🛠 À toi de coder : ajoute chaque livraison au stock du plat correspondant.
    //    Renvoie les noms des livraisons refusées, parce qu'aucun plat du menu ne porte ce nom.
    //    Indice : ExecuteAsync renvoie le nombre de lignes modifiées.
    private async Task<List<string>> ReceptionnerAsync(IEnumerable<Livraison> livraisons)
    {
        // >>> SOLUTION
        await using var connexion = await Source.OpenConnectionAsync();
        var refusees = new List<string>();
        foreach (var livraison in livraisons)
        {
            var modifiees = await connexion.ExecuteAsync(
                "UPDATE plats SET stock = stock + @Quantite WHERE nom = @Plat", livraison);
            if (modifiees == 0) refusees.Add(livraison.Plat);
        }
        return refusees;
        // <<< SOLUTION
    }

    // 🛠 À toi de coder : écris l'inventaire (nom du plat -> stock) dans un fichier JSON, trié par nom de plat.
    //    Le fichier attendu ressemble à : {"Cerveau flambé":3,"Cervelle nature":15,...}
    //    Indice : un Dictionary<string, int> devient un objet JSON ; SortedDictionary le trie par clé.
    private async Task EcrireInventaireAsync(string chemin)
    {
        // >>> SOLUTION
        await using var connexion = await Source.OpenConnectionAsync();
        var stocks = await connexion.QueryAsync<(string Nom, int Stock)>("SELECT nom, stock FROM plats");

        var inventaire = new SortedDictionary<string, int>(stocks.ToDictionary(s => s.Nom, s => s.Stock), StringComparer.Ordinal);
        await using var flux = File.Create(chemin);
        await JsonSerializer.SerializeAsync(flux, inventaire);
        // <<< SOLUTION
    }

    [Fact]
    public async Task On_receptionne_l_arrivage()
    {
        var refusees = await ReceptionnerAsync(Camion);

        await using var connexion = await Source.OpenConnectionAsync();
        var tartare = await connexion.ExecuteScalarAsync<int>("SELECT stock FROM plats WHERE nom = 'Tartare de neurones'");

        Check.That(refusees).ContainsExactly("Cornetto");
        Check.That(tartare).IsEqualTo(8);
    }

    [Fact]
    public async Task On_ecrit_l_inventaire_apres_l_arrivage()
    {
        var chemin = Path.Combine(_dossier.FullName, "inventaire.json");

        await ReceptionnerAsync(Camion);
        await EcrireInventaireAsync(chemin);

        await using var flux = File.OpenRead(chemin);
        var inventaire = await JsonSerializer.DeserializeAsync<Dictionary<string, int>>(flux);

        Check.That(inventaire!.Keys).ContainsExactly(
            "Cerveau flambé", "Cervelle nature", "Chou-fleur vapeur", "Gratin de chou-fleur", "Tartare de neurones");
        Check.That(inventaire["Cervelle nature"]).IsEqualTo(15);
        Check.That(inventaire["Chou-fleur vapeur"]).IsEqualTo(22);
    }
}
