using System.Text;

namespace Fundamentals;

// 🎯 Objectif : les strings C# ressemblent à celles de Java... sauf quand elles ne leur ressemblent pas.
public class AProposDesChaines
{
    [Fact]
    public void Double_egal_compare_le_contenu_des_chaines()
    {
        // ⚠️ Java : == compare les RÉFÉRENCES, il faut utiliser .equals()
        // C#     : == est surchargé sur string et compare les VALEURS
        var a = "cerveau";
        var b = new string(['c', 'e', 'r', 'v', 'e', 'a', 'u']);

        Check.That(a == b).IsEqualTo(___);
        Check.That(ReferenceEquals(a, b)).IsEqualTo(___);
    }

    [Fact]
    public void Les_chaines_sont_immuables()
    {
        var cri = "braaains";
        var plusFort = cri.ToUpper();

        Check.That(cri).IsEqualTo(___);
        Check.That(plusFort).IsEqualTo(___);
    }

    [Fact]
    public void String_minuscule_et_String_majuscule_sont_le_meme_type()
    {
        // string est un alias de System.String, comme int pour Int32
        Check.That(typeof(string) == typeof(String)).IsEqualTo(___);
        Check.That(string.IsNullOrWhiteSpace("   ")).IsEqualTo(___);
    }

    [Fact]
    public void Les_index_et_ranges_remplacent_substring()
    {
        // Java : s.substring(0, 3), s.charAt(s.length() - 1)
        var mot = "apocalypse";

        Check.That(mot[..3]).IsEqualTo(___);
        Check.That(mot[5..]).IsEqualTo(___);
        Check.That(mot[^1]).IsEqualTo(___);
        Check.That(mot[3..6]).IsEqualTo(___);
    }

    [Fact]
    public void Les_chaines_verbatim_ignorent_les_echappements()
    {
        // Pratique pour les chemins Windows et les regex
        var chemin = @"C:\zombies\horde.txt";

        Check.That(chemin.Length).IsEqualTo(___);
        Check.That(chemin).IsEqualTo(___);
    }

    [Fact]
    public void Split_et_Join_fonctionnent_comme_en_Java()
    {
        var horde = "Bob,Alice,Charlie";

        var noms = horde.Split(',');
        var reformate = string.Join(" & ", noms);

        Check.That(noms.Length).IsEqualTo(___);
        Check.That(reformate).IsEqualTo(___);
    }

    [Fact]
    public void StringBuilder_pour_construire_en_boucle()
    {
        // Même classe qu'en Java (ou presque), dans System.Text
        var sb = new StringBuilder();
        for (var i = 0; i < 3; i++)
            sb.Append("bra");
        sb.Append("ins");

        Check.That(sb.ToString()).IsEqualTo(___);
    }

    [Fact]
    public void Les_char_sont_des_nombres_deguises()
    {
        var lettre = 'A';

        Check.That(lettre + 1).IsEqualTo(___);
        Check.That((char)(lettre + 1)).IsEqualTo(___);
        Check.That(char.IsDigit('7')).IsEqualTo(___);
    }

    [Fact]
    public void Les_comparaisons_peuvent_ignorer_la_casse()
    {
        // Java : equalsIgnoreCase
        var egal = string.Equals("ZOMBIE", "zombie", StringComparison.OrdinalIgnoreCase);

        Check.That(egal).IsEqualTo(___);
        Check.That("Zombieland".StartsWith("zombie", StringComparison.OrdinalIgnoreCase)).IsEqualTo(___);
    }

    // 🛠 À toi de coder : renvoie le mot à l'envers ("zombie" -> "eibmoz")
    private static string Inverser(string mot)
    {
        throw new NotImplementedException("🧟 À toi de jouer !");
    }

    [Fact]
    public void On_sait_inverser_une_chaine()
    {
        Check.That(Inverser("zombie")).IsEqualTo("eibmoz");
        Check.That(Inverser("")).IsEqualTo("");
    }

    // 🛠 À toi de coder : un palindrome se lit pareil dans les deux sens, sans tenir compte de la casse
    private static bool EstUnPalindrome(string mot)
    {
        throw new NotImplementedException("🧟 À toi de jouer !");
    }

    [Fact]
    public void On_sait_detecter_un_palindrome()
    {
        Check.That(EstUnPalindrome("Kayak")).IsTrue();
        Check.That(EstUnPalindrome("Zombie")).IsFalse();
    }
}
