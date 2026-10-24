namespace Winchester.Tests;

// 📜 Histoire 4 · Passer une commande
//
//   En tant que client affamé,
//   je veux commander un plat en quelques portions,
//   et savoir précisément pourquoi on refuse ma commande.
//
//   Règles, dans cet ordre :
//   1. la quantité doit être positive
//   2. on ne sert pas les vivants (Shaun)
//   3. chacun son régime (histoire 3)
//   4. pas de plat périmé (histoire 2)
//   5. pas plus de portions qu'il n'en reste en stock
//   Sinon : la commande est passée, et son total vaut prix × quantité.
//
//   Exemples (aujourd'hui, le 31 octobre 2026) :
//   - Bob commande 2 cervelles à 12 dents, 10 en stock : commande passée, 24 dents
//   - Bob commande 0 cervelle : quantité invalide
//   - Shaun commande une cervelle : client encore vivant
//   - Gégé commande une cervelle : régime non respecté
//   - Bob commande un tartare préparé le 20 octobre : cerveau périmé, 11 jours
//   - Bob commande 3 cervelles, il en reste 1 : rupture de stock
//
// 💡 Un refus n'est pas une exception : Passer renvoie un Result<Commande> (fourni dans Result.cs).
//    Constructeur privé + méthode statique : impossible de créer une commande sans passer par les règles.
// 🧰 Séance 2 : encapsulation, factory, generics · Séance 1 : switch expression

public class PasserUneCommande
{
    // >>> TESTS
    private static readonly DateOnly Aujourdhui = new(2026, 10, 31);
    private static readonly Plat Cervelle = new("Cervelle nature", Categorie.Cerveau, new Dents(12), Aujourdhui);
    private static readonly Rodeur Bob = new("Bob");

    [Fact]
    public void Un_rodeur_commande_deux_cervelles()
    {
        var commande = Commande.Passer(Bob, Cervelle, quantite: 2, enStock: 10, Aujourdhui).DoitReussir();

        Check.That(commande.Client).IsEqualTo(Bob);
        Check.That(commande.Total).IsEqualTo(new Dents(24));
        Check.That(commande.Etat).IsEqualTo(new Passee());
    }

    [Fact]
    public void La_quantite_doit_etre_positive()
    {
        var erreur = Commande.Passer(Bob, Cervelle, quantite: 0, enStock: 10, Aujourdhui).DoitEchouer();

        Check.That(erreur).IsEqualTo(new QuantiteInvalide(0));
    }

    [Fact]
    public void On_ne_sert_pas_les_vivants()
    {
        var erreur = Commande.Passer(new Survivant("Shaun"), Cervelle, 1, 10, Aujourdhui).DoitEchouer();

        Check.That(erreur).IsEqualTo(new ClientEncoreVivant("Shaun"));
    }

    [Fact]
    public void Un_zombie_vegan_ne_commande_pas_de_cervelle()
    {
        var erreur = Commande.Passer(new ZombieVegan("Gégé"), Cervelle, 1, 10, Aujourdhui).DoitEchouer();

        Check.That(erreur).IsEqualTo(new RegimeNonRespecte("Gégé", Categorie.Cerveau));
    }

    [Fact]
    public void On_ne_sert_pas_de_cerveau_perime()
    {
        var tartare = new Plat("Tartare de neurones", Categorie.Cerveau, new Dents(15), new DateOnly(2026, 10, 20));

        var erreur = Commande.Passer(Bob, tartare, 1, 10, Aujourdhui).DoitEchouer();

        Check.That(erreur).IsEqualTo(new CerveauPerime("Tartare de neurones", 11));
    }

    [Fact]
    public void On_ne_vend_pas_plus_que_le_stock()
    {
        var erreur = Commande.Passer(Bob, Cervelle, quantite: 3, enStock: 1, Aujourdhui).DoitEchouer();

        Check.That(erreur).IsEqualTo(new RuptureDeStock("Cervelle nature", 1));
    }

    [Fact]
    public void Les_regles_s_appliquent_dans_l_ordre()
    {
        var perime = Cervelle with { PrepareLe = new DateOnly(2026, 10, 1) };

        // Shaun demande un plat périmé, en rupture : on le refuse d'abord parce qu'il est vivant
        var erreur = Commande.Passer(new Survivant("Shaun"), perime, 5, 0, Aujourdhui).DoitEchouer();

        Check.That(erreur).IsInstanceOf<ClientEncoreVivant>();
    }
    // <<< TESTS
}
