---
theme: default
title: "C# 101 · Séance 3 · LINQ & principes fonctionnels"
info: C# 101, pour développeurs Java. Séance 3 sur 5.
fonts:
  sans: Inter
  serif: Bebas Neue
  mono: JetBrains Mono
  weights: '400,500,600,700,800,900'
colorSchema: light
routerMode: hash
lineNumbers: false
transition: fade
mdc: true
layout: cover
---

<div class="kicker mb-4">Séance 3 / 5</div>

# C# 101

## LINQ & principes fonctionnels

<Zombie kind="brain" pos="bl" :size="6" />
<Zombie kind="zombie" pos="br" :size="6" />

<!--
Accueil. Séance 2 : on a appris à modéliser (records, struct, héritage, pattern matching).
Aujourd'hui : manipuler ces données. Les fonctions deviennent des valeurs, et LINQ en tire parti.
Fil rouge : le recensement de la horde, un registre de zombies, de survivants, de camps et de rencontres.
-->

---

# Au menu

| Temps | Phase |
|---|---|
| 0:00 | Combien de Streams pour compter la horde ? |
| 0:10 | Les fonctions sont des valeurs |
| 0:17 | Principes fonctionnels |
| 0:22 | LINQ vs Streams |
| 0:29 | Exécution différée et `yield` |
| 0:35 | Koans en binôme |
| 1:15 | Conclusions |

<Zombie kind="brain" pos="br" :size="5" />

---
layout: center
---

# Combien de Streams pour compter la horde ?

<div class="text-center text-xl mt-6">

**5 minutes, en binôme**, en Java Streams, sur une `List<Zombie> horde`<br>
avec `record Zombie(String nom, Type type, int faim, String quartier)`

</div>

<div class="grid grid-cols-3 gap-4 mt-8 text-center">
  <div class="pill text-base">les noms des affamés (faim > 5) du centre-ville</div>
  <div class="pill text-base">le nombre de zombies par type</div>
  <div class="pill text-base">le zombie le plus affamé</div>
</div>

<div v-click class="mt-8">

```csharp
horde.Where(z => z.Faim > 5 && z.Quartier == "Centre-ville").Select(z => z.Nom);
horde.CountBy(z => z.Type);
horde.MaxBy(z => z.Faim);
```

</div>

<Zombie kind="hand" pos="br" :size="5" />

<!--
Connexion (10 min).
1. En binôme, 5 min, sur papier : les trois requêtes en Java Streams.
2. On en écrit une ou deux au tableau. Points de friction habituels : Collectors.groupingBy(..., Collectors.counting()), max qui renvoie un Optional, comparingInt.
3. Au clic : la version LINQ. Mêmes idées, moins de cérémonie : pas de .stream(), pas de Collectors, MaxBy renvoie directement l'élément.
4. Question ouverte : « Qu'est-ce qui rend ça possible ? » → des fonctions passées en paramètre, c'est la première partie.
-->

---
layout: section
---

<div class="kicker mb-3">Partie 01</div>

# Les fonctions sont des valeurs

---
layout: two-cols-header
---

# `Func` et `Action`

::left::

<Lang name="java" />

```java {all|1-5|7-9|11-13|all}{at:1}
// FUNCTION : prend T, renvoie R
Function<Integer, Integer> doubler = x -> x * 2;
doubler.apply(21);
BiFunction<Integer, Integer, Integer> add
    = (a, b) -> a + b;

// CONSUMER : ne renvoie rien
Consumer<String> crier = s -> System.out.println(s);
crier.accept("braaains");

// SUPPLIER / PREDICATE
Supplier<String> cri = () -> "Braaains";
Predicate<Integer> affame = f -> f > 5;
```

::right::

<Lang name="csharp" />

```csharp {all|1-4|6-8|10-12|all}{at:1}
// FUNC : le DERNIER type est le retour
Func<int, int> doubler = x => x * 2;
doubler(21);                  // pas de .apply()
Func<int, int, int> add = (a, b) => a + b;

// ACTION : ne renvoie rien
Action<string> crier = s => Console.WriteLine(s);
crier("braaains");

// SANS PARAMÈTRE / BOOLÉEN
Func<string> cri = () => "Braaains";
Func<int, bool> affame = f => f > 5;
```

