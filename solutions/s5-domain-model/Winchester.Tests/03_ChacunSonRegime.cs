namespace Winchester.Tests;

// 📜 Histoire 3 · Chacun son régime
//
//   En tant que cuisinier,
//   je veux savoir ce que chaque client peut manger,
//   afin de ne pas servir de cerveau à un zombie vegan.
//
//   Les clients :
//   - Bob est un rôdeur : il mange de tout
//   - Gégé est un zombie vegan : il ne mange que du chou-fleur
//   - Shaun est un survivant qui imite très bien les zombies : on ne lui sert rien
//
// 💡 Une sorte de client = un type, pas une chaîne ni un booléen « EstVivant ».
// 🧰 Séance 2 : hiérarchie de records, pattern matching · Séance 3 : méthode d'extension

public class ChacunSonRegime
{
    // >>> TESTS
    private static Dents Prix(int dents) => Dents.Creer(dents).DoitReussir();

    private static readonly DateOnly Hier = new(2026, 10, 30);
    private static readonly Plat Cervelle = new("Cervelle nature", Categorie.Cerveau, Prix(12), Hier);
    private static readonly Plat Gratin = new("Gratin de chou-fleur", Categorie.ChouFleur, Prix(10), Hier);

    [Fact]
    public void Un_rodeur_mange_de_tout()
    {
        var bob = new Rodeur("Bob");

        Check.That(bob.PeutManger(Cervelle)).IsTrue();
        Check.That(bob.PeutManger(Gratin)).IsTrue();
    }

    [Fact]
    public void Un_zombie_vegan_ne_mange_que_du_chou_fleur()
    {
        var gege = new ZombieVegan("Gégé");

        Check.That(gege.PeutManger(Gratin)).IsTrue();
        Check.That(gege.PeutManger(Cervelle)).IsFalse();
    }

    [Fact]
    public void On_ne_sert_rien_a_un_survivant()
    {
        var shaun = new Survivant("Shaun");

        Check.That(shaun.PeutManger(Cervelle)).IsFalse();
        Check.That(shaun.PeutManger(Gratin)).IsFalse();
    }
    // <<< TESTS
}
