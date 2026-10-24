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
    // 🔴 Ta liste de tests. Un test à la fois : rouge (il échoue), vert (le minimum pour passer), refactor.
    //    Tu peux renommer, réordonner, ajouter : c'est ta liste.
    //    [ ] Un_rodeur_commande_deux_cervelles
    //    [ ] La_quantite_doit_etre_positive
    //    [ ] On_ne_sert_pas_les_vivants
    //    [ ] Un_zombie_vegan_ne_commande_pas_de_cervelle
    //    [ ] On_ne_sert_pas_de_cerveau_perime
    //    [ ] On_ne_vend_pas_plus_que_le_stock
    //    [ ] Les_regles_s_appliquent_dans_l_ordre
}
