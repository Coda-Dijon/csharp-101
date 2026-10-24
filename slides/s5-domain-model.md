---
theme: default
title: "C# 101 · Séance 5 · Le Food-Truck Zombie"
info: C# 101, pour développeurs Java. Séance 5 sur 6.
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

<div class="kicker mb-4">Séance 5 / 6</div>

# C# 101

## Le Food-Truck Zombie, en TDD

<Zombie kind="brain" pos="bl" :size="6" />
<Zombie kind="zombie" pos="br" :size="6" />

<!--
Quatre séances d'outils. Aujourd'hui, pas de nouveau concept de langage : on assemble.
Pas de koans non plus : ce sont les participants qui écrivent les tests.
Pas de Docker aujourd'hui : tout le domaine se teste en mémoire.
-->

---
layout: center
---

<div class="kicker mb-4 text-center">Le but du jour</div>

<div class="text-4xl font-bold text-center" style="line-height: 1.35">
  <div>Mettre en pratique <span style="color: var(--blood)">tout ce qu'on a vu</span></div>
  <div>en concevant un <span style="color: var(--blood)">domain model</span>, en <span style="color: var(--blood)">TDD</span></div>
</div>

<div class="grid grid-cols-4 gap-4 mt-12 small">
<div v-click class="callout"><strong>Séance 1</strong><br>switch expressions, nullabilité, exceptions</div>
<div v-click class="callout"><strong>Séance 2</strong><br>records, encapsulation, hiérarchies, pattern matching, generics</div>
<div v-click class="callout"><strong>Séance 3</strong><br>fonctions pures, immuabilité, LINQ, extensions</div>
<div v-click class="callout"><strong>Séance 4</strong><br><code>async</code> / <code>await</code>, concurrence, <code>IAsyncEnumerable</code></div>
</div>

<div v-click class="text-center text-xl mt-10">

⟶ un seul domaine, le **Winchester**, construit test après test

</div>

<!--
Le message à faire passer d'emblée : rien de nouveau dans le langage aujourd'hui. Chaque histoire réinvestit une ou deux séances.
Le TDD n'est pas le sujet en soi : c'est la méthode qui nous fait concevoir le modèle par l'usage, un exemple à la fois.
-->

---

# Au menu

| Temps | Phase |
|---|---|
| 0:00 | Votre boîte à outils |
| 0:10 | Le TDD en 5 minutes, et modéliser un domaine |
| 0:25 | Histoire 1 en mob |
| 0:35 | Les histoires en binôme, en ping-pong |
| 1:15 | Pour aller plus loin : monades, *functional core*, union types |
| 1:25 | Rétro |

<Zombie kind="brain" pos="br" :size="5" />

---

# Votre boîte à outils

<div class="text-center text-lg mt-2">

Le Winchester a cinq besoins. **Quel outil des séances 1 à 4** pour chacun ?

</div>

<div class="grid grid-cols-5 gap-3 mt-8 text-center">
  <div class="pill text-sm">Un prix en dents, jamais négatif</div>
  <div class="pill text-sm">Un client vegan, ou vivant, mais pas un booléen de chaque</div>
  <div class="pill text-sm">Dire <em>pourquoi</em> une commande est refusée</div>
  <div class="pill text-sm">Les comptes du soir</div>
  <div class="pill text-sm">100 zombies, 10 cervelles, en même temps</div>
</div>

<div v-click class="grid grid-cols-5 gap-3 mt-6 text-center small">
  <div class="callout"><code>record struct</code> + factory method</div>
  <div class="callout">une hiérarchie de <code>record</code></div>
  <div class="callout"><code>Result&lt;T&gt;</code> plutôt qu'une exception</div>
  <div class="callout">LINQ</div>
  <div class="callout"><code>async</code> + <code>SemaphoreSlim</code></div>
</div>

<Zombie kind="hand" pos="br" :size="4" />

<!--
Connexion (10 min), en binôme, sur post-its : un besoin, un outil, une séance.
On ne corrige pas tout de suite : la réponse affichée est celle de la solution, d'autres choix se défendent (une class avec un constructeur, un enum...). On y revient en conclusion.
-->

---
layout: section
---

<div class="kicker mb-3">Partie 01</div>

# Le TDD en 5 minutes

---
layout: two-cols
---

