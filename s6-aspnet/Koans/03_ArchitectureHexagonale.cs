namespace Ecosysteme;

// 🎯 Objectif : comprendre l'architecture hexagonale (ports & adaptateurs) du FoodTruck.
//
//  Program.cs (HTTP)  ──>  Application (cas d'usage)  ──>  Domaine (modèle + PORTS)
//                                                                    ▲
//                                           Infrastructure (ADAPTATEURS PostgreSQL) ──┘
//
// Les dépendances pointent vers le domaine : le domaine ne connaît ni PostgreSQL, ni HTTP.
// Les ports sont des interfaces (Domaine/Ports.cs) ; Program.cs branche chacun sur un adaptateur.
// Conséquence : on peut brancher d'autres adaptateurs, par exemple en mémoire, pour tester le cas d'usage
// sans base de données ni serveur. Ici, aucun Docker : ces tests tournent en quelques millisecondes.

// Un entrepôt en mémoire, partagé par les adaptateurs du test (l'équivalent des tables)
public sealed class EntrepotEnMemoire
{
    public List<Plat> Plats { get; } =
    [
        new(1, "Cervelle nature", "Cerveau", 12, 10),
        new(2, "Cerveau flambé", "Cerveau", 20, 3),
        new(3, "Tartare de neurones", "Cerveau", 15, 0),
        new(4, "Chou-fleur vapeur", "Chou-fleur", 8, 12),
        new(5, "Gratin de chou-fleur", "Chou-fleur", 10, 5)
    ];

    public List<Client> Clients { get; } =
    [
        new(1, "Bob", "Rodeur", false),
        new(3, "Gégé", "Vegan", false),
        new(4, "Shaun", "Rodeur", true)
    ];

    public List<Commande> Commandes { get; } = [];
}

public sealed class ClientsEnMemoire(EntrepotEnMemoire entrepot) : IRepositoryDesClients
{
    // Un port async implémenté de façon synchrone : Task.FromResult emballe la valeur dans une Task déjà terminée
    public Task<Client?> TrouverAsync(int id) => Task.FromResult(entrepot.Clients.SingleOrDefault(c => c.Id == id));
}

public sealed class CommandesEnMemoire(EntrepotEnMemoire entrepot) : IRepositoryDesCommandes
{
    public Task<Commande?> TrouverAsync(int id) => Task.FromResult(entrepot.Commandes.SingleOrDefault(c => c.Id == id));

    public Task<Commande?> EnregistrerAsync(NouvelleCommande demande)
    {
        var index = entrepot.Plats.FindIndex(p => p.Id == demande.PlatId);
        var plat = entrepot.Plats[index];
        if (plat.Stock < demande.Quantite) return Task.FromResult<Commande?>(null);

        entrepot.Plats[index] = plat with { Stock = plat.Stock - demande.Quantite };
        var commande = new Commande(entrepot.Commandes.Count + 1, demande.ClientId, demande.PlatId, demande.Quantite, DateTime.UtcNow);
        entrepot.Commandes.Add(commande);
        return Task.FromResult<Commande?>(commande);
    }

    public Task<IReadOnlyDictionary<string, int>> ChiffreDAffairesParTypeAsync() =>
        throw new NotSupportedException("Inutile pour ces tests");
}

// 🛠 À toi de coder : l'adaptateur en mémoire du port des plats, qui lit entrepot.Plats.
//    MenuAsync : tous les plats, ou seulement ceux de la catégorie demandée.
public sealed class PlatsEnMemoire(EntrepotEnMemoire entrepot) : IRepositoryDesPlats
{
    public Task<IReadOnlyList<Plat>> MenuAsync(string? categorie = null)
    {
        throw new NotImplementedException("🧟 À toi de jouer !");
    }

    public Task<Plat?> TrouverAsync(int id)
    {
        throw new NotImplementedException("🧟 À toi de jouer !");
    }
}

public class AProposDeLArchitectureHexagonale
{
    private readonly EntrepotEnMemoire _entrepot = new();

    // Sans conteneur d'injection : on branche les adaptateurs à la main
    private PrendreUneCommande CasDUsage() =>
        new(new ClientsEnMemoire(_entrepot), new PlatsEnMemoire(_entrepot), new CommandesEnMemoire(_entrepot));

    [Fact]
    public void Le_cas_d_usage_ne_depend_que_des_ports()
    {
        var dependances = typeof(PrendreUneCommande).GetConstructors().Single().GetParameters()
            .Select(p => p.ParameterType);

        Check.That(dependances.All(t => t.IsInterface)).IsEqualTo(___);
        Check.That(dependances.Any(t => t.Namespace!.StartsWith("Npgsql"))).IsEqualTo(___);
    }

    [Fact]
    public async Task On_sait_ecrire_un_adaptateur_en_memoire()
    {
        var plats = new PlatsEnMemoire(_entrepot);

        Check.That(await plats.MenuAsync()).HasSize(5);
        Check.That(await plats.MenuAsync("Chou-fleur")).HasSize(2);
        Check.That((await plats.TrouverAsync(2))!.Nom).IsEqualTo("Cerveau flambé");
        Check.That(await plats.TrouverAsync(99)).IsNull();
    }

    [Fact]
    public async Task Le_cas_d_usage_se_teste_sans_base_ni_serveur()
    {
        var resultat = await CasDUsage().ExecuterAsync(new NouvelleCommande(ClientId: 1, PlatId: 1, Quantite: 2));

        Check.That(resultat is CommandeServie).IsEqualTo(___);
        Check.That(_entrepot.Plats[0].Stock).IsEqualTo(___);    // la cervelle a été servie...
        Check.That(_entrepot.Commandes.Count).IsEqualTo(___);   // ...et la commande enregistrée
    }

    [Fact]
    public async Task Les_refus_ne_touchent_pas_aux_donnees()
    {
        var shaun = await CasDUsage().ExecuterAsync(new NouvelleCommande(ClientId: 4, PlatId: 1, Quantite: 1));
        var inconnu = await CasDUsage().ExecuterAsync(new NouvelleCommande(ClientId: 99, PlatId: 1, Quantite: 1));

        Check.That(shaun is CommandeRefusee { Refus: ClientEncoreVivant }).IsEqualTo(___);
        Check.That(inconnu is ClientOuPlatIntrouvable).IsEqualTo(___);
        Check.That(_entrepot.Commandes.Count).IsEqualTo(___);
    }
}
