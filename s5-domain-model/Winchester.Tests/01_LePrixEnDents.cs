namespace Winchester.Tests;

// 📜 Histoire 1 · Le prix en dents
//
//   En tant que cuisinier du Winchester,
//   je veux exprimer mes prix en dents, la seule monnaie qui vaut encore quelque chose,
//   afin de ne jamais me tromper en rendant la monnaie.
//
//   Exemples :
//   - un prix se crée à partir d'un nombre de dents : 12 dents
//   - un prix négatif n'existe pas : la création échoue, avec la raison (PrixNegatif), sans exception
//   - deux prix de 12 dents sont le même prix
//   - 12 dents + 8 dents = 20 dents
//   - 3 cervelles à 12 dents coûtent 36 dents
//   - un prix s'affiche « 12 dents »
//
// 💡 Pas de constructeur qui lance une exception : rien dans « new Dents(-1) » ne dit que ça peut échouer.
//    Constructeur privé + FACTORY METHOD Dents.Creer(...) qui renvoie un Result<Dents> (fourni) :
//    la signature avoue que la création peut échouer, et l'appelant ne peut pas l'ignorer.
// 🧰 Séance 2 : record struct, constructeur privé, opérateurs · Result<T>

public class LePrixEnDents
{
    // 🎁 Le premier test est donné, en commentaire : décommente-le.
    //    Il ne compile pas, Dents n'existe pas encore : c'est le premier rouge du TDD.
    //    Écris le minimum de code pour le faire passer, puis continue avec ta liste de tests.
    // [Fact]
    // public void Un_prix_se_cree_a_partir_d_un_nombre_de_dents()
    // {
    //     var prix = Dents.Creer(12).DoitReussir();

    //     Check.That(prix.Valeur).IsEqualTo(12);
    // }

    // 🔴 Ta liste de tests. Un test à la fois : rouge (il échoue), vert (le minimum pour passer), refactor.
    //    Tu peux renommer, réordonner, ajouter : c'est ta liste.
    //    [ ] Un_prix_negatif_n_existe_pas
    //    [ ] Deux_prix_de_meme_montant_sont_egaux
    //    [ ] Des_dents_s_additionnent
    //    [ ] Le_prix_de_plusieurs_portions_est_le_prix_unitaire_multiplie
    //    [ ] Un_prix_s_affiche_avec_son_unite
}
