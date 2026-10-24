namespace Winchester.Tests;

// 📜 Histoire 7 · Le rush de minuit
//
//   En tant que gérant,
//   je veux qu'un comptoir prenne les commandes de toute la horde en même temps,
//   sans jamais vendre une portion que je n'ai plus.
//
//   Le comptoir connaît le stock de chaque plat et l'horloge. Pour chaque commande, il lit le stock,
//   consulte son carnet (une E/S lente, simulée par await Task.Delay(1)), laisse Commande.Passer décider,
//   puis, si la commande est passée, décrémente le stock et la garde.
//
//   Exemples :
//   - 10 cervelles en stock, Bob en commande 2 : il en reste 8, et le comptoir garde la commande
//   - un refus ne touche pas au stock
//   - 10 cervelles en stock, 100 zombies en commandent une chacun, tous en même temps :
//     exactement 10 sont servis, 90 reçoivent une rupture de stock, et il reste 0 cervelle
//   - la horde arrive en flux (IAsyncEnumerable) : le comptoir renvoie un résultat par client, au fil de l'eau
//
// 💡 Le comptoir est la coquille : l'horloge, l'état, les accès concurrents. Les décisions restent dans le cœur.
// 🧰 Séance 4 : async/await, Task.WhenAll, SemaphoreSlim, IAsyncEnumerable, await foreach

public class LeRushDeMinuit
{
    // 🔴 Ta liste de tests. Un test à la fois : rouge (il échoue), vert (le minimum pour passer), refactor.
    //    Tu peux renommer, réordonner, ajouter : c'est ta liste.
    //    [ ] Une_commande_passee_decremente_le_stock
    //    [ ] Un_refus_ne_touche_pas_au_stock
    //    [ ] Cent_zombies_pour_dix_cervelles
    //    [ ] La_horde_arrive_en_flux
}