<!--
Func et Action sont des délégués : des types qui représentent une signature de méthode. On les appelle directement, comme une méthode.
Func<..., TResult> : le dernier paramètre de type est TOUJOURS le retour. Action<...> : pas de retour.
Jusqu'à 16 paramètres pour chacun.
-->

---

# Le dictionnaire des interfaces fonctionnelles

| Java | C# |
|---|---|
| `Function<T, R>` · `UnaryOperator<T>` | `Func<T, TResult>` · `Func<T, T>` |
| `BiFunction<T, U, R>` | `Func<T1, T2, TResult>` |
| `Supplier<T>` | `Func<T>` |
| `Consumer<T>` · `BiConsumer<T, U>` | `Action<T>` · `Action<T1, T2>` |
| `Runnable` | `Action` |
| `Predicate<T>` | `Func<T, bool>` (ou `Predicate<T>`) |
| `IntFunction`, `ToIntFunction`, `IntPredicate`, `IntBinaryOperator`... | inutiles : `Func<int, ...>` |

<div v-click class="callout mt-6">

**Pourquoi si peu ?** Les generics sont réifiés (séance 2) : `Func<int, int>` manipule de vrais `int`, sans boxing. Java a dû créer des dizaines de variantes pour éviter le boxing des primitifs.

</div>

<Zombie kind="skull" pos="br" :size="3.5" />

---
layout: two-cols-header
---

# Lambdas, method groups et closures

::left::

<Lang name="java" />

```java {all|1-2|4-6|8-11|all}{at:1}
// METHOD REFERENCE
horde.stream().map(Recensement::decrire);

// CAPTURE : effectivement final
int abattus = 0;
Runnable abattre = () -> abattus++; // ne compile pas

// FONCTION QUI RENVOIE UNE FONCTION
Predicate<Zombie> plusAffameQue(int seuil) {
    return z -> z.faim() > seuil;
}
```

::right::

<Lang name="csharp" />

```csharp {all|1-2|4-7|9-11|all}{at:1}
// METHOD GROUP : le nom suffit
horde.Select(Decrire);

// CLOSURE : on capture la variable elle-même
var abattus = 0;
Action abattre = () => abattus++;   // OK
for (var i = 0; i < 3; i++) fs.Add(() => i); // piège

// FONCTION QUI RENVOIE UNE FONCTION
Func<Zombie, bool> PlusAffameQue(int seuil)
    => z => z.Faim > seuil;
```

<!--
Method group : pas de ::, on passe le nom de la méthode. Le compilateur crée le délégué.
Closure : la lambda capture la variable, pas sa valeur. Le piège : toutes les lambdas créées dans un for partagent le même i, qui vaut 3 à la fin. foreach crée une nouvelle variable à chaque tour depuis C# 5.
Une fonction locale `static` interdit toute capture : utile pour être sûr de ne pas capturer par accident.
-->

---
layout: section
---

<div class="kicker mb-3">Partie 02</div>

# Principes fonctionnels

---

# Cinq principes, sans dogme

| Principe | L'idée | En C# |
|---|---|---|
| Fonctions d'ordre supérieur | une fonction prend ou renvoie une fonction | `Where(Func<T, bool>)`, `PlusAffameQue(5)` |
| Fonctions pures | même entrée, même sortie, aucun effet de bord | pas de `compteur++` dans un `Where` |
| Immuabilité | on crée une nouvelle valeur au lieu de modifier | `record` + `with`, `ImmutableList<T>` |
| Composition | assembler de petites fonctions | `Composer(f, g)`, un pipeline d'étapes |
| Expressions | décrire le résultat plutôt que les étapes | `Sum`, `Aggregate`, `switch` expression |

<div v-click class="callout mt-6">

C# reste un langage objet : on garde ces principes là où ils rendent le code **plus simple à lire et à tester**.

</div>

<!--
Fonction pure : le koan montre Impure(5) qui renvoie 6 puis 7. Impossible à tester sans connaître l'historique.
Un effet de bord dans une requête LINQ se rejoue à chaque parcours : on y revient avec l'exécution différée.
ImmutableList.Add renvoie une nouvelle liste ; en Java, List.of(...).add() lève une exception.
-->

---
layout: two-cols-header
---

# Composer des fonctions

::left::

<Lang name="java" />

