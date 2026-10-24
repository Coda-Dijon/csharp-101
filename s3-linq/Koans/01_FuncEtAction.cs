namespace Fonctionnel;

// 🎯 Objectif : manipuler des fonctions comme des valeurs. C'est la base de tout LINQ.
//
//   Java                                  C#
//   Function<T, R>  /  BiFunction<...>     Func<T, TResult>  /  Func<T1, T2, TResult>
//   Supplier<T>                           Func<T>
//   Consumer<T>  /  Runnable              Action<T>  /  Action
//   Predicate<T>                          Predicate<T>, ou plus souvent Func<T, bool>
//   IntFunction, ToIntFunction...         inutile : les generics sont réifiés, Func<int, int> suffit
public class AProposDeFuncEtAction
{
    [Fact]
    public void Func_est_une_fonction_qui_renvoie_une_valeur()
    {
        // Java : Function<Integer, Integer> doubler = x -> x * 2;  doubler.apply(21)
        Func<int, int> doubler = x => x * 2;
        // le DERNIER paramètre de type est toujours le type de retour
        Func<int, int, int> additionner = (a, b) => a + b;

        Check.That(doubler(21)).IsEqualTo(___); // pas de .apply() : on appelle la variable directement
        Check.That(additionner(3, 4)).IsEqualTo(___);
    }

    [Fact]
    public void Action_est_une_fonction_qui_ne_renvoie_rien()
    {
        // Java : Consumer<String> crier = s -> journal.add(...);
        var journal = new List<string>();
        Action<string> crier = mot => journal.Add(mot.ToUpper());

        crier("braaains");

        Check.That(journal[0]).IsEqualTo(___);
    }

    [Fact]
    public void Sans_parametre_on_obtient_l_equivalent_de_Supplier_et_Runnable()
    {
        var alarmes = 0;
        Func<string> cri = () => "Braaains";  // Java : Supplier<String>
        Action sonner = () => alarmes++;      // Java : Runnable

        sonner();
        sonner();

        Check.That(cri()).IsEqualTo(___);
        Check.That(alarmes).IsEqualTo(___);
    }

    [Fact]
    public void Un_predicat_est_une_fonction_qui_renvoie_un_bool()
    {
        // Predicate<T> existe (List.FindAll l'utilise), mais LINQ préfère Func<T, bool>
        Predicate<int> estAffame = faim => faim > 5;
        Func<int, bool> estRepu = faim => faim <= 5;
        List<int> faims = [3, 8, 9];

        Check.That(faims.FindAll(estAffame).Count).IsEqualTo(___);
        Check.That(faims.Count(estRepu)).IsEqualTo(___);
    }

    private static string Decrire(Zombie zombie) => $"{zombie.Nom} ({zombie.Faim}/10)";

    [Fact]
    public void Une_methode_existante_se_passe_par_son_nom()
    {
        // Java : horde.stream().map(Recensement::decrire)  /  C# : un « method group », sans :: ni ()
        Func<Zombie, string> decrire = Decrire;

        var descriptions = Registre.Zombies.Take(2).Select(Decrire).ToList();

        Check.That(decrire(Registre.Zombies[0])).IsEqualTo(___);
        Check.That(descriptions[1]).IsEqualTo(___);
    }

    [Fact]
    public void Une_lambda_capture_les_variables_qui_l_entourent()
    {
        // Java : la variable capturée doit être « effectivement final »
        // C#   : on capture la variable elle-même, et on peut la modifier (une « closure »)
        var abattus = 0;
        Action abattre = () => abattus++;

        abattre();
        abattre();
        abattre();

        Check.That(abattus).IsEqualTo(___);
    }

    [Fact]
    public void Attention_une_boucle_for_partage_sa_variable_avec_les_lambdas()
    {
        var appels = new List<Func<int>>();
        for (var i = 0; i < 3; i++)
            appels.Add(() => i); // toutes les lambdas capturent LA MÊME variable i

        var copies = new List<Func<int>>();
        foreach (var n in new[] { 0, 1, 2 })
            copies.Add(() => n); // foreach crée une nouvelle variable à chaque tour (depuis C# 5)

        Check.That(appels[0]()).IsEqualTo(___); // i vaut 3 quand la boucle se termine
        Check.That(copies[0]()).IsEqualTo(___);
    }

    [Fact]
    public void Une_fonction_peut_renvoyer_une_fonction()
    {
        // une « fabrique de prédicats » : chaque appel fabrique une nouvelle fonction
        static Func<Zombie, bool> PlusAffameQue(int seuil) => zombie => zombie.Faim > seuil;

        var tresAffame = PlusAffameQue(8);

        Check.That(tresAffame(Registre.Zombies[1])).IsEqualTo(___); // Alice : 9
        Check.That(Registre.Zombies.Count(PlusAffameQue(5))).IsEqualTo(___);
    }

    // 🛠 À toi de coder : renvoie une fonction qui applique `f` le nombre de fois demandé.
    //    Repeter(x => x + 1, 3)(0) == 3 ; avec 0 fois, la valeur ressort telle quelle.
    private static Func<int, int> Repeter(Func<int, int> f, int fois)
    {
        throw new NotImplementedException("🧟 À toi de jouer !");
    }

    [Fact]
    public void On_sait_fabriquer_une_fonction()
    {
        Check.That(Repeter(x => x + 1, 3)(0)).IsEqualTo(3);
        Check.That(Repeter(x => x * 2, 10)(1)).IsEqualTo(1024);
        Check.That(Repeter(x => x * 2, 0)(5)).IsEqualTo(5);
    }

    // 🛠 À toi de coder : renvoie le prédicat contraire (Java : predicate.negate()).
    private static Func<T, bool> Non<T>(Func<T, bool> predicat)
    {
        throw new NotImplementedException("🧟 À toi de jouer !");
    }

    [Fact]
    public void On_sait_transformer_une_fonction()
    {
        Func<Zombie, bool> estDuPort = zombie => zombie.Quartier == "Port";

        Check.That(Registre.Zombies.Count(estDuPort)).IsEqualTo(3);
        Check.That(Registre.Zombies.Count(Non(estDuPort))).IsEqualTo(6);
    }
}