# Rouge, vert, refactor

<div class="flex flex-col gap-4 mt-8 mr-8">
  <div class="callout" style="border-left-color: var(--blood)">
    <div class="font-bold text-lg">🔴 Rouge</div>
    J'écris <strong>un</strong> test, pour un exemple. Il échoue... ou ne compile pas.
  </div>
  <div class="callout" style="border-left-color: #2e7d32; background: #edf7ee">
    <div class="font-bold text-lg">🟢 Vert</div>
    J'écris le <strong>minimum</strong> de code pour le faire passer. Même naïf.
  </div>
  <div class="callout" style="border-left-color: #1565c0; background: #eaf2fb">
    <div class="font-bold text-lg">🔵 Refactor</div>
    Je nettoie le code <strong>et</strong> les tests, sans changer de couleur.
  </div>
</div>

::right::

<div class="mt-20">

<v-clicks>

- On commence par une **liste de tests** : les exemples de l'histoire (Kent Beck)
- Un test = un exemple **du métier**, avec un nom qui se lit comme une phrase
- Le test est le **premier client** du code : il dessine l'API avant qu'elle existe
- En C#, le compilateur fait partie du rouge : « `Dents` n'existe pas » est un premier échec
- Rien de propre à C# : c'est le même cycle qu'avec JUnit

</v-clicks>

</div>

<!--
Les trois lois : pas de code de production sans un test rouge ; pas plus de test que nécessaire pour échouer ; pas plus de code que nécessaire pour passer.
dotnet watch test relance les tests à chaque sauvegarde, comme un mode continu.
-->

---
layout: two-cols-header
---

# Le premier test, en direct

::left::

<div class="kicker mb-2">🔴 Le test d'abord</div>

```csharp
[Fact]
public void Un_prix_se_cree_a_partir_d_un_nombre_de_dents()
{
    var prix = Dents.Creer(12).DoitReussir();

    Check.That(prix.Valeur).IsEqualTo(12);
}
```

<div v-click="1" class="small mt-2">

```text
error CS0103: The name 'Dents' does not exist
              in the current context
```

</div>

::right::

<div v-click="2">

<div class="kicker mb-2">🟢 Le minimum</div>

```csharp
public readonly record struct Dents
{
    public int Valeur { get; }
    private Dents(int valeur) => Valeur = valeur;

    public static Result<Dents> Creer(int valeur)
        => new Dents(valeur);
}
```

</div>

<div v-click="3" class="mt-4">

<div class="kicker mb-2">🔵 Et ensuite ?</div>

- Rien à nettoyer : test suivant de la liste
- « Un prix négatif n'existe pas » : un nouveau rouge, qui fera naître le `PrixNegatif`

</div>

::bottom::

<div v-click="4" class="callout small mt-2">

Pas de validation, pas d'opérateur, pas de `ToString` **tant qu'aucun test ne les demande**. Le design émerge des exemples.

</div>

<!--
À faire en direct dans l'IDE, pas sur la slide : c'est le début de l'histoire 1 en mob.
Faire remarquer que le test impose déjà la forme : Creer renvoie un Result<Dents>, donc la création peut échouer. Le record struct donnera ensuite l'égalité gratuitement (séance 2).
-->

---
layout: section
---

<div class="kicker mb-3">Partie 02</div>

# Modéliser le domaine

---

# Le Winchester, en un coup d'œil

<div class="grid grid-cols-3 gap-4 mt-6 small">
<div v-click class="callout">
  <div class="kicker mb-2">Value objects</div>
  <code>Dents</code> · un prix, jamais négatif (<code>Creer</code>)<br>
  <code>Plat</code> · nom, catégorie, prix, date de préparation<br>
  <span class="muted">définis par leur valeur, immuables</span>
</div>
<div v-click class="callout">
  <div class="kicker mb-2">Agrégat</div>
  <code>Commande</code> · client, plat, quantité, état<br>
  <code>Passer(…)</code>, <code>Preparer(…)</code>, <code>Servir()</code><br>
  <span class="muted">on ne la crée et ne la change que par ses méthodes</span>
</div>
<div v-click class="callout">
  <div class="kicker mb-2">Erreurs</div>
  <code>ClientEncoreVivant</code>, <code>RuptureDeStock</code>…<br>
  un <code>record</code> par refus, dans un <code>Result&lt;T&gt;</code><br>
  <span class="muted">le refus fait partie de la signature</span>
