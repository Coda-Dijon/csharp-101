namespace Winchester.Tests;

// 📜 Histoire 1 · Le prix en dents
//
//   En tant que cuisinier du Winchester,
//   je veux exprimer mes prix en dents, la seule monnaie qui vaut encore quelque chose,
//   afin de ne jamais me tromper en rendant la monnaie.
//
//   Exemples :
//   - 12 dents + 8 dents = 20 dents
//   - deux prix de 12 dents sont le même prix
//   - un prix négatif n'existe pas : c'est un bug, l'erreur doit être immédiate
//   - 3 cervelles à 12 dents coûtent 36 dents
//   - un prix s'affiche « 12 dents »
//
// 🧰 Séance 2 : record struct, égalité structurelle, opérateurs · Séance 1 : exceptions

public class LePrixEnDents
{
    // 🎁 Le premier test est donné, en commentaire : décommente-le.
    //    Il ne compile pas, Dents n'existe pas encore : c'est le premier rouge du TDD.
    //    Écris le minimum de code pour le faire passer, puis continue avec ta liste de tests.
    // [Fact]
    // public void Des_dents_s_additionnent()
    // {
    //     var total = new Dents(12) + new Dents(8);

    //     Check.That(total).IsEqualTo(new Dents(20));
    // }

    // 🔴 Ta liste de tests. Un test à la fois : rouge (il échoue), vert (le minimum pour passer), refactor.
    //    Tu peux renommer, réordonner, ajouter : c'est ta liste.
    //    [ ] Deux_prix_de_meme_montant_sont_egaux
    //    [ ] Un_prix_ne_peut_pas_etre_negatif
    //    [ ] Le_prix_de_plusieurs_portions_est_le_prix_unitaire_multiplie
    //    [ ] Un_prix_s_affiche_avec_son_unite
}
