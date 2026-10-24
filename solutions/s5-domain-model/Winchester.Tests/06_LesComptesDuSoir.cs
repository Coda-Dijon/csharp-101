namespace Winchester.Tests;

// 📜 Histoire 6 · Les comptes du soir
//
//   En tant que gérant du Winchester,
//   je veux faire mes comptes à la fermeture,
//   afin de savoir ce qui se vend et ce que me coûte mon cuisinier.
//
//   Ce soir : Bob a été servi de 2 cervelles (24 dents) puis d'1 cervelle (12 dents),
//   Gégé d'1 gratin de chou-fleur (10 dents), et la commande de 3 cervelles de Zed
//   a été dévorée en cuisine (36 dents perdues). Une dernière commande est encore en cuisine.
//
//   Exemples :
//   - le chiffre d'affaires ne compte que les commandes servies : 46 dents
//   - le plat le plus populaire, en portions servies : la cervelle
//   - le chiffre d'affaires par sorte de client : rôdeurs 36 dents, vegans 10 dents
//   - les pertes en cuisine : 36 dents
//   - sans commande servie : 0 dent, et pas de plat le plus populaire
//
// 🧰 Séance 3 : Where, GroupBy, MaxBy, Aggregate, ToDictionary, fonctions pures

public class LesComptesDuSoir
{
    // >>> TESTS
    private static Dents Prix(int dents) => Dents.Creer(dents).DoitReussir();

    private static readonly DateOnly Aujourdhui = new(2026, 10, 31);
    private static readonly Plat Cervelle = new("Cervelle nature", Categorie.Cerveau, Prix(12), Aujourdhui);
    private static readonly Plat Gratin = new("Gratin de chou-fleur", Categorie.ChouFleur, Prix(10), Aujourdhui);

    private static Commande Servie(Client client, Plat plat, int quantite) =>
        Commande.Passer(client, plat, quantite, 100, Aujourdhui)
            .Bind(c => c.Preparer("Gégé"))
            .Bind(c => c.Servir())
            .DoitReussir();

    private static readonly Commande[] LaSoiree =
    [
        Servie(new Rodeur("Bob"), Cervelle, 2),
        Servie(new Rodeur("Bob"), Cervelle, 1),
        Servie(new ZombieVegan("Gégé"), Gratin, 1),
        Commande.Passer(new Rodeur("Zed"), Cervelle, 3, 100, Aujourdhui)
            .Bind(c => c.Preparer("Gégé"))
            .Bind(c => c.EtreDevoreeEnCuisine())
            .DoitReussir(),
        Commande.Passer(new Rodeur("Zed"), Gratin, 5, 100, Aujourdhui).DoitReussir()
    ];

    [Fact]
    public void Seules_les_commandes_servies_rapportent()
    {
        Check.That(Statistiques.ChiffreDAffaires(LaSoiree)).IsEqualTo(Prix(46));
    }

    [Fact]
    public void La_cervelle_est_le_plat_le_plus_populaire()
    {
        Check.That(Statistiques.PlatLePlusPopulaire(LaSoiree)).IsEqualTo("Cervelle nature");
    }

    [Fact]
    public void Le_chiffre_d_affaires_par_sorte_de_client()
    {
        var parSorte = Statistiques.ChiffreDAffairesParSorteDeClient(LaSoiree);

        Check.That(parSorte["Rodeur"]).IsEqualTo(Prix(36));
        Check.That(parSorte["ZombieVegan"]).IsEqualTo(Prix(10));
        Check.That(parSorte.Count).IsEqualTo(2);
    }

    [Fact]
    public void Les_pertes_en_cuisine()
    {
        Check.That(Statistiques.PertesEnCuisine(LaSoiree)).IsEqualTo(Prix(36));
    }

    [Fact]
    public void Une_soiree_sans_client()
    {
        Check.That(Statistiques.ChiffreDAffaires([])).IsEqualTo(Dents.Zero);
        Check.That(Statistiques.PlatLePlusPopulaire([])).IsNull();
    }
    // <<< TESTS
}
