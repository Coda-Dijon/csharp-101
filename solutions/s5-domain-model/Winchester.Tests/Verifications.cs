using Xunit.Sdk;

namespace Winchester.Tests;

// 🎁 Fourni : deux raccourcis pour vérifier un Result<T> dans les tests
//   var commande = Commande.Passer(...).DoitReussir();
//   Check.That(Commande.Passer(...).DoitEchouer()).IsEqualTo(new ClientEncoreVivant("Shaun"));
public static class Verifications
{
    public static T DoitReussir<T>(this Result<T> resultat) => resultat.Match(
        valeur => valeur,
        erreur => throw new XunitException($"Un succès était attendu, mais : {erreur}"));

    public static Erreur DoitEchouer<T>(this Result<T> resultat) => resultat.Match(
        valeur => throw new XunitException($"Un échec était attendu, mais : {valeur}"),
        erreur => erreur);
}
