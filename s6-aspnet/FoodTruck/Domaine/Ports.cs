namespace FoodTruck.Domaine;

// Les PORTS de l'hexagone : ce dont le cœur a besoin pour accéder aux données, dit dans son langage.
// Ce sont des interfaces : elles ne savent rien de PostgreSQL, de Dapper ni de HTTP.
// Les ADAPTATEURS (Infrastructure/PostgreSql, ou des versions en mémoire pour les tests) les implémentent.
//
// Java / Spring : les interfaces de repository, et Spring Data qui en génère l'implémentation.
// .NET : on écrit l'implémentation (Dapper, Entity Framework Core...), et on la branche dans Program.cs.

public interface IRepositoryDesPlats
{
    Task<IReadOnlyList<Plat>> MenuAsync(string? categorie = null);
    Task<Plat?> TrouverAsync(int id);
}

public interface IRepositoryDesClients
{
    Task<Client?> TrouverAsync(int id);
}

public interface IRepositoryDesCommandes
{
    Task<Commande?> TrouverAsync(int id);

    // Décrémente le stock du plat et enregistre la commande, d'un seul bloc.
    // Renvoie null si le stock ne suffit plus (une autre commande est passée entre-temps).
    Task<Commande?> EnregistrerAsync(NouvelleCommande demande);

    Task<IReadOnlyDictionary<string, int>> ChiffreDAffairesParTypeAsync();
}
