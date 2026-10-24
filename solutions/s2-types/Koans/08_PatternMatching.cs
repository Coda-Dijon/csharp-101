namespace Types;

// 🎯 Objectif : interroger la forme des données plutôt qu'écrire des cascades de if/instanceof.
public class AProposDuPatternMatching
{
    private abstract record Creature(string Nom);

    private record Humain(string Nom, int PointsDeVie) : Creature(Nom);

    private record Zombie(string Nom, int Faim) : Creature(Nom);

    private record Chien(string Nom, bool Contamine) : Creature(Nom);

    [Fact]
    public void Le_type_pattern_teste_et_capture_en_une_fois()
    {
        // Java 16 : if (o instanceof Integer n && n > 40)
        object trouvaille = 42;

        Check.That(trouvaille is int n && n > 40).IsEqualTo(/*__*/true/*__*/);
        Check.That(trouvaille is string).IsEqualTo(/*__*/false/*__*/);
    }

    [Fact]
    public void Les_patterns_relationnels_et_logiques_se_combinent()
    {
        // and, or, not : des mots-clés, pas && || !
        static bool EstUneVoyelle(char c) => c is 'a' or 'e' or 'i' or 'o' or 'u' or 'y';
        static bool EstUnScore(int n) => n is >= 0 and <= 10;

        Check.That(EstUneVoyelle('o')).IsEqualTo(/*__*/true/*__*/);
        Check.That(EstUnScore(11)).IsEqualTo(/*__*/false/*__*/);
        Check.That("Rick" is not null).IsEqualTo(/*__*/true/*__*/);
    }

    [Fact]
    public void Les_property_patterns_inspectent_les_proprietes()
    {
        Creature rick = new Humain("Rick", 0);

        Check.That(rick is Humain { PointsDeVie: > 0 }).IsEqualTo(/*__*/false/*__*/);
        // les patterns s'imbriquent : ici sur la propriété Length de Nom
        Check.That(rick is { Nom.Length: 4 }).IsEqualTo(/*__*/true/*__*/);
    }

    [Fact]
    public void Le_switch_sur_les_types_remplace_le_pattern_visitor()
    {
        // Java 21 : switch (c) { case Humain h -> ...; case Zombie z when z.faim() > 5 -> ...; }
        static string Decrire(Creature creature) => creature switch
        {
            Humain h => $"{h.Nom} est un humain",
            Zombie { Faim: > 5 } => "un zombie affamé",
            Zombie => "un zombie repu",
            _ => "autre chose", // ⚠️ obligatoire : le compilateur ne sait pas que la hiérarchie est fermée
        };

        Check.That(Decrire(new Humain("Rick", 100))).IsEqualTo(/*__*/"Rick est un humain"/*__*/);
        Check.That(Decrire(new Zombie("Bob", 9))).IsEqualTo(/*__*/"un zombie affamé"/*__*/);
        Check.That(Decrire(new Zombie("Bob", 2))).IsEqualTo(/*__*/"un zombie repu"/*__*/);
        Check.That(Decrire(new Chien("Rex", true))).IsEqualTo(/*__*/"autre chose"/*__*/);
    }

    [Fact]
    public void Les_patterns_positionnels_utilisent_la_deconstruction()
    {
        // Java 21 : case Zombie(var nom, var faim) when faim == 0 -> ...
        static string Activite(Zombie zombie) => zombie switch
        {
            (var nom, 0) => $"{nom} dort",
            (var nom, var faim) when faim > 8 => $"{nom} chasse",
            (var nom, _) => $"{nom} erre",
        };

        Check.That(Activite(new Zombie("Bob", 0))).IsEqualTo(/*__*/"Bob dort"/*__*/);
        Check.That(Activite(new Zombie("Bob", 9))).IsEqualTo(/*__*/"Bob chasse"/*__*/);
        Check.That(Activite(new Zombie("Bob", 5))).IsEqualTo(/*__*/"Bob erre"/*__*/);
    }

    [Fact]
    public void Les_list_patterns_decrivent_la_forme_d_une_sequence()
    {
        // Pas d'équivalent en Java
        static string Resumer(int[] scores) => scores switch
        {
            [] => "aucun score",
            [var seul] => $"un seul : {seul}",
            [var premier, .., var dernier] => $"de {premier} à {dernier}",
        };

        Check.That(Resumer([])).IsEqualTo(/*__*/"aucun score"/*__*/);
        Check.That(Resumer([7])).IsEqualTo(/*__*/"un seul : 7"/*__*/);
        Check.That(Resumer([1, 2, 3, 4])).IsEqualTo(/*__*/"de 1 à 4"/*__*/);
        Check.That(new[] { 1, 2, 3 } is [1, ..]).IsEqualTo(/*__*/true/*__*/);
    }

    // 🛠 À toi de coder : le triage à l'entrée du bunker, avec un switch expression.
    //    - un humain avec plus de 50 points de vie -> "entre"
    //    - un humain plus faible                   -> "infirmerie"
    //    - un chien non contaminé                  -> "entre"
    //    - tout le reste (zombies, chiens contaminés...) -> "dehors"
    private static string Trier(Creature creature)
    {
        // >>> SOLUTION
        return creature switch
        {
            Humain { PointsDeVie: > 50 } => "entre",
            Humain => "infirmerie",
            Chien { Contamine: false } => "entre",
            _ => "dehors",
        };
        // <<< SOLUTION
    }

    [Fact]
    public void On_sait_trier_avec_des_property_patterns()
    {
        Check.That(Trier(new Humain("Rick", 80))).IsEqualTo("entre");
        Check.That(Trier(new Humain("Morty", 12))).IsEqualTo("infirmerie");
        Check.That(Trier(new Chien("Rex", Contamine: false))).IsEqualTo("entre");
        Check.That(Trier(new Chien("Cujo", Contamine: true))).IsEqualTo("dehors");
        Check.That(Trier(new Zombie("Bob", 3))).IsEqualTo("dehors");
    }

    // 🛠 À toi de coder : annonce qui arrive au bunker, avec des list patterns.
    //    []                     -> "personne"
    //    ["Rick"]               -> "Rick, seul"
    //    ["Rick", "Morty"]      -> "Rick et Morty"
    //    ["Rick", "Morty", ...] -> "Rick et 3 autres" (indice : `.. var reste` capture le reste dans un tableau)
    private static string Annoncer(string[] arrivants)
    {
        // >>> SOLUTION
        return arrivants switch
        {
            [] => "personne",
            [var seul] => $"{seul}, seul",
            [var premier, var second] => $"{premier} et {second}",
            [var premier, .. var reste] => $"{premier} et {reste.Length} autres",
        };
        // <<< SOLUTION
    }

    [Fact]
    public void On_sait_decrire_une_liste_avec_des_list_patterns()
    {
        Check.That(Annoncer([])).IsEqualTo("personne");
        Check.That(Annoncer(["Rick"])).IsEqualTo("Rick, seul");
        Check.That(Annoncer(["Rick", "Morty"])).IsEqualTo("Rick et Morty");
        Check.That(Annoncer(["Rick", "Morty", "Carl", "Negan"])).IsEqualTo("Rick et 3 autres");
    }
}