```java {all|1-4|6-9|11-12|all}{at:1}
// COMPOSITION
Function<Integer, Integer> doubler = x -> x * 2;
Function<Integer, String> enCri = x -> x + " cerveaux !";
doubler.andThen(enCri).apply(21);

// CURRYING
Function<Integer, Function<Integer, Integer>> ajouter
    = a -> b -> a + b;
ajouter.apply(2).apply(5);

// REDUCE
etapes.stream().reduce(identity(), Function::andThen);
```

::right::

<Lang name="csharp" />

```csharp {all|1-4|6-8|10-11|all}{at:1}
// COMPOSITION : pas de andThen, on l'écrit
Func<A, C> Composer<A, B, C>(Func<A, B> f, Func<B, C> g)
    => x => g(f(x));
Composer(doubler, enCri)(21);    // "42 cerveaux !"

// CURRYING
Func<int, Func<int, int>> ajouter = a => b => a + b;
ajouter(2)(5);

// AGGREGATE ≈ REDUCE : un pipeline d'étapes immuables
etapes.Aggregate(bob, (zombie, etape) => etape(zombie));
```

<!--
Composer est un exercice des koans (02_PrincipesFonctionnels) : trois lignes, et on comprend ce que fait andThen.
Le dernier exercice du fichier fait passer un zombie par une liste d'étapes (nourrir, déplacer, faire évoluer), chacune renvoyant une copie avec `with`.
-->

---
layout: section
---

<div class="kicker mb-3">Partie 03</div>

# LINQ vs Streams

---
layout: two-cols-header
---

# La même requête

::left::

<Lang name="java" />

```java {all|1-5|7-9|11-12|all}{at:1}
// FILTER + MAP
horde.stream()
    .filter(z -> z.quartier().equals("Centre-ville"))
    .map(Zombie::nom)
    .toList();

// GROUPING
horde.stream().collect(
    groupingBy(Zombie::type, counting()));

// MAX → un Optional
horde.stream().max(comparingInt(Zombie::faim)).get();
```

::right::

<Lang name="csharp" />

```csharp {all|1-5|7-9|11-12|all}{at:1}
// WHERE + SELECT
horde
    .Where(z => z.Quartier == "Centre-ville")
    .Select(z => z.Nom)
    .ToList();

// GROUPING
horde.CountBy(z => z.Type);
// ou : horde.GroupBy(z => z.Type)

// MAX → l'élément lui-même
horde.MaxBy(z => z.Faim);
```

::bottom::

<div v-click="4" class="callout small mt-2">

Pas de `.stream()` : LINQ fonctionne directement sur tout `IEnumerable<T>` (tableaux, listes, dictionnaires, ensembles...).

</div>

---

# Le dictionnaire Streams ↔ LINQ

<div class="grid grid-cols-2 gap-8">
<div>

| Streams | LINQ |
|---|---|
| `filter` | `Where` |
| `map` | `Select` |
| `flatMap` | `SelectMany` |
| `sorted(comparing(...))` | `OrderBy` · `ThenBy` |
| `distinct` | `Distinct` · `DistinctBy` |
| `limit` · `skip` | `Take` · `Skip` |
| `takeWhile` | `TakeWhile` |
| `findFirst` | `First` · `FirstOrDefault` |

</div>
<div>

| Streams | LINQ |
|---|---|
| `anyMatch` · `allMatch` | `Any` · `All` |
| `count` | `Count` |
| `mapToInt(...).sum()` | `Sum` |
| `max(comparing(...))` | `MaxBy` · `MinBy` |
| `reduce` | `Aggregate` |
| `collect(groupingBy(...))` | `GroupBy` · `CountBy` |
| `collect(toMap(...))` | `ToDictionary` |
| `toList()` | `ToList` · `ToArray` |

</div>
</div>

<div class="callout small mt-4">

Et quelques opérateurs sans équivalent direct : `Join`, `Zip`, `Chunk`, `Index`

</div>

---

# La syntaxe requête

<div class="grid grid-cols-2 gap-8 mt-4">
<div>

### Syntaxe méthode

```csharp
var affames = horde
    .Where(z => z.Faim > 5)
    .OrderBy(z => z.Nom)
    .Select(z => z.Nom);
```

La plus courante, et celle des koans.

</div>
<div>

### Syntaxe requête

```csharp
var affames =
    from z in horde
    where z.Faim > 5
    orderby z.Nom
    select z.Nom;
```

