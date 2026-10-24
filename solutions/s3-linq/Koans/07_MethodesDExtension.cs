namespace Fonctionnel;

// 🎯 Objectif : comprendre que LINQ n'est qu'une bibliothèque de méthodes d'extension... et écrire les siennes.

// Des opérateurs « métier » sur la horde : une méthode d'extension sur IEnumerable<Zombie>
public static class OperateursDeHorde
{
    public static IEnumerable<Zombie> Affames(this IEnumerable<Zombie> horde, int seuil = 5) =>
        horde.Where(z => z.Faim > seuil);

    // 🛠 À toi de coder : les zombies d'un quartier donné.
    public static IEnumerable<Zombie> DansLeQuartier(this IEnumerable<Zombie> horde, string quartier)
    {
        // >>> SOLUTION
        return horde.Where(z => z.Quartier == quartier);
        // <<< SOLUTION
    }

    // C# 14 : des propriétés d'extension
    extension(Zombie zombie)
    {
        public bool EstDangereux => zombie.Faim >= 8 || zombie.Type == TypeDeZombie.Tank;
    }

    extension(IEnumerable<Zombie> horde)
    {
        public int FaimTotale => horde.Sum(z => z.Faim);
    }
}

// 🛠 À toi de coder : réécris Where et Select toi-même, avec yield return.
//    Ils doivent rester paresseux : ils doivent fonctionner sur une séquence infinie.
public static class MonLinq
{
    public static IEnumerable<T> Filtrer<T>(this IEnumerable<T> source, Func<T, bool> predicat)
    {
        // >>> SOLUTION
        foreach (var element in source)
        {
            if (predicat(element))
                yield return element;
        }
        // <<< SOLUTION
    }

    public static IEnumerable<TResultat> Projeter<T, TResultat>(this IEnumerable<T> source, Func<T, TResultat> projection)
    {
        // >>> SOLUTION
        foreach (var element in source)
            yield return projection(element);
        // <<< SOLUTION
    }
}

public class AProposDesMethodesDExtension
{
    [Fact]
    public void LINQ_n_est_qu_une_collection_de_methodes_d_extension()
    {
        // horde.Where(...) n'est que du sucre pour un appel static à Enumerable.Where(horde, ...)
        var avecLeSucre = Registre.Zombies.Where(z => z.Faim > 8).Count();
        var sansLeSucre = Enumerable.Count(Enumerable.Where(Registre.Zombies, z => z.Faim > 8));

        Check.That(avecLeSucre).IsEqualTo(/*__*/3/*__*/);
        Check.That(sansLeSucre == avecLeSucre).IsEqualTo(/*__*/true/*__*/);
    }

    [Fact]
    public void Ses_propres_operateurs_se_chainent_comme_ceux_de_LINQ()
    {
        // Java : impossible d'ajouter une méthode à Stream ; il faut un Collector ou une méthode static
        var nombre = Registre.Zombies.Affames().Count();
        var tresAffames = Registre.Zombies.Affames(seuil: 8).Select(z => z.Nom);

        Check.That(nombre).IsEqualTo(/*__*/6/*__*/);
        Check.That(string.Join(", ", tresAffames)).IsEqualTo(/*__*/"Alice, Big Mama, Nina"/*__*/);
    }

    [Fact]
    public void Une_propriete_d_extension_s_utilise_comme_une_propriete()
    {
        var bob = Registre.Zombies[0];       // faim 7, rôdeur
        var bigMama = Registre.Zombies[3];   // faim 10, tank

        Check.That(bob.EstDangereux).IsEqualTo(/*__*/false/*__*/);
        Check.That(bigMama.EstDangereux).IsEqualTo(/*__*/true/*__*/);
        Check.That(Registre.Zombies.FaimTotale).IsEqualTo(/*__*/59/*__*/);
    }

    [Fact]
    public void On_sait_ecrire_un_operateur_metier()
    {
        var duPort = Registre.Zombies.DansLeQuartier("Port").Select(z => z.Nom);

        Check.That(duPort).ContainsExactly("Alice", "Gégé", "Oscar");
        Check.That(Registre.Zombies.DansLeQuartier("Port").Affames().Count()).IsEqualTo(2);
    }

    [Fact]
    public void On_sait_reecrire_Where_et_Select()
    {
        var noms = Registre.Zombies.Filtrer(z => z.Faim > 8).Projeter(z => z.Nom);

        Check.That(noms).ContainsExactly("Alice", "Big Mama", "Nina");
    }

    [Fact]
    public void Nos_operateurs_sont_paresseux()
    {
        var lus = 0;
        IEnumerable<int> UnMillionDeZombies()
        {
            for (var n = 0; n < 1_000_000; n++)
            {
                lus++;
                yield return n;
            }
        }

        var troisPairs = UnMillionDeZombies().Filtrer(n => n % 2 == 0).Projeter(n => n * 10).Take(3).ToList();

        Check.That(troisPairs).ContainsExactly(0, 20, 40);
        // un opérateur paresseux s'arrête dès que Take(3) est servi : il n'a lu que 0, 1, 2, 3, 4
        Check.That(lus).IsEqualTo(5);
    }
}