</div>
</div>

<div class="grid grid-cols-2 gap-4 mt-4 small">
<div v-click class="callout">
  <div class="kicker mb-2">Hiérarchie · les clients</div>
  <code>Client</code> ⟶ <code>Rodeur</code> · <code>ZombieVegan</code> · <code>Survivant</code>
</div>
<div v-click class="callout">
  <div class="kicker mb-2">Hiérarchie · les états</div>
  <code>Passee</code> ⟶ <code>EnPreparation(cuisinier)</code> ⟶ <code>Servie</code> | <code>Devoree(parLeCuisinier)</code>
</div>
</div>

<div v-click class="text-center mt-6">

**Ni horloge, ni base, ni thread** dans le modèle : la date et le stock sont des paramètres

</div>

<!--
Le vocabulaire du Domain-Driven Design, sans le dogme : value object, agrégat, invariant.
Ce diagramme est celui de la solution. Les binômes peuvent arriver à un autre modèle : c'est le principe du TDD, du moment que les exemples passent.
-->

---
layout: two-cols-header
---

# Value object : une factory method

::left::

<div class="kicker mb-2">❌ Le constructeur qui lance</div>

```csharp
public readonly record struct Dents
{
    public int Valeur { get; }

    public Dents(int valeur)
    {
        ArgumentOutOfRangeException
            .ThrowIfNegative(valeur);
        Valeur = valeur;
    }
}

var prix = new Dents(-1);   // 💥 surprise
```

::right::

<div v-click="1">

<div class="kicker mb-2">✅ La factory method</div>

```csharp
public readonly record struct Dents
{
    public int Valeur { get; }
    private Dents(int valeur) => Valeur = valeur;

    public static Result<Dents> Creer(int valeur)
        => valeur < 0
            ? new PrixNegatif(valeur)
            : new Dents(valeur);
}

Dents.Creer(-1);   // Echec(PrixNegatif(-1))
```

</div>

::bottom::

<div class="grid grid-cols-3 gap-3 mt-2 small">
<div v-click="2" class="callout">Un constructeur ne peut renvoyer que l'objet : s'il peut échouer, <strong>sa signature ment</strong></div>
<div v-click="3" class="callout"><code>Result&lt;Dents&gt;</code> l'avoue : l'appelant doit traiter l'échec, et sait <strong>pourquoi</strong></div>
<div v-click="4" class="callout"><code>+</code> et <code>*</code> gardent le constructeur privé : ils ne peuvent pas produire un prix négatif</div>
</div>

<!--
Piège : default(Dents) contourne la factory et vaut 0 dent. Ici c'est acceptable ; pour un value object où 0 est invalide, on préfère un record class.
Java n'a pas de surcharge d'opérateurs : plus(), times(). En Java aussi, on préfère une méthode statique of(...) qui renvoie un Optional ou un résultat à un constructeur compact qui lance.
La multiplication prend un uint : le type interdit déjà une quantité négative.
-->

---
layout: two-cols-header
---

# Rendre les états illégaux irreprésentables

::left::

<div class="kicker mb-2">❌ Tout dans une classe</div>

```csharp
class Commande
{
    public string Etat { get; set; } // "SERVIE"...
    public string? Cuisinier { get; set; }
    public bool Devoree { get; set; }
    // Servie ET dévorée ? Un cuisinier sans préparation ?
}
```

<div v-click="1" class="small mt-3">

```csharp
public abstract record EtatDeCommande;
public sealed record Passee : EtatDeCommande;
public sealed record EnPreparation(string Cuisinier)
    : EtatDeCommande;
public sealed record Servie : EtatDeCommande;
public sealed record Devoree(string ParLeCuisinier)
    : EtatDeCommande;
```

</div>

::right::

<div v-click="2">

<div class="kicker mb-2">✅ Un état = un type</div>

```csharp
public Result<Commande> Servir() => Etat switch
{
    EnPreparation => this with { Etat = new Servie() },
    _ => new TransitionImpossible(Etat, "Servir")
};

public Result<Commande> EtreDevoreeEnCuisine()
    => Etat switch
    {
        EnPreparation p =>
            this with { Etat = new Devoree(p.Cuisinier) },
        _ => new TransitionImpossible(Etat, "Devorer")
    };
```

