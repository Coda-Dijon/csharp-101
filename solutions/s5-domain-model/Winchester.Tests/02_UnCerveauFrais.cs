namespace Winchester.Tests;

// 📜 Histoire 2 · Un cerveau frais
//
//   En tant que client du Winchester,
//   je veux qu'on ne me serve jamais de cerveau de plus de 3 jours,
//   parce que même un zombie a ses limites.
//
//   Exemples (aujourd'hui, nous sommes le 31 octobre 2026) :
//   - une cervelle préparée aujourd'hui a 0 jour : elle est fraîche
//   - une cervelle préparée le 28 octobre a 3 jours : elle se mange encore
//   - une cervelle préparée le 27 octobre a 4 jours : elle est périmée
//   - un plat a un nom, une catégorie (cerveau ou chou-fleur), un prix en dents et une date de préparation
//
// 💡 Le plat ne lit pas l'horloge lui-même : passe-lui la date du jour. Le test reste vrai demain.
// 🧰 Séance 2 : record, enum · Séance 1 : DateOnly, expressions

public class UnCerveauFrais
{
    // >>> TESTS
    private static Dents Prix(int dents) => Dents.Creer(dents).DoitReussir();

    private static readonly DateOnly Aujourdhui = new(2026, 10, 31);

    private static Plat CervellePrepareeLe(DateOnly date) => new("Cervelle nature", Categorie.Cerveau, Prix(12), date);

    [Fact]
    public void Une_cervelle_du_jour_est_fraiche()
    {
        var cervelle = CervellePrepareeLe(Aujourdhui);

        Check.That(cervelle.Age(Aujourdhui)).IsEqualTo(0);
        Check.That(cervelle.EstPerime(Aujourdhui)).IsFalse();
    }

    [Fact]
    public void Une_cervelle_de_trois_jours_se_mange_encore()
    {
        var cervelle = CervellePrepareeLe(new DateOnly(2026, 10, 28));

        Check.That(cervelle.Age(Aujourdhui)).IsEqualTo(3);
        Check.That(cervelle.EstPerime(Aujourdhui)).IsFalse();
    }

    [Fact]
    public void Une_cervelle_de_quatre_jours_est_perimee()
    {
        var cervelle = CervellePrepareeLe(new DateOnly(2026, 10, 27));

        Check.That(cervelle.EstPerime(Aujourdhui)).IsTrue();
    }
    // <<< TESTS
}
