namespace Winchester.Tests;

// 📜 Histoire 7 · Le rush de minuit
//
//   En tant que gérant,
//   je veux qu'un comptoir prenne les commandes de toute la horde en même temps,
//   sans jamais vendre une portion que je n'ai plus.
//
//   Le comptoir connaît le stock de chaque plat et l'horloge. Pour chaque commande, il lit le stock,
//   consulte son carnet (une E/S lente, simulée par await Task.Delay(1)), laisse Commande.Passer décider,
//   puis, si la commande est passée, décrémente le stock et la garde.
//
//   Exemples :
//   - 10 cervelles en stock, Bob en commande 2 : il en reste 8, et le comptoir garde la commande
//   - un refus ne touche pas au stock
//   - 10 cervelles en stock, 100 zombies en commandent une chacun, tous en même temps :
//     exactement 10 sont servis, 90 reçoivent une rupture de stock, et il reste 0 cervelle
//   - la horde arrive en flux (IAsyncEnumerable) : le comptoir renvoie un résultat par client, au fil de l'eau
//
// 💡 Le comptoir est la coquille : l'horloge, l'état, les accès concurrents. Les décisions restent dans le cœur.
// 🧰 Séance 4 : async/await, Task.WhenAll, SemaphoreSlim, IAsyncEnumerable, await foreach

public class LeRushDeMinuit
{
    // >>> TESTS
    private static Dents Prix(int dents) => Dents.Creer(dents).DoitReussir();

    private static readonly DateOnly Aujourdhui = new(2026, 10, 31);
    private static readonly Plat Cervelle = new("Cervelle nature", Categorie.Cerveau, Prix(12), Aujourdhui);

    private static Comptoir UnComptoirAvec(int cervelles) =>
        new(new Dictionary<string, int> { [Cervelle.Nom] = cervelles }, () => Aujourdhui);

    [Fact]
    public async Task Une_commande_passee_decremente_le_stock()
    {
        var comptoir = UnComptoirAvec(10);

        var resultat = await comptoir.PrendreAsync(new Rodeur("Bob"), Cervelle, 2);

        Check.That(resultat.DoitReussir().Quantite).IsEqualTo(2);
        Check.That(comptoir.Restant(Cervelle.Nom)).IsEqualTo(8);
        Check.That(comptoir.Commandes).HasSize(1);
    }

    [Fact]
    public async Task Un_refus_ne_touche_pas_au_stock()
    {
        var comptoir = UnComptoirAvec(10);

        var resultat = await comptoir.PrendreAsync(new Survivant("Shaun"), Cervelle, 2);

        Check.That(resultat.DoitEchouer()).IsInstanceOf<ClientEncoreVivant>();
        Check.That(comptoir.Restant(Cervelle.Nom)).IsEqualTo(10);
        Check.That(comptoir.Commandes).IsEmpty();
    }

    [Fact]
    public async Task Cent_zombies_pour_dix_cervelles()
    {
        var comptoir = UnComptoirAvec(10);

        var resultats = await Task.WhenAll(
            Enumerable.Range(1, 100).Select(i => comptoir.PrendreAsync(new Rodeur($"Zombie {i}"), Cervelle, 1)));

        Check.That(resultats.Count(r => r is Result<Commande>.Succes)).IsEqualTo(10);
        Check.That(resultats.Count(r => r is Result<Commande>.Echec { Erreur: RuptureDeStock })).IsEqualTo(90);
        Check.That(comptoir.Restant(Cervelle.Nom)).IsEqualTo(0);
    }

    [Fact]
    public async Task La_horde_arrive_en_flux()
    {
        var comptoir = UnComptoirAvec(2);

        var refus = new List<Erreur>();
        await foreach (var resultat in comptoir.ServirLaFileAsync(LaHorde()))
            resultat.Match(_ => 0, erreur => { refus.Add(erreur); return 0; });

        Check.That(comptoir.Commandes.Select(c => c.Client.Nom)).ContainsExactly("Bob", "Zed");
        Check.That(refus).ContainsExactly(new ClientEncoreVivant("Shaun"), new RuptureDeStock("Cervelle nature", 0));
    }

    private static async IAsyncEnumerable<(Client, Plat, int)> LaHorde()
    {
        Client[] arrivees = [new Rodeur("Bob"), new Survivant("Shaun"), new Rodeur("Zed"), new Rodeur("Ash")];
        foreach (var client in arrivees)
        {
            await Task.Delay(1);   // les zombies arrivent un par un
            yield return (client, Cervelle, 1);
        }
    }
    // <<< TESTS
}
