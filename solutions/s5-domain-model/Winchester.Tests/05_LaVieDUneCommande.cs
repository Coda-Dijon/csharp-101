namespace Winchester.Tests;

// 📜 Histoire 5 · La vie d'une commande
//
//   En tant que chef du Winchester,
//   je veux suivre chaque commande de la prise à la livraison,
//   y compris quand mon cuisinier, affamé, la dévore lui-même.
//
//        Passée ──Preparer(cuisinier)──> En préparation ──Servir()───────────────> Servie
//                                                       └──EtreDevoreeEnCuisine()──> Dévorée (par le cuisinier)
//
//   Exemples :
//   - une commande passée part en cuisine chez Gégé : elle est en préparation chez Gégé
//   - une commande en préparation est servie
//   - une commande en préparation chez Gégé peut être dévorée : par Gégé
//   - on ne sert pas une commande qui n'est pas passée en cuisine : transition impossible
//   - une commande servie ou dévorée ne change plus
//   - faire évoluer une commande ne modifie pas l'originale
//
// 💡 Chaque état est un type, qui ne porte que ses propres données. Chaque transition renvoie
//    une nouvelle commande (with) ou une erreur : Result<Commande> encore, et Bind pour enchaîner.
// 🧰 Séance 2 : hiérarchie, pattern matching, immuabilité · Séance 3 : fonctions qui s'enchaînent

public class LaVieDUneCommande
{
    // >>> TESTS
    private static Dents Prix(int dents) => Dents.Creer(dents).DoitReussir();

    private static readonly DateOnly Aujourdhui = new(2026, 10, 31);
    private static readonly Plat Cervelle = new("Cervelle nature", Categorie.Cerveau, Prix(12), Aujourdhui);

    private static Commande UneCommandePassee() =>
        Commande.Passer(new Rodeur("Bob"), Cervelle, 1, 10, Aujourdhui).DoitReussir();

    [Fact]
    public void Une_commande_passee_part_en_cuisine()
    {
        var enCuisine = UneCommandePassee().Preparer("Gégé").DoitReussir();

        Check.That(enCuisine.Etat).IsEqualTo(new EnPreparation("Gégé"));
    }

    [Fact]
    public void Une_commande_en_preparation_est_servie()
    {
        var servie = UneCommandePassee().Preparer("Gégé").Bind(c => c.Servir()).DoitReussir();

        Check.That(servie.Etat).IsEqualTo(new Servie());
        Check.That(servie.EstTerminee).IsTrue();
    }

    [Fact]
    public void Le_cuisinier_peut_devorer_la_commande()
    {
        var devoree = UneCommandePassee().Preparer("Gégé").Bind(c => c.EtreDevoreeEnCuisine()).DoitReussir();

        Check.That(devoree.Etat).IsEqualTo(new Devoree("Gégé"));
    }

    [Fact]
    public void On_ne_sert_pas_une_commande_qui_n_est_pas_en_cuisine()
    {
        var erreur = UneCommandePassee().Servir().DoitEchouer();

        Check.That(erreur).IsEqualTo(new TransitionImpossible(new Passee(), "Servir"));
    }

    [Fact]
    public void Une_commande_servie_ne_change_plus()
    {
        var servie = UneCommandePassee().Preparer("Gégé").Bind(c => c.Servir()).DoitReussir();

        Check.That(servie.Preparer("Bob").DoitEchouer()).IsInstanceOf<TransitionImpossible>();
        Check.That(servie.EtreDevoreeEnCuisine().DoitEchouer()).IsInstanceOf<TransitionImpossible>();
    }

    [Fact]
    public void Faire_evoluer_une_commande_ne_modifie_pas_l_originale()
    {
        var passee = UneCommandePassee();

        passee.Preparer("Gégé").DoitReussir();

        Check.That(passee.Etat).IsEqualTo(new Passee());
    }

    [Fact]
    public void Le_premier_echec_court_circuite_la_suite()
    {
        // Servir avant de préparer échoue : Bind ne lance même pas la suite
        var resultat = UneCommandePassee().Servir().Bind(c => c.EtreDevoreeEnCuisine());

        Check.That(resultat.DoitEchouer()).IsEqualTo(new TransitionImpossible(new Passee(), "Servir"));
    }
    // <<< TESTS
}
