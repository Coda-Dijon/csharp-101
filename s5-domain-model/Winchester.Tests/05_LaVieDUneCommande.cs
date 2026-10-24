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
    // 🔴 Ta liste de tests. Un test à la fois : rouge (il échoue), vert (le minimum pour passer), refactor.
    //    Tu peux renommer, réordonner, ajouter : c'est ta liste.
    //    [ ] Une_commande_passee_part_en_cuisine
    //    [ ] Une_commande_en_preparation_est_servie
    //    [ ] Le_cuisinier_peut_devorer_la_commande
    //    [ ] On_ne_sert_pas_une_commande_qui_n_est_pas_en_cuisine
    //    [ ] Une_commande_servie_ne_change_plus
    //    [ ] Faire_evoluer_une_commande_ne_modifie_pas_l_originale
    //    [ ] Le_premier_echec_court_circuite_la_suite
}
