// Les alias de type se déclarent en haut du fichier, avec using (voir Un_alias_donne_un_nom_a_un_type_existant)
using Position = (int Ligne, int Colonne);
using Dents = int;

namespace Fundamentals;

// 🎯 Objectif : retrouver ses repères Java dans la syntaxe C#.
// Remplace chaque ___ par la bonne valeur, puis lance `dotnet test`.
public class AProposDeLaSyntaxe
{
    [Fact]
    public void Var_infere_le_type_a_la_compilation()
    {
        // Java 10+ : var x = 42;  -> même idée, le typage reste statique
        var x = 42;

        Check.That(x.GetType()).IsEqualTo(/*__*/typeof(int)/*__*/);
    }

    [Fact]
    public void Les_types_primitifs_sont_des_alias_de_types_dotnet()
    {
        // Java : int (primitif) vs Integer (boxé) sont deux types distincts.
        // C#   : int est juste un alias de System.Int32, et c'est une struct.
        Check.That(typeof(int) == typeof(System.Int32)).IsEqualTo(/*__*/true/*__*/);
        Check.That(42.GetType().Name).IsEqualTo(/*__*/"Int32"/*__*/);
    }

    [Fact]
    public void Les_types_primitifs_ont_des_methodes()
    {
        // Java : Integer.toString(42)  /  C# : on appelle directement sur la valeur
        Check.That(42.ToString()).IsEqualTo(/*__*/"42"/*__*/);
        Check.That(int.MaxValue).IsEqualTo(/*__*/2147483647/*__*/);
    }

    [Fact]
    public void Const_est_evalue_a_la_compilation()
    {
        // Java : static final int  /  C# : const (forcément connu à la compilation)
        const int reponse = 6 * 7;

        Check.That(reponse).IsEqualTo(/*__*/42/*__*/);
    }

    [Fact]
    public void L_interpolation_de_chaines_remplace_String_format()
    {
        // Java : String.format("%s a %d ans", nom, age)  ou  "..." + age
        var nom = "Rick";
        var age = 70;

        Check.That($"{nom} a {age} ans").IsEqualTo(/*__*/"Rick a 70 ans"/*__*/);
        Check.That($"Dans 10 ans : {age + 10}").IsEqualTo(/*__*/"Dans 10 ans : 80"/*__*/);
    }

    [Fact]
    public void Les_raw_string_literals_evitent_les_echappements()
    {
        // Java 15 : text blocks """ ... """  /  C# 11 : raw string literals
        var json = """
                   {"nom": "Rick"}
                   """;

        // L'indentation à gauche du """ fermant est retirée automatiquement
        Check.That(json).IsEqualTo(/*__*/"{\"nom\": \"Rick\"}"/*__*/);
    }

    [Fact]
    public void Les_methodes_peuvent_avoir_des_parametres_par_defaut_et_nommes()
    {
        // Java : pas de paramètres par défaut, il faut des surcharges (ou un builder)
        static string Saluer(string nom, string salutation = "Bonjour") => $"{salutation} {nom}";

        Check.That(Saluer("Morty")).IsEqualTo(/*__*/"Bonjour Morty"/*__*/);
        Check.That(Saluer(salutation: "Wubba lubba dub dub", nom: "Rick")).IsEqualTo(/*__*/"Wubba lubba dub dub Rick"/*__*/);
    }

    [Fact]
    public void Les_methodes_courtes_s_ecrivent_avec_une_fleche()
    {
        // Expression-bodied member : pas de { return ...; }
        static int Carre(int x) => x * x;

        Check.That(Carre(9)).IsEqualTo(/*__*/81/*__*/);
    }

    [Fact]
    public void Out_permet_de_renvoyer_plusieurs_valeurs()
    {
        // Java : Integer.parseInt lève une NumberFormatException
        // C#   : le pattern TryXxx renvoie un bool + la valeur via `out`
        var ok = int.TryParse("123", out var nombre);
        var ko = int.TryParse("cent", out var rate);

        Check.That(ok).IsEqualTo(/*__*/true/*__*/);
        Check.That(nombre).IsEqualTo(/*__*/123/*__*/);
        Check.That(ko).IsEqualTo(/*__*/false/*__*/);
        Check.That(rate).IsEqualTo(/*__*/0/*__*/);
    }

    [Fact]
    public void Les_tuples_sont_natifs()
    {
        // Java : pas de tuples, on crée une petite classe dédiée ou on abuse de Map.Entry
        static (int Min, int Max) Bornes(int[] valeurs) => (valeurs.Min(), valeurs.Max());

        var bornes = Bornes([3, 1, 4, 1, 5, 9, 2, 6]);
        var (min, max) = bornes; // déconstruction

        Check.That(bornes.Max).IsEqualTo(/*__*/9/*__*/);
        Check.That(min).IsEqualTo(/*__*/1/*__*/);
        Check.That(min + max - 2).IsEqualTo(/*__*/8/*__*/);
    }

    [Fact]
    public void Un_alias_donne_un_nom_a_un_type_existant()
    {
        // Java : pas d'alias de type (Kotlin a typealias).
        // C#   : using Nom = Type; en haut du fichier. Depuis C# 12, n'importe quel type : tuple, tableau...
        Position abri = (3, 7);
        Dents prix = 12;

        Check.That(abri.Colonne).IsEqualTo(/*__*/7/*__*/);

        // Un alias est un NOUVEAU NOM, pas un nouveau type : le compilateur le remplace par int
        int memePrix = prix;
        Check.That(typeof(Dents) == typeof(int)).IsEqualTo(/*__*/true/*__*/);
        Check.That(memePrix + prix).IsEqualTo(/*__*/24/*__*/);
    }

    [Fact]
    public void L_echange_de_variables_tient_en_une_ligne()
    {
        var a = "zombie";
        var b = "humain";

        (a, b) = (b, a);

        Check.That(a).IsEqualTo(/*__*/"humain"/*__*/);
    }

    [Fact]
    public void Les_tableaux_se_creent_avec_des_collection_expressions()
    {
        // Java : int[] nombres = {1, 2, 3};
        int[] nombres = [1, 2, 3];
        int[] encorePlus = [0, .. nombres, 4]; // spread, comme le ... de JavaScript

        Check.That(encorePlus.Length).IsEqualTo(/*__*/5/*__*/);
        Check.That(encorePlus[1]).IsEqualTo(/*__*/1/*__*/);
        Check.That(encorePlus[^1]).IsEqualTo(/*__*/4/*__*/); // ^1 = le dernier
    }
}