</div>

<div v-click="3" class="callout small mt-3">

Chaque état ne porte que **ses** données. Chaque transition renvoie une **nouvelle** commande (`with`) : l'ancienne ne change pas.

</div>

<!--
« Make illegal states unrepresentable » (Yaron Minsky). Java 21 : sealed interface Etat permits Passee, EnPreparation... et des records, avec un switch exhaustif.
En C#, la hiérarchie n'est pas fermée pour le compilateur : il exige le cas _. Les union types régleront ça (fin de séance).
-->

---

# Un refus n'est pas une exception

<div class="grid grid-cols-[3fr_2fr] gap-6">
<div>

```csharp
public static Result<Commande> Passer(Client client, Plat plat,
    int quantite, int enStock, DateOnly aujourdhui) => client switch
{
    _ when quantite <= 0 => new QuantiteInvalide(quantite),
    Survivant s => new ClientEncoreVivant(s.Nom),
    _ when !client.PeutManger(plat)
        => new RegimeNonRespecte(client.Nom, plat.Categorie),
    _ when plat.EstPerime(aujourdhui)
        => new CerveauPerime(plat.Nom, plat.Age(aujourdhui)),
    _ when enStock < quantite
        => new RuptureDeStock(plat.Nom, enStock),
    _ => new Commande(client, plat, quantite)
};
```

</div>
<div class="small">

<v-clicks>

- `Result<T>` est fourni : un `Succes(T Valeur)` ou un `Echec(Erreur)`
- Le refus est **dans la signature** : impossible de l'oublier
- Grâce aux conversions implicites, on renvoie l'erreur ou la commande, directement
- Le constructeur de `Commande` est **privé** : `Passer` est la seule porte d'entrée
- Comme `Dents.Creer` : ce qui peut échouer le dit dans sa signature

</v-clicks>

</div>
</div>

<!--
Java : pas de Result standard. On l'écrit avec une sealed interface, ou on utilise Vavr (Either<L, R>).
Le switch sur client sert surtout de garde ordonnée : l'ordre des branches est celui des règles de l'histoire 4.
-->

---

# Sept histoires, quatre séances

| Histoire | Ce qu'on construit | Réinvestit |
|---|---|---|
| 1 · Le prix en dents | `Dents`, un value object et sa factory | S2 `record struct`, constructeur privé, opérateurs · `Result<T>` |
| 2 · Un cerveau frais | `Plat`, et sa date de préparation | S2 `record`, `enum` · S1 `DateOnly` |
| 3 · Chacun son régime | les clients et ce qu'ils mangent | S2 hiérarchie, pattern matching · S3 extension |
| 4 · Passer une commande | l'agrégat `Commande`, ses refus | S2 constructeur privé, generics · `Result<T>` |
| 5 · La vie d'une commande | les états, les transitions | S2 hiérarchie d'états, `with` · `Bind` |
| 6 · Les comptes du soir | les statistiques | S3 LINQ, fonctions pures |
| 7 · Le rush de minuit | le comptoir face à la horde | S4 `async`, `SemaphoreSlim`, `IAsyncEnumerable` |

<div class="callout mt-4">

Les histoires **1 à 5** forment le cœur du domaine · **6** et **7** sont des bonus

</div>

---
layout: section
---

<div class="kicker mb-3">Partie 03</div>

# À vous de jouer

<Zombie pos="br" :size="8" />

---

# Le Winchester, côté code

<div class="grid grid-cols-2 gap-8">
<div>

```
s5-domain-model/
├── Winchester/
│   ├── Result.cs         🎁 fourni
│   ├── Dents.cs          ✍️ histoire 1
│   ├── Plat.cs           ✍️ histoire 2
│   ├── ...
│   └── Comptoir.cs       ✍️ histoire 7
└── Winchester.Tests/
    ├── Verifications.cs  🎁 DoitReussir, DoitEchouer
    ├── 01_LePrixEnDents.cs
    └── ...
```

```bash
cd s5-domain-model/Winchester.Tests
dotnet watch test
```

</div>
<div>

Chaque fichier de test contient :

<v-clicks>

