using System.Text.Json;

namespace Asynchrone;

// 🎯 Objectif : lire et écrire des fichiers (texte, CSV, JSON), en async.
//
// Java : java.nio.file.Files + Path, try-with-resources, Jackson pour le JSON
// C#   : System.IO.File + Path, using / await using, System.Text.Json (intégré au framework)

public sealed record Livraison(string Plat, int Quantite);

public class AProposDesFichiers : IDisposable
{
    private static readonly string Arrivage = Path.Combine(AppContext.BaseDirectory, "donnees", "arrivage.csv");

    // Un dossier temporaire par test, supprimé à la fin (Java : @TempDir)
    private readonly DirectoryInfo _dossier = Directory.CreateTempSubdirectory("winchester-");

    public void Dispose() => _dossier.Delete(recursive: true);

    [Fact]
    public void Path_decoupe_les_chemins()
    {
        // Path.Combine choisit le bon séparateur, / ou \ selon l'OS (Java : Path.of(...).resolve(...))
        var chemin = Path.Combine("donnees", "arrivage.csv");

        Check.That(Path.GetFileName(chemin)).IsEqualTo(___);
        Check.That(Path.GetExtension(chemin)).IsEqualTo(___);
        Check.That(Path.GetFileNameWithoutExtension(chemin)).IsEqualTo(___);
    }

    [Fact]
    public async Task On_ecrit_et_on_relit_un_fichier_en_une_ligne()
    {
        // Un test peut être async : xUnit attend la Task (JUnit : impossible, il faut bloquer avec .get())
        var chemin = Path.Combine(_dossier.FullName, "cri.txt");

        await File.WriteAllTextAsync(chemin, "Braaains");   // Java : Files.writeString(path, "Braaains")
        var contenu = await File.ReadAllTextAsync(chemin);  // Java : Files.readString(path)

        Check.That(File.Exists(chemin)).IsEqualTo(___);
        Check.That(contenu).IsEqualTo(___);
    }

    [Fact]
    public async Task On_lit_un_gros_fichier_ligne_par_ligne()
    {
        // ReadLinesAsync ne charge pas tout le fichier : il renvoie un IAsyncEnumerable<string>,
        // qu'on parcourt avec await foreach (Java : Files.lines(path), un Stream qu'il faut fermer)
        var nombreDeLignes = 0;
        await foreach (var ligne in File.ReadLinesAsync(Arrivage))
            nombreDeLignes++;

        Check.That(nombreDeLignes).IsEqualTo(___); // l'en-tête + 4 livraisons
    }

    [Fact]
    public async Task Await_using_ferme_le_fichier_comme_try_with_resources()
    {
        var chemin = Path.Combine(_dossier.FullName, "stock.csv");
        long tailleAvantLaFermeture;

        // Java : try (var writer = Files.newBufferedWriter(path)) { ... }
        await using (var ecrivain = new StreamWriter(chemin))
        {
            await ecrivain.WriteLineAsync("Cervelle nature;5");
            tailleAvantLaFermeture = new FileInfo(chemin).Length;
        } // ici, DisposeAsync vide le tampon et ferme le fichier

        Check.That(tailleAvantLaFermeture).IsEqualTo(___); // tout est encore dans le tampon
        Check.That(new FileInfo(chemin).Length > 0).IsEqualTo(___);
    }

    [Fact]
    public void System_Text_Json_garde_les_noms_CSharp_par_defaut()
    {
        var zombie = new { Nom = "Bob", Faim = 7 };

        // Par défaut : les noms des propriétés tels quels, en PascalCase (Jackson : les noms des getters, en camelCase)
        Check.That(JsonSerializer.Serialize(zombie)).IsEqualTo(___);

        // JsonSerializerOptions.Web : les conventions du web, celles qu'utilise ASP.NET Core
        Check.That(JsonSerializer.Serialize(zombie, JsonSerializerOptions.Web))
            .IsEqualTo(___);
    }

    [Fact]
    public void Les_pieges_de_System_Text_Json()
    {
        // 1. Les caractères non ASCII sont échappés par défaut (prudence contre le XSS)
        var json = JsonSerializer.Serialize(new { Nom = "Gégé" });
        Check.That(json.Contains("Gégé")).IsEqualTo(___); // {"Nom":"Gégé"}

        // 2. Par défaut, les noms sont sensibles à la casse, et une propriété inconnue est ignorée sans erreur
        //    (Jackson lève une exception sur une propriété inconnue)
        const string depuisLeWeb = """{"nom":"Cervelle nature","categorie":"Cerveau","prixEnDents":12,"stock":10}""";
        var platParDefaut = JsonSerializer.Deserialize<Plat>(depuisLeWeb)!;
        var platWeb = JsonSerializer.Deserialize<Plat>(depuisLeWeb, JsonSerializerOptions.Web)!;

        Check.That(platParDefaut.Nom is null).IsEqualTo(___);
        Check.That(platWeb.PrixEnDents).IsEqualTo(___);
    }

    // 🛠 À toi de coder : lis le fichier d'arrivage (séparateur « ; », une ligne d'en-tête à ignorer).
    //    Indice : File.ReadLinesAsync, await foreach, string.Split, int.Parse
    private static async Task<List<Livraison>> LireArrivageAsync(string chemin)
    {
        throw new NotImplementedException("🧟 À toi de jouer !");
    }

    [Fact]
    public async Task On_sait_lire_un_fichier_CSV()
    {
        var livraisons = await LireArrivageAsync(Arrivage);

        Check.That(livraisons).HasSize(4);
        Check.That(livraisons[0]).IsEqualTo(new Livraison("Cervelle nature", 5));
        Check.That(livraisons.Sum(l => l.Quantite)).IsEqualTo(26);
    }

    // 🛠 À toi de coder : sauvegarde les livraisons en JSON (conventions du web), en écrivant directement dans le fichier.
    //    Indice : File.Create renvoie un flux, JsonSerializer.SerializeAsync écrit dedans
    private static async Task SauvegarderAsync(string chemin, IReadOnlyList<Livraison> livraisons)
    {
        throw new NotImplementedException("🧟 À toi de jouer !");
    }

    [Fact]
    public async Task On_sait_ecrire_un_fichier_JSON()
    {
        var chemin = Path.Combine(_dossier.FullName, "arrivage.json");
        Livraison[] livraisons = [new("Cervelle nature", 5), new("Cornetto", 3)];

        await SauvegarderAsync(chemin, livraisons);

        Check.That(await File.ReadAllTextAsync(chemin))
            .IsEqualTo("""[{"plat":"Cervelle nature","quantite":5},{"plat":"Cornetto","quantite":3}]""");
    }
}
