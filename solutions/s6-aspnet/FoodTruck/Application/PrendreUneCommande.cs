using FoodTruck.Domaine;

namespace FoodTruck.Application;

// Le résultat du cas d'usage : encore une hiérarchie fermée, que l'API traduira en HTTP
public abstract record ResultatDePrise;
public sealed record CommandeServie(Commande Commande) : ResultatDePrise;
public sealed record CommandeRefusee(Decision Refus) : ResultatDePrise;
public sealed record ClientOuPlatIntrouvable : ResultatDePrise;

// Un CAS D'USAGE : il orchestre. Il lit les données par les ports, laisse le domaine décider, puis enregistre.
// Il ne dépend que d'interfaces : on peut le tester avec des repositories en mémoire, sans base ni serveur.
public sealed class PrendreUneCommande(
    IRepositoryDesClients clients,
    IRepositoryDesPlats plats,
    IRepositoryDesCommandes commandes)
{
    // 🛠 À toi de coder : prendre une commande.
    //    - client ou plat introuvable           -> ClientOuPlatIntrouvable
    //    - Cuisine.Decider refuse               -> CommandeRefusee, avec la décision
    //    - sinon, commandes.EnregistrerAsync    -> CommandeServie
    //      (si EnregistrerAsync renvoie null, le stock a été vidé entre-temps : RuptureDeStock, avec 0 restant)
    public async Task<ResultatDePrise> ExecuterAsync(NouvelleCommande demande)
    {
        // >>> SOLUTION
        var client = await clients.TrouverAsync(demande.ClientId);
        var plat = await plats.TrouverAsync(demande.PlatId);
        if (client is null || plat is null) return new ClientOuPlatIntrouvable();

        var decision = Cuisine.Decider(client, plat, demande.Quantite);
        if (decision is not Acceptee) return new CommandeRefusee(decision);

        var commande = await commandes.EnregistrerAsync(demande);
        return commande is null
            ? new CommandeRefusee(new RuptureDeStock(plat.Nom, 0))
            : new CommandeServie(commande);
        // <<< SOLUTION
    }
}
