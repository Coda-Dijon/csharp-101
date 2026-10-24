namespace Fonctionnel;

// 🎯 Objectif : comprendre QUAND une requête LINQ s'exécute. Spoiler : pas quand on l'écrit.
//
// Java : un Stream ne s'exécute qu'à l'opération terminale... et ne peut servir qu'une fois.
// C#   : une requête LINQ (IEnumerable<T>) ne s'exécute qu'au parcours... et se rejoue à CHAQUE parcours.
public class AProposDeLExecutionDifferee
{
    [Fact]
    public void Une_requete_ne_s_execute_pas_quand_on_l_ecrit()
    {
        var journal = new List<string>();

        var affames = Registre.Zombies.Where(z =>
        {
            journal.Add(z.Nom);
            return z.Faim > 8;
        });

        Check.That(journal.Count).IsEqualTo(___); // rien ne s'est passé : affames est une recette

        var resultat = affames.ToList(); // ToList, Count, First, foreach... déclenchent l'exécution

        Check.That(journal.Count).IsEqualTo(___);
        Check.That(resultat.Count).IsEqualTo(___);
    }

    [Fact]
    public void Chaque_parcours_rejoue_la_requete()
    {
        var parcours = 0;
        var affames = Registre.Zombies.Where(z =>
        {
            if (z == Registre.Zombies[0]) parcours++; // compte les passages sur le premier zombie
            return z.Faim > 8;
        });

        _ = affames.Count();
        _ = affames.Any();
        foreach (var _ in affames) { }

        // Java : réutiliser un Stream lève IllegalStateException ; C# : ça marche, mais ça recalcule tout
        Check.That(parcours).IsEqualTo(___);
    }

    [Fact]
    public void La_requete_voit_les_changements_de_la_source()
    {
        List<string> horde = ["Bob", "Alice"];
        var commencentParB = horde.Where(nom => nom.StartsWith('B'));

        horde.Add("Big Mama"); // ajouté APRÈS l'écriture de la requête

        Check.That(commencentParB.Count()).IsEqualTo(___);
    }

    [Fact]
    public void Les_elements_traversent_la_chaine_un_par_un()
    {
        // Comme les Streams Java : chaque élément passe par Where PUIS Select avant le suivant
        var journal = new List<string>();
        int[] faims = [3, 9, 7];

        var resultat = faims
            .Where(f => { journal.Add($"W{f}"); return f > 5; })
            .Select(f => { journal.Add($"S{f}"); return f * 2; })
            .ToList();

        Check.That(string.Join(" ", journal)).IsEqualTo(___);
    }

    [Fact]
    public void First_s_arrete_des_qu_il_a_trouve()
    {
        var examines = 0;

        var premierAffame = Registre.Zombies.First(z =>
        {
            examines++;
            return z.Faim > 8;
        });

        Check.That(premierAffame.Nom).IsEqualTo(___);
        Check.That(examines).IsEqualTo(___); // Bob, puis Alice : le reste n'est jamais lu
    }

    // 🛠 À toi de coder : un scanner de zone coûteux (il compte ses passages) renvoie la horde détectée.
    //    Calcule le nombre de zombies ET leur faim maximale en ne parcourant le scan qu'UNE seule fois.
    //    Indice : matérialiser le résultat avec ToList() avant de le réutiliser.
    private static (int Nombre, int FaimMax) Rapport(IEnumerable<Zombie> scan)
    {
        throw new NotImplementedException("🧟 À toi de jouer !");
    }

    private sealed class Scanner
    {
        public int Passages { get; private set; }

        public IEnumerable<Zombie> ScannerLaZone()
        {
            Passages++;
            foreach (var zombie in Registre.Zombies.Where(z => z.Quartier == "Port"))
                yield return zombie;
        }
    }

    [Fact]
    public void On_sait_eviter_la_double_enumeration()
    {
        var scanner = new Scanner();

        var rapport = Rapport(scanner.ScannerLaZone());

        Check.That(rapport).IsEqualTo((3, 9));
        Check.That(scanner.Passages).IsEqualTo(1);
    }
}