Le même résultat : le compilateur la traduit en appels de méthodes.

</div>
</div>

<div v-click class="callout mt-6">

À savoir **lire** : on la croise dans du code existant, et elle reste pratique pour les jointures (`join ... on ... equals`) et les variables intermédiaires (`let`).

</div>

<!--
Pas d'équivalent en Java. Elle ressemble à du SQL, mais ce n'est que du sucre syntaxique : from/where/select deviennent Where/Select.
-->

---
layout: section
---

<div class="kicker mb-3">Partie 04</div>

# Exécution différée et `yield`

---
layout: two-cols
---

# Une requête est une recette

```csharp {all|1-6|8-9|11-12|all}
var affames = horde.Where(z =>
{
    Console.WriteLine($"examen de {z.Nom}");
    return z.Faim > 8;
});
// rien n'est affiché : rien ne s'est passé

affames.Count();  // 9 lignes affichées
affames.Count();  // 9 de plus : la requête est rejouée

var liste = affames.ToList(); // on matérialise une fois
liste.Count;                  // plus rien n'est rejoué
```

::right::

<div class="mt-20">

<v-clicks>

- Une requête LINQ ne s'exécute qu'au **parcours** : `foreach`, `ToList`, `Count`, `First`, `Sum`...
- Les éléments traversent la chaîne **un par un**, comme dans un Stream
- **Java** : un Stream ne sert qu'une fois (`IllegalStateException`)
- **C#** : une requête se **rejoue** à chaque parcours, et voit les changements de la source
- Parcourir deux fois une requête coûteuse ? **`ToList()`** d'abord

</v-clicks>

</div>

<!--
Le piège de la double énumération : une méthode qui reçoit un IEnumerable et fait Count() puis Max() relance deux fois la requête (ou la base de données, ou le scan de fichiers). Le dernier exercice de 05_ExecutionDifferee le fait toucher du doigt.
Rider signale « Possible multiple enumeration ».
-->

---
layout: two-cols-header
---

# `yield return`

::left::

<Lang name="java" />

```java {all|1-8|10-12|all}{at:1}
// ITERATOR À LA MAIN
class HordeInfinie implements Iterator<String> {
    private int numero = 1;
    public boolean hasNext() { return true; }
    public String next() {
        return "Zombie n°" + numero++;
    }
}

// OU UN STREAM
Stream.iterate(1, n -> n + 1)
    .map(n -> "Zombie n°" + n);
```

::right::

<Lang name="csharp" />

```csharp {all|1-7|9-10|all}{at:1}
// YIELD RETURN : le compilateur écrit l'itérateur
IEnumerable<string> HordeInfinie()
{
    var numero = 1;
    while (true)
        yield return $"Zombie n°{numero++}";
}

HordeInfinie().Take(3);   // n°1, n°2, n°3
// yield break : termine la séquence
```

<!--
Une méthode avec yield return est transformée en machine à états par le compilateur (comme async/await, séance 4).
Le corps ne s'exécute pas à l'appel, mais au fil du parcours : chaque MoveNext reprend là où le dernier yield s'est arrêté.
Une boucle infinie n'est pas un problème tant que personne ne demande tous les éléments (Count() ne terminerait jamais).
-->

---
layout: two-cols
---

# LINQ, sous le capot

```csharp
public static IEnumerable<T> Filtrer<T>(
    this IEnumerable<T> source,
    Func<T, bool> predicat)
{
    foreach (var element in source)
        if (predicat(element))
            yield return element;
}

horde.Filtrer(z => z.Faim > 8).Take(3);

// et vos propres opérateurs métier
horde.DansLeQuartier("Port").Affames();
```

::right::

<div class="mt-20">

<v-clicks>

- `horde.Where(...)` n'est que du sucre pour `Enumerable.Where(horde, ...)`
- Méthode d'extension (séance 2) + `Func` + `yield` = **tout LINQ**
- **Java** : impossible d'ajouter une méthode à `Stream`, il faut un `Collector` ou un `Gatherer` (Java 24)
- **`IQueryable<T>`** (Entity Framework) : la même syntaxe, mais la lambda devient un arbre d'expression **traduit en SQL**

</v-clicks>

</div>

