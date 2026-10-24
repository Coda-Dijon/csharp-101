using System.Numerics;

namespace Types;

// 🎯 Objectif : des generics qui existent vraiment à l'exécution (pas de type erasure !).
public class AProposDesGeneriques
{
    private class Caisse<T>(T contenu)
    {
        public T Ouvrir() => contenu;
    }

    [Fact]
    public void Une_classe_generique_s_ecrit_comme_en_Java()
    {
        var caisse = new Caisse<string>("conserve");
        var munitions = new Caisse<int>(12); // Java : Caisse<Integer>, avec boxing ; C# : un vrai int

        Check.That(caisse.Ouvrir()).IsEqualTo(/*__*/"conserve"/*__*/);
        Check.That(munitions.Ouvrir() + 1).IsEqualTo(/*__*/13/*__*/);
    }

    [Fact]
    public void Le_type_d_une_methode_generique_est_infere()
    {
        static T Premier<T>(T[] valeurs) => valeurs[0];

        Check.That(Premier([4, 8, 15])).IsEqualTo(/*__*/4/*__*/);
        Check.That(Premier(["Rick", "Morty"])).IsEqualTo(/*__*/"Rick"/*__*/);
    }

    [Fact]
    public void Les_generics_sont_reifies()
    {
        // Java : new ArrayList<Integer>().getClass() == new ArrayList<String>().getClass()  -> true (erasure)
        Check.That(new List<int>().GetType() == new List<string>().GetType()).IsEqualTo(/*__*/false/*__*/);
        Check.That(new List<int>().GetType().GenericTypeArguments[0]).IsEqualTo(/*__*/typeof(int)/*__*/);
    }

    [Fact]
    public void On_peut_utiliser_T_a_l_execution()
    {
        // Impossible en Java : T est effacé, il faut passer un Class<T> en paramètre
        static string NomDuType<T>() => typeof(T).Name;

        Check.That(NomDuType<int>()).IsEqualTo(/*__*/"Int32"/*__*/);
        Check.That(NomDuType<List<string>>()).IsEqualTo(/*__*/"List`1"/*__*/); // `1 = un paramètre générique
    }

    [Fact]
    public void Default_donne_la_valeur_par_defaut_de_T()
    {
        static T? ParDefaut<T>() => default;

        Check.That(ParDefaut<int>()).IsEqualTo(/*__*/0/*__*/);
        Check.That(ParDefaut<bool>()).IsEqualTo(/*__*/false/*__*/);
        Check.That(ParDefaut<string>() is null).IsEqualTo(/*__*/true/*__*/);
    }

    private class Abri
    {
        public string Nom { get; set; } = "cabane";
    }

    [Fact]
    public void Les_contraintes_where_restreignent_T()
    {
        // Java : <T extends Comparable<T>>  /  C# : where T : IComparable<T>
        static T LePlusGrand<T>(T a, T b) where T : IComparable<T> => a.CompareTo(b) >= 0 ? a : b;

        // where T : new() -> T a un constructeur sans paramètre : on peut écrire new T() !
        static T Fabriquer<T>() where T : new() => new T();

        Check.That(LePlusGrand(3, 7)).IsEqualTo(/*__*/7/*__*/);
        Check.That(LePlusGrand("bob", "alice")).IsEqualTo(/*__*/"bob"/*__*/);
        Check.That(Fabriquer<Abri>().Nom).IsEqualTo(/*__*/"cabane"/*__*/);
    }

    [Fact]
    public void La_variance_se_declare_sur_l_interface()
    {
        // Java : List<? extends Object> à chaque utilisation (use-site variance)
        // C#   : IEnumerable<out T> une fois pour toutes (declaration-site variance)
        IEnumerable<string> noms = ["Rick", "Morty"];
        IEnumerable<object> objets = noms; // OK : IEnumerable ne fait que produire des T

        Check.That(objets.First()).IsEqualTo(/*__*/"Rick"/*__*/);
        // List<T> consomme ET produit des T : elle est invariante
        Check.That(typeof(List<object>).IsAssignableFrom(typeof(List<string>))).IsEqualTo(/*__*/false/*__*/);
    }

    private class Creature
    {
        public string Nom { get; init; } = "";
    }

    private class Zombie : Creature;

    private class Humain : Creature;

    [Fact]
    public void La_contravariance_accepte_un_consommateur_plus_general()
    {
        // Java : Consumer<? super Zombie>  /  C# : Action<in T>, déclaré une fois pour toutes
        var abattus = new List<string>();
        Action<Creature> abattre = creature => abattus.Add(creature.Nom);

        // qui sait abattre n'importe quelle créature sait abattre un zombie
        Action<Zombie> abattreUnZombie = abattre;
        abattreUnZombie(new Zombie { Nom = "Bob" });

        Check.That(abattus[0]).IsEqualTo(/*__*/"Bob"/*__*/);
        // l'inverse ne compile pas : Action<Creature> a = abattreUnZombie;
        Check.That(typeof(Action<Creature>).IsAssignableFrom(typeof(Action<Zombie>))).IsEqualTo(/*__*/false/*__*/);
    }

    [Fact]
    public void Les_tableaux_sont_covariants_et_c_est_un_piege()
    {
        // Héritage de Java 1.0, conservé par C# : un tableau de zombies passe pour un tableau de créatures...
        Creature[] creatures = new Zombie[1];

        // ... ça compile, mais le runtime refuse d'y ranger un humain (ArrayStoreException en Java)
        Check.ThatCode(() => creatures[0] = new Humain()).ThrowsType(/*__*/typeof(ArrayTypeMismatchException)/*__*/);
    }

    [Fact]
    public void Le_math_generique_fonctionne_pour_tous_les_nombres()
    {
        // C# 11 : INumber<T> expose +, Zero... sous forme de membres static d'interface. Impossible en Java.
        static T Somme<T>(T[] valeurs) where T : INumber<T>
        {
            var total = T.Zero;
            foreach (var v in valeurs)
                total += v;
            return total;
        }

        Check.That(Somme([1, 2, 3])).IsEqualTo(/*__*/6/*__*/);
        Check.That(Somme([0.5, 0.25])).IsEqualTo(/*__*/0.75/*__*/);
        Check.That(Somme([1.5m, 2.5m])).IsEqualTo(/*__*/4.0m/*__*/);
    }

    // 🛠 À toi de coder : renvoie le plus petit et le plus grand élément d'un tableau non vide,
    //    pour n'importe quel type comparable (indice : CompareTo renvoie < 0, 0 ou > 0).
    private static (T Min, T Max) Bornes<T>(T[] valeurs) where T : IComparable<T>
    {
        // >>> SOLUTION
        var (min, max) = (valeurs[0], valeurs[0]);
        foreach (var v in valeurs)
        {
            if (v.CompareTo(min) < 0) min = v;
            if (v.CompareTo(max) > 0) max = v;
        }
        return (min, max);
        // <<< SOLUTION
    }

    [Fact]
    public void On_sait_ecrire_une_methode_generique()
    {
        Check.That(Bornes([3, 1, 4, 1, 5, 9, 2, 6])).IsEqualTo((1, 9));
        Check.That(Bornes(["Morty", "Rick", "Carl", "Negan"])).IsEqualTo(("Carl", "Rick"));
        Check.That(Bornes([DateOnly.Parse("2026-10-31"), DateOnly.Parse("2026-01-01")]).Min.Month).IsEqualTo(1);
    }
}
