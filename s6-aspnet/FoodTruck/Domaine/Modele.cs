namespace FoodTruck.Domaine;

// Le domaine du Food-Truck Zombie (séance 5), réduit à ce qu'on stocke dans PostgreSQL.
public sealed record Plat(int Id, string Nom, string Categorie, int PrixEnDents, int Stock);

public sealed record Client(int Id, string Nom, string Type, bool EstVivant);

public sealed record Commande(int Id, int ClientId, int PlatId, int Quantite, DateTime PasseeLe);

public sealed record NouvelleCommande(int ClientId, int PlatId, int Quantite);

// La décision de la cuisine : une hiérarchie fermée, exploitée par pattern matching (séance 2)
public abstract record Decision;
public sealed record Acceptee : Decision;
public sealed record ClientEncoreVivant(string Nom) : Decision;
public sealed record RegimeNonRespecte(string Nom, string Categorie) : Decision;
public sealed record RuptureDeStock(string Plat, int Restant) : Decision;

// Functional core : aucune base, aucun HTTP, juste une fonction pure facile à tester.
// L'imperative shell (les endpoints) va chercher les données, appelle Decider, puis agit.
public static class Cuisine
{
    // 🛠 À toi de coder : les règles, dans cet ordre.
    //    1. Un client encore vivant n'est pas servi           -> ClientEncoreVivant
    //    2. Un client « Vegan » ne mange que du « Chou-fleur »  -> RegimeNonRespecte
    //    3. Pas assez de stock pour la quantité demandée      -> RuptureDeStock (avec le stock restant)
    //    4. Sinon                                             -> Acceptee
    public static Decision Decider(Client client, Plat plat, int quantite)
    {
        throw new NotImplementedException("🧟 À toi de jouer !");
    }
}