- **L'histoire** : qui, quoi, pourquoi
- **Les exemples** : les règles, avec des données concrètes
- **La liste de tests** : `[ ] Un_prix_ne_peut_pas_etre_negatif`...
- Seul le tout premier test est écrit, en commentaire : décommentez-le, puis à vous les autres, **un à la fois**

</v-clicks>

<div v-click class="callout mt-4">

Pas de blanc `___` aujourd'hui : **vous** écrivez les tests, **vous** choisissez les noms et la forme du code.

</div>

</div>
</div>

---

# 50 minutes, en mob puis en binôme

<div class="grid grid-cols-2 gap-8 mt-4">
<div>

<div class="kicker mb-2">10 min · Histoire 1 en mob</div>

- Un clavier pour toute la salle, on tourne toutes les 3 minutes
- Le premier rouge, le premier vert, le premier refactor, ensemble
- Objectif : que tout le monde ait vu le cycle **une fois**

</div>
<div>

<div class="kicker mb-2">40 min · En binôme, en ping-pong</div>

- A écrit un test rouge, B le fait passer, puis B écrit le test suivant...
- Un commit (ou au moins une pause) à chaque vert
- Bloqués plus de 5 minutes ? Levez la main, ou regardez la solution **d'une seule** histoire

</div>
</div>

<div class="callout mt-8">

Priorité aux histoires **1 à 5** · le refactor n'est pas optionnel

</div>

<Zombie kind="hand" pos="br" :size="4" />

<!--
Pièges habituels :
- histoire 2 : DateTime.Now dans le plat, le test devient faux demain ; on passe la date.
- histoire 4 : l'ordre des règles (Shaun refusé comme vivant, même avec un plat périmé) ; un switch sur client avec des gardes when.
- histoire 5 : modifier l'état en place (set) au lieu de renvoyer une nouvelle commande avec with.
- histoire 7 : lock ne permet pas await à l'intérieur ; SemaphoreSlim(1, 1) avec WaitAsync / Release dans un finally.
-->

---
layout: section
---

<div class="kicker mb-3">Partie 04</div>

# Pour aller plus loin

---
layout: two-cols
---

# `Result<T>` est une monade

```csharp {all|1-4|6-10|all}{at:1}
// BIND : enchaîner des étapes qui peuvent échouer
var servie = Commande.Passer(bob, cervelle, 2, 10, jour)
    .Bind(c => c.Preparer("Gégé"))
    .Bind(c => c.Servir());

// LA MÊME CHOSE, EN SYNTAXE REQUÊTE (séance 3)
var servie = from passee in Commande.Passer(...)
             from enCuisine in passee.Preparer("Gégé")
             from prete in enCuisine.Servir()
             select prete;
```

<div v-click="3" class="callout small mt-4">

Le premier échec **court-circuite** la suite : pas de `if` après chaque étape.

</div>

::right::

<div class="mt-16">

<v-clicks at="4">

- Une monade : une **boîte** (`Result<T>`) et un **`Bind`** pour enchaîner ce qui renvoie une boîte
- Vous en utilisez déjà : `Optional.flatMap`, `Stream.flatMap`, `CompletableFuture.thenCompose`
- En C# : `SelectMany` pour `IEnumerable`, `await` pour `Task`, `?.` pour `null`
- Il suffit d'écrire `Select` et `SelectMany` pour que LINQ accepte n'importe quelle monade

</v-clicks>

</div>

<!--
Pas besoin de la théorie des catégories : « une boîte, et une façon d'enchaîner ce qui renvoie une boîte ».
Le Result.cs fourni contient Map, Bind, Match, Select et SelectMany : ouvrez-le après la séance.
Lien avec la séance 4 : async/await, c'est la syntaxe dédiée de C# pour la monade Task.
-->

---

# Functional core, imperative shell

<div class="grid grid-cols-2 gap-8">
<div class="mt-4">
<div class="rounded-lg p-4" style="border: 2px dashed var(--muted)">
  <div class="kicker mb-2">La coquille · impérative</div>
  <div class="small"><code>Comptoir</code> : l'horloge, le stock, le verrou, l'<code>async</code><br>Séance 6 : l'API HTTP, PostgreSQL</div>
  <div class="rounded-lg p-4 mt-4" style="border: 2px solid var(--blood); background: var(--blood-soft)">
    <div class="kicker mb-2">Le cœur · fonctionnel</div>
    <div class="small"><code>Dents</code>, <code>Plat</code>, <code>Commande.Passer</code>, les transitions, <code>Statistiques</code><br>des fonctions pures, des données immuables</div>
  </div>