<!--
Le dernier exercice des koans fait réécrire Where et Select (Filtrer, Projeter) avec yield, et vérifie qu'ils restent paresseux.
IQueryable : un mot seulement, on n'en fera pas ici. Where(z => z.Faim > 8) sur un DbSet devient WHERE Faim > 8 côté base.
-->

---

# Les différences qui piquent

| Java | C# | Piège |
|---|---|---|
| `x -> x * 2` | `x => x * 2` | la flèche |
| `f.apply(x)` | `f(x)` | une `Func` s'appelle comme une méthode |
| des dizaines d'interfaces | `Func` et `Action` | le **dernier** type de `Func` est le retour |
| capture « effectivement final » | capture de la variable | une boucle `for` partage sa variable |
| `.stream()` ... `.toList()` | LINQ sur tout `IEnumerable<T>` | pas de `stream()` |
| un Stream ne sert qu'une fois | la requête se rejoue | double énumération : `ToList()` d'abord |
| `map.get(k)` renvoie `null` | `dict[k]` lève une exception | `TryGetValue`, `GetValueOrDefault` |
| `Iterator` écrit à la main | `yield return` | séquences paresseuses, même infinies |

<Zombie kind="eye" pos="br" :size="3.5" />

---
layout: section
---

<div class="kicker mb-3">Partie 05</div>

# À vous de jouer

<Zombie pos="br" :size="8" />

---

# Le registre de la horde

```csharp
record Zombie(string Nom, TypeDeZombie Type, int Faim, string Quartier, DateOnly TransformeLe);
record Arme(string Nom, int Degats, int Munitions);
record Survivant(string Nom, int Age, string Camp,
                 IReadOnlyList<string> Competences, Arme? Arme);
record Camp(string Nom, string Ville, int Capacite);
record Rencontre(string Survivant, string Zombie, DateOnly Date, Issue Issue);
```

```csharp
Registre.Zombies     // 9 zombies, dans 3 quartiers
Registre.Survivants  // 6 survivants, dans 3 camps (Maggie n'a pas d'arme)
Registre.Camps       // Alexandria, Hilltop, Kingdom
Registre.Rencontres  // 8 rencontres : victoire, fuite ou morsure
```

<div class="callout mt-4">

`04_LeRecensement.cs` : **12 questions** du QG, de difficulté croissante, une expression LINQ chacune

</div>

---

# 40 minutes, en binôme

| Fichier | Thème |
|---|---|
| `01_FuncEtAction.cs` | `Func`, `Action`, method groups, closures |
| `02_PrincipesFonctionnels.cs` | fonctions pures, immuabilité, composition |
| `03_Collections.cs` | `List`, `Dictionary`, `HashSet`, `Queue` |
| `04_LeRecensement.cs` | **12 questions** sur le registre de la horde |
| `05_ExecutionDifferee.cs` | quand la requête s'exécute, double énumération |
| `06_Yield.cs` | `yield return`, séquences infinies |
| `07_MethodesDExtension.cs` | ses propres opérateurs, réécrire `Where` et `Select` |

<div class="callout mt-6">

On change de pilote à chaque fichier · priorité à **01**, **04** et **05**

</div>

<Zombie kind="hand" pos="br" :size="4" />

<!--
57 koans : comme en séance 2, c'est volontairement trop pour 40 minutes. 04 est le cœur de la séance.
Pièges habituels : le for qui partage sa variable (01), Impure qui renvoie 6 puis 7 (02), la clé absente du Dictionary (03), ThenBy oublié (Q02), CountBy puis ToDictionary (Q04), Arme?.Degats ?? 0 (Q08), la double énumération (05).
-->

---

# Conclusions

<v-clicks>

- Correction live des questions du recensement qui ont résisté
- **Retour à la connexion** : réécrivez vos trois Streams en LINQ, qu'est-ce qui a disparu ?
- **Tour de table** : quelle boucle de votre code Java deviendrait une requête ?

</v-clicks>

<!--
Ce qui disparaît en général : .stream(), Collectors, Optional.get(), comparingInt.
Ouverture vers la séance 4 : yield return génère une machine à états, async/await fait pareil pour l'asynchrone.
-->

---
layout: end
---

<div class="text-8xl mb-4"><game-icons-brain class="inline" /></div>

# Braaains !

<div class="kicker mt-2">Prochaine séance · Async et concurrence</div>
