namespace Winchester;

// Histoire 4 · Passer une commande (séance 2 : constructeur privé + factory ; Result<T> plutôt que des exceptions)
// Histoire 5 · La vie d'une commande (séance 2 : hiérarchie d'états, pattern matching, immuabilité avec with)

// >>> MODELE
// Les refus du domaine : un record par raison, avec ce qu'il faut pour l'expliquer
public sealed record QuantiteInvalide(int Quantite) : Erreur;
public sealed record ClientEncoreVivant(string Nom) : Erreur;
public sealed record RegimeNonRespecte(string Nom, Categorie Categorie) : Erreur;
public sealed record CerveauPerime(string Plat, int Age) : Erreur;
public sealed record RuptureDeStock(string Plat, int Restant) : Erreur;
public sealed record TransitionImpossible(EtatDeCommande Depuis, string Action) : Erreur;

// Les états : chacun ne porte que les données qui ont du sens pour lui.
// Pas de « string Cuisinier » nullable sur une commande qui n'est pas encore en cuisine.
public abstract record EtatDeCommande;
public sealed record Passee : EtatDeCommande;
public sealed record EnPreparation(string Cuisinier) : EtatDeCommande;
public sealed record Servie : EtatDeCommande;
public sealed record Devoree(string ParLeCuisinier) : EtatDeCommande;

// Un AGRÉGAT : on ne peut le créer et le faire évoluer qu'à travers ses méthodes, qui garantissent ses invariants.
public sealed record Commande
{
    public Client Client { get; }
    public Plat Plat { get; }
    public int Quantite { get; }
    public EtatDeCommande Etat { get; private init; } = new Passee();

    public Dents Total => Plat.Prix * (uint)Quantite;   // Quantite > 0 : garanti par Passer

    // Privé : impossible de construire une commande sans passer par les règles
    private Commande(Client client, Plat plat, int quantite) =>
        (Client, Plat, Quantite) = (client, plat, quantite);

    // La factory : toutes les entrées en paramètres, y compris le stock et la date. Aucun effet de bord.
    // L'ordre des règles compte : un survivant est refusé comme vivant, même s'il demande un plat périmé.
    public static Result<Commande> Passer(Client client, Plat plat, int quantite, int enStock, DateOnly aujourdhui) =>
        client switch
        {
            _ when quantite <= 0 => new QuantiteInvalide(quantite),
            Survivant survivant => new ClientEncoreVivant(survivant.Nom),
            _ when !client.PeutManger(plat) => new RegimeNonRespecte(client.Nom, plat.Categorie),
            _ when plat.EstPerime(aujourdhui) => new CerveauPerime(plat.Nom, plat.Age(aujourdhui)),
            _ when enStock < quantite => new RuptureDeStock(plat.Nom, enStock),
            _ => new Commande(client, plat, quantite)
        };

    // Chaque transition renvoie une NOUVELLE commande (with), ou explique pourquoi elle est impossible
    public Result<Commande> Preparer(string cuisinier) => Etat switch
    {
        Passee => this with { Etat = new EnPreparation(cuisinier) },
        _ => new TransitionImpossible(Etat, nameof(Preparer))
    };

    public Result<Commande> Servir() => Etat switch
    {
        EnPreparation => this with { Etat = new Servie() },
        _ => new TransitionImpossible(Etat, nameof(Servir))
    };

    public Result<Commande> EtreDevoreeEnCuisine() => Etat switch
    {
        EnPreparation enCuisine => this with { Etat = new Devoree(enCuisine.Cuisinier) },
        _ => new TransitionImpossible(Etat, nameof(EtreDevoreeEnCuisine))
    };

    public bool EstTerminee => Etat is Servie or Devoree;
}
// <<< MODELE