</div>
</div>
<div class="small">

<v-clicks>

- Le **cœur** décide : pas d'horloge, pas de base, pas de thread. Des entrées, une sortie
- La **coquille** agit : elle lit le monde, appelle le cœur, applique sa décision
- Des centaines de tests rapides sur le cœur, quelques tests d'intégration sur la coquille
- C'est ce qui rend le TDD facile ici : `Passer(..., aujourdhui)` plutôt que `DateTime.Now`
- Séance 6 : la même coquille, mais en ASP.NET Core avec PostgreSQL

</v-clicks>

</div>
</div>

<!--
Gary Bernhardt, « Boundaries » (2012). Proche de l'architecture hexagonale de la séance 6 : le cœur ne dépend de rien.
Dans l'histoire 7, la règle « pas plus que le stock » reste dans Passer ; le comptoir ne fait que fournir le stock et le décrémenter sous verrou.
-->

---
layout: two-cols
---

# 🔮 Bientôt : les union types

```csharp
// C# 15 (preview .NET 11), syntaxe non définitive
public record Passee;
public record EnPreparation(string Cuisinier);
public record Servie;
public record Devoree(string ParLeCuisinier);

public union EtatDeCommande(
    Passee, EnPreparation, Servie, Devoree);

string Libelle(EtatDeCommande etat) => etat switch
{
    Passee => "en attente",
    EnPreparation p => $"chez {p.Cuisinier}",
    Servie => "servie",
    Devoree d => $"dévorée par {d.ParLeCuisinier}"
};  // exhaustif : plus de « _ => throw »
```

::right::

<div class="mt-16">

<v-clicks>

- Aujourd'hui : `abstract record` + constructeur privé, une hiérarchie fermée **par convention**
- Avec les unions : le compilateur **connaît** tous les cas, et vérifie chaque `switch`
- Ajouter un état casse la compilation partout où il manque : c'est ce qu'on veut
- `Result<T>` et les `Erreur` sont les candidats suivants
- Java 17 l'a déjà, avec `sealed interface ... permits`

</v-clicks>

</div>

<!--
À revérifier au moment de la séance : la syntaxe et le calendrier des union types (C# 15 / .NET 11) peuvent encore changer.
Promesse tenue de la séance 2 : « les union types arrivent, on les verra en séance 5 ».
-->

---

# Les différences qui piquent

| Java | C# | Piège |
|---|---|---|
| `record` + constructeur compact qui lance | constructeur privé + factory `Creer` | `default(Dents)` contourne la factory |
| `sealed interface ... permits` | `abstract record` + constructeur privé | pas d'exhaustivité : le `_` reste obligatoire |
| méthodes `withX(...)` à écrire | `with { X = ... }` | une copie **superficielle** : les listes sont partagées |
| `Optional.flatMap` | `Bind`, `SelectMany` | à écrire soi-même pour son `Result<T>` |
| pas de surcharge d'opérateurs | `operator +`, `operator *` | à réserver aux vraies valeurs (`Dents`) |
| `synchronized` | `lock`, `SemaphoreSlim` | pas d'`await` dans un `lock` |

<Zombie kind="skull" pos="br" :size="3.5" />

---

# Conclusions

<v-clicks>

- Correction live de l'histoire 4 : `Commande.Passer`, les règles dans l'ordre
- **Retour à la connexion** : quel outil avez-vous vraiment pris pour chaque besoin ?
- **Rétro de la séance** : le TDD vous a-t-il fait écrire un autre modèle que celui que vous auriez dessiné d'abord ?
- **Et après ?** Les niveaux 5 à 7 du *.NET Maturity Stack* : l'écosystème, la performance, l'architecture

</v-clicks>

<!--
La rétro peut se faire en « +/Δ » sur deux colonnes de post-its.
Faire le lien : le Winchester de la séance 6 est une version simplifiée de ce modèle, exposée en API.
-->

---
layout: end
---

<div class="text-8xl mb-4"><game-icons-brain class="inline" /></div>

# Braaains !

<div class="kicker mt-2">Prochaine séance · Exposer le Winchester en API</div>
