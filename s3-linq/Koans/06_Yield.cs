namespace Fonctionnel;

// 🎯 Objectif : fabriquer ses propres séquences paresseuses avec yield return.
//
// Java : implémenter Iterator<T> à la main (hasNext / next), ou Stream.iterate / Stream.generate
// C#   : une méthode qui renvoie IEnumerable<T> et qui contient `yield return` devient un itérateur :
//        le compilateur génère la machine à états, et chaque élément n'est produit qu'à la demande.
public class AProposDeYield
{
    private static IEnumerable<string> Alertes(List<string> journal)
    {
        journal.Add("début");
        yield return "zombie au nord";
        journal.Add("après le nord");
        yield return "zombie au sud";
        journal.Add("fin");
    }

    [Fact]
    public void Yield_return_rend_la_main_a_chaque_element()
    {
        var journal = new List<string>();

        var alertes = Alertes(journal);
        Check.That(journal.Count).IsEqualTo(___); // l'appel n'exécute rien du tout

        var premiere = alertes.First();

        Check.That(premiere).IsEqualTo(___);
        Check.That(string.Join(", ", journal)).IsEqualTo(___); // on s'est arrêté au premier yield
    }

    private static IEnumerable<string> HordeInfinie()
    {
        var numero = 1;
        while (true) // une boucle infinie... sans danger, tant que personne ne demande TOUS les éléments
            yield return $"Zombie n°{numero++}";
    }

    [Fact]
    public void Une_sequence_peut_etre_infinie()
    {
        // Java : Stream.iterate(1, n -> n + 1).limit(3)
        var troisPremiers = HordeInfinie().Take(3).ToList();

        Check.That(troisPremiers.Last()).IsEqualTo(___);
        Check.That(HordeInfinie().ElementAt(99)).IsEqualTo(___);
        // HordeInfinie().Count()  -> ne termine jamais !
    }

    [Fact]
    public void TakeWhile_s_arrete_au_premier_element_qui_ne_convient_pas()
    {
        static IEnumerable<int> FaimQuiMonte()
        {
            var faim = 1;
            while (true)
            {
                yield return faim;
                faim *= 2;
            }
        }

        Check.That(FaimQuiMonte().TakeWhile(f => f < 10).Sum()).IsEqualTo(___); // 1 + 2 + 4 + 8
    }

    [Fact]
    public void Yield_break_termine_la_sequence()
    {
        static IEnumerable<Zombie> JusquAuPremierTank(IEnumerable<Zombie> horde)
        {
            foreach (var zombie in horde)
            {
                if (zombie.Type == TypeDeZombie.Tank) yield break; // ≈ return, pour un itérateur
                yield return zombie;
            }
        }

        var avantLeTank = JusquAuPremierTank(Registre.Zombies).Select(z => z.Nom);

        Check.That(string.Join(", ", avantLeTank)).IsEqualTo(___);
    }

    // 🛠 À toi de coder : chaque vague de zombies compte autant que les deux précédentes réunies.
    //    Les deux premières vagues comptent 1 zombie : 1, 1, 2, 3, 5, 8... sans fin.
    private static IEnumerable<int> Vagues()
    {
        throw new NotImplementedException("🧟 À toi de jouer !");
    }

    [Fact]
    public void On_sait_ecrire_une_sequence_infinie()
    {
        Check.That(Vagues().Take(7)).ContainsExactly(1, 1, 2, 3, 5, 8, 13);
        Check.That(Vagues().First(v => v > 100)).IsEqualTo(144);
    }

    // 🛠 À toi de coder : les gardes se relaient à tour de rôle, indéfiniment.
    //    ["Rick", "Daryl"] -> Rick, Daryl, Rick, Daryl, Rick...  (une liste vide ne produit rien)
    private static IEnumerable<string> Relais(string[] gardes)
    {
        throw new NotImplementedException("🧟 À toi de jouer !");
    }

    [Fact]
    public void On_sait_faire_tourner_les_gardes()
    {
        Check.That(Relais(["Rick", "Daryl"]).Take(5)).ContainsExactly("Rick", "Daryl", "Rick", "Daryl", "Rick");
        Check.That(Relais([]).Take(5)).IsEmpty();
    }
}
