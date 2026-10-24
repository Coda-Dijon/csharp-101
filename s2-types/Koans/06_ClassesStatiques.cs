using static System.Math; // ≈ import static java.lang.Math.*;

namespace Types;

// 🎯 Objectif : les classes utilitaires sans bricolage, et un avant-goût des méthodes d'extension (séance 3).

// Java : public final class Conversions { private Conversions() {} public static double ... }
// C#   : `static class` -> ni instanciable, ni héritable, et le compilateur refuse tout membre d'instance
public static class Conversions
{
    public static double EnFahrenheit(double celsius) => celsius * 9 / 5 + 32;
}

// Une méthode d'extension se déclare dans une classe static : le `this` du premier paramètre
// la « greffe » sur le type string, sans le modifier ni en hériter.
public static class ExtensionsDeCri
{
    public static string EnCri(this string texte) => texte.ToUpper() + " !";

    // C# 14 : les blocs `extension` ajoutent aussi des propriétés
    extension(string texte)
    {
        public bool EstUnCri => texte.EndsWith('!');
    }
}

// 🛠 À toi de coder : affiche un prix en dents, la monnaie de l'apocalypse.
//    0 ou 1 au singulier ("0 dent", "1 dent"), au-delà au pluriel ("3 dents")
public static class ExtensionsDePrix
{
    public static string EnDents(this int prix)
    {
        throw new NotImplementedException("🧟 À toi de jouer !");
    }
}

public class AProposDesClassesStatiques
{
    [Fact]
    public void Une_classe_static_s_utilise_sans_instance()
    {
        // new Conversions()  -> ne compile pas
        Check.That(Conversions.EnFahrenheit(100)).IsEqualTo(___);
    }

    [Fact]
    public void Dans_l_IL_une_classe_static_est_abstract_et_sealed()
    {
        // C'est ce que Roslyn génère : impossible à instancier (abstract) ET à hériter (sealed)
        Check.That(typeof(Conversions).IsAbstract).IsEqualTo(___);
        Check.That(typeof(Conversions).IsSealed).IsEqualTo(___);
    }

    [Fact]
    public void Using_static_importe_les_membres_static()
    {
        // Le `using static System.Math;` en haut du fichier permet d'écrire Max au lieu de Math.Max.
        // C'est aussi ce qui vous permet d'écrire ___ au lieu de Blanks.___ dans les koans !
        Check.That(Max(3, 7)).IsEqualTo(___);
        Check.That(Abs(-42)).IsEqualTo(___);
    }

    private static class Apocalypse
    {
        public static int Ouvertures;

        // le constructeur static s'exécute une seule fois, juste avant le premier accès (≈ bloc static { } en Java)
        static Apocalypse() => Ouvertures++;

        public static string Jour => "J+1";
    }

    [Fact]
    public void Le_constructeur_static_ne_s_execute_qu_une_fois()
    {
        _ = Apocalypse.Jour;
        _ = Apocalypse.Jour;
        _ = Apocalypse.Jour;

        Check.That(Apocalypse.Ouvertures).IsEqualTo(___);
    }

    private static class Regles
    {
        public const int MaxZombies = 50; // const est implicitement static

        public static readonly string[] Armes = ["batte", "machette"];
    }

    [Fact]
    public void Static_readonly_protege_la_reference_pas_le_contenu()
    {
        // Regles.Armes = [];  -> ne compile pas... mais comme un `static final` tableau en Java :
        Regles.Armes[0] = "pelle";

        Check.That(Regles.Armes[0]).IsEqualTo(___);
        Check.That(Regles.MaxZombies).IsEqualTo(___);
    }

    private class Bunker
    {
        private static readonly string CodeSecret = "1234";

        // Java : static class Porte  /  C# : toute classe imbriquée est « static » au sens Java,
        // elle n'a pas de référence implicite vers une instance de Bunker... mais voit ses membres privés
        public class Porte
        {
            public bool Ouvrir(string code) => code == CodeSecret;
        }
    }

    [Fact]
    public void Une_classe_imbriquee_s_instancie_sans_la_classe_englobante()
    {
        var porte = new Bunker.Porte(); // Java (inner class) : bunker.new Porte()

        Check.That(porte.Ouvrir("0000")).IsEqualTo(___);
        Check.That(porte.Ouvrir("1234")).IsEqualTo(___);
    }

    [Fact]
    public void Une_methode_d_extension_s_appelle_comme_une_methode_d_instance()
    {
        // Java : StringUtils.enCri("braaains")  /  C# : "braaains".EnCri()
        Check.That("braaains".EnCri()).IsEqualTo(___);
        Check.That(ExtensionsDeCri.EnCri("bof")).IsEqualTo(___); // ce n'est qu'un appel static déguisé
        Check.That("braaains".EnCri().EstUnCri).IsEqualTo(___);
    }

    [Fact]
    public void On_sait_ecrire_une_methode_d_extension()
    {
        Check.That(0.EnDents()).IsEqualTo("0 dent");
        Check.That(1.EnDents()).IsEqualTo("1 dent");
        Check.That(3.EnDents()).IsEqualTo("3 dents");
    }
}
