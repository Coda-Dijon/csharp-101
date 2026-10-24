namespace Fundamentals;

// 🎯 Objectif : apprivoiser null. Le compilateur C# vous aide... à condition de l'écouter.
// Le projet active <Nullable>enable</Nullable> : `string` ne devrait jamais être null, `string?` peut l'être.
public class AProposDeLaNullabilite
{
    [Fact]
    public void Le_point_d_interrogation_signale_qu_une_valeur_peut_etre_null()
    {
        // Java : @Nullable String (simple annotation)  /  C# : string? (vérifié par le compilateur)
        string? survivant = null;

        Check.That(survivant is null).IsEqualTo(___);
    }

    [Fact]
    public void L_operateur_null_conditionnel_court_circuite()
    {
        // Java : Optional.ofNullable(s).map(String::length).orElse(null)
        string? nom = null;

        int? longueur = nom?.Length;

        Check.That(longueur).IsEqualTo(___);
    }

    [Fact]
    public void L_operateur_double_point_d_interrogation_fournit_une_valeur_par_defaut()
    {
        // Java : Optional.ofNullable(nom).orElse("Inconnu")
        string? nom = null;

        Check.That(nom ?? "Inconnu").IsEqualTo(___);
        Check.That(nom?.Length ?? 0).IsEqualTo(___);
    }

    [Fact]
    public void L_affectation_conditionnelle_n_ecrase_que_null()
    {
        string? arme = null;
        arme ??= "batte de baseball";
        arme ??= "tronçonneuse";

        Check.That(arme).IsEqualTo(___);
    }

    [Fact]
    public void Les_types_valeur_nullables_sont_des_Nullable_de_T()
    {
        // int ne peut pas être null... int? (= Nullable<int>) le peut
        int? munitions = null;

        Check.That(munitions.HasValue).IsEqualTo(___);
        Check.That(munitions.GetValueOrDefault()).IsEqualTo(___);

        munitions = 12;
        Check.That(munitions.Value).IsEqualTo(___);
    }

    [Fact]
    public void Is_not_null_permet_de_tester_et_de_capturer()
    {
        object? trouvaille = "conserve de haricots";

        // pattern matching : teste le type ET déclare une variable typée
        var description = trouvaille is string texte ? $"un texte de {texte.Length} caractères" : "autre chose";

        Check.That(description).IsEqualTo(___);
    }

    [Fact]
    public void Le_compilateur_suit_les_verifications_de_null()
    {
        static int LongueurSure(string? s)
        {
            if (s is null) return -1;
            return s.Length; // pas de warning : le compilateur sait que s n'est plus null ici
        }

        Check.That(LongueurSure(null)).IsEqualTo(___);
        Check.That(LongueurSure("Rick")).IsEqualTo(___);
    }

    // 🛠 À toi de coder : renvoie le nom du chef du groupe en majuscules, ou "PERSONNE" s'il n'y a pas de chef
    //    (le groupe lui-même peut être null !). Une seule ligne avec ?. et ?? suffit.
    private class Survivant(string nom)
    {
        public string Nom { get; } = nom;
    }

    private class Groupe(Survivant? chef)
    {
        public Survivant? Chef { get; } = chef;
    }

    private static string NomDuChef(Groupe? groupe)
    {
        throw new NotImplementedException("🧟 À toi de jouer !");
    }

    [Fact]
    public void On_sait_naviguer_dans_des_donnees_peut_etre_nulles()
    {
        Check.That(NomDuChef(new Groupe(new Survivant("Rick")))).IsEqualTo("RICK");
        Check.That(NomDuChef(new Groupe(null))).IsEqualTo("PERSONNE");
        Check.That(NomDuChef(null)).IsEqualTo("PERSONNE");
    }
}
