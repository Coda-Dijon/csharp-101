namespace Fonctionnel;

// 🎯 Objectif : les collections de tous les jours. Mêmes idées qu'en Java, quelques différences qui piquent.
//
//   Java                     C#
//   ArrayList<T>             List<T>
//   HashMap<K, V>            Dictionary<TKey, TValue>
//   HashSet<T>               HashSet<T>
//   ArrayDeque<T> (file)     Queue<T>
//   List.of(...)             IReadOnlyList<T>, ImmutableList<T>, ou une collection expression [...]
public class AProposDesCollections
{
    [Fact]
    public void Une_List_ressemble_a_une_ArrayList()
    {
        List<string> horde = ["Bob", "Alice"];

        horde.Add("Charlie");       // Java : add
        horde.Insert(0, "Big Mama"); // Java : add(0, ...)
        horde.Remove("Alice");

        Check.That(horde.Count).IsEqualTo(___); // Java : size()
        Check.That(horde[0]).IsEqualTo(___); // Java : get(0)
        Check.That(horde.Contains("Alice")).IsEqualTo(___);
    }

    [Fact]
    public void Un_Dictionary_se_lit_et_s_ecrit_avec_des_crochets()
    {
        // Java : map.put("Bob", 7) / map.get("Bob")
        var faimParNom = new Dictionary<string, int>
        {
            ["Bob"] = 7,
            ["Alice"] = 9,
        };

        faimParNom["Bob"] = 8;      // remplace la valeur
        faimParNom["Charlie"] = 3;  // ajoute la clé

        Check.That(faimParNom["Bob"]).IsEqualTo(___);
        Check.That(faimParNom.Count).IsEqualTo(___);
    }

    [Fact]
    public void Une_cle_absente_leve_une_exception()
    {
        // ⚠️ Java : map.get("Zorg") renvoie null  /  C# : l'indexeur lève une exception
        var faimParNom = new Dictionary<string, int> { ["Bob"] = 7 };

        Check.ThatCode(() => faimParNom["Zorg"]).ThrowsType(___);
    }

    [Fact]
    public void TryGetValue_et_GetValueOrDefault_evitent_l_exception()
    {
        var faimParNom = new Dictionary<string, int> { ["Bob"] = 7 };

        // le pattern TryXxx de la séance 2, avec un paramètre out
        var trouve = faimParNom.TryGetValue("Bob", out var faim);

        Check.That(trouve).IsEqualTo(___);
        Check.That(faim).IsEqualTo(___);
        // Java : getOrDefault("Zorg", -1)
        Check.That(faimParNom.GetValueOrDefault("Zorg", -1)).IsEqualTo(___);
    }

    [Fact]
    public void Un_Dictionary_se_parcourt_par_paires_cle_valeur()
    {
        var faimParNom = new Dictionary<string, int> { ["Bob"] = 7, ["Alice"] = 9 };
        var total = 0;

        // Java : for (var entry : map.entrySet())  /  C# : on déconstruit chaque KeyValuePair
        foreach (var (nom, faim) in faimParNom)
            total += faim;

        Check.That(total).IsEqualTo(___);
    }

    [Fact]
    public void Un_HashSet_ignore_les_doublons()
    {
        var quartiersContamines = new HashSet<string> { "Port", "Gare" };

        var ajoute = quartiersContamines.Add("Port"); // Add renvoie false si l'élément est déjà là

        Check.That(ajoute).IsEqualTo(___);
        Check.That(quartiersContamines.Count).IsEqualTo(___);
    }

    [Fact]
    public void Un_HashSet_sait_faire_des_operations_ensemblistes()
    {
        var hier = new HashSet<string> { "Port", "Gare" };
        string[] aujourdhui = ["Gare", "Centre-ville"];

        // Java : retainAll / addAll, qui modifient aussi l'ensemble
        hier.IntersectWith(aujourdhui);

        Check.That(hier.Single()).IsEqualTo(___); // contaminé deux jours de suite
    }

    [Fact]
    public void Une_Queue_sert_de_file_d_attente()
    {
        // Java : ArrayDeque avec offer / poll
        var fileDuBunker = new Queue<string>(["Rick", "Carl"]);
        fileDuBunker.Enqueue("Daryl");

        var premier = fileDuBunker.Dequeue();

        Check.That(premier).IsEqualTo(___);
        Check.That(fileDuBunker.Peek()).IsEqualTo(___);
    }

    // 🛠 À toi de coder, À LA MAIN (sans LINQ) : regroupe les noms des zombies par quartier.
    //    Indice : TryGetValue, et une nouvelle liste quand le quartier n'existe pas encore.
    //    (Dans 04_LeRecensement, GroupBy fera la même chose en une ligne.)
    private static Dictionary<string, List<string>> RegrouperParQuartier(IEnumerable<Zombie> horde)
    {
        throw new NotImplementedException("🧟 À toi de jouer !");
    }

    [Fact]
    public void On_sait_regrouper_avec_un_Dictionary()
    {
        var parQuartier = RegrouperParQuartier(Registre.Zombies);

        Check.That(parQuartier.Keys).ContainsExactly("Centre-ville", "Port", "Gare");
        Check.That(parQuartier["Port"]).ContainsExactly("Alice", "Gégé", "Oscar");
        Check.That(parQuartier["Gare"]).ContainsExactly("Big Mama", "Marcel");
    }
}
