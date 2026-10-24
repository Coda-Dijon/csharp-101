# C# 101 — Plan de formation (5 × 1h30)

## Context
Créer une formation hands-on « C# 101 » dans l'esprit de `fp101/` (code d'abord, exercices à compléter, domaine fil rouge) et du deck `fsharp.pdf` (comparaisons **Java ↔ C#** pour ancrer chaque concept).
- **Public** : développeurs venant d'un autre langage (surtout Java).
- **Format** : koans xUnit en rouge → à faire passer au vert.
- **Périmètre** : niveaux 1 à 4 du *.NET Maturity Stack* (IMG_9725.jpg), sans viser l'exhaustivité.
- **Exercices autonomes** : chaque fichier de test se suffit à lui-même (pas de dépendance d'une séance à l'autre).
- **Séance 5** : modéliser un domaine complet en C#.

## Déroulé type d'une séance (90 min)
| Temps | Activité |
|---|---|
| 10 min | **Connexion** : une question ouverte en post-its (séance 1 : « Qu'est-ce que C# m'inspire ? ») |
| 15 min | **Concepts** : slides Java ↔ C# côte à côte (style fp101, Slidev) |
| 50 min | **Pratique** : koans en binôme ou en mob, du rouge au vert |
| 15 min | **Conclusion** : correction live, puis « ce qui m'a surpris par rapport à Java » |
| bonus 5 min | **🔮 Futur de C#** : les features encore en preview liées au thème du jour (projet séparé `preview/` en `<LangVersion>preview</LangVersion>` sur le SDK .NET 11 preview) |

> ⚠️ Le statut des previews (C# 15 / .NET 11) est à revérifier au moment de préparer chaque séance, car la syntaxe peut encore changer.

## Programme

### Séance 1 : Fondamentaux (niveau 1)
*Objectif : écrire, lire et déboguer du C# en confiance.*
- **Histoire de .NET en 10 min (frise)** :
  - 2002 : .NET Framework 1.0 + C# 1.0 (Anders Hejlsberg, ex-Turbo Pascal/Delphi), dans le contexte du procès Sun/Microsoft autour de J++
  - 2005 : generics réifiés (C# 2) ; 2007 : LINQ + lambdas (C# 3), avant Java 8
  - 2012 : `async/await` (C# 5), un modèle repris ensuite par JS, Python et Rust
  - 2014 : open source, Roslyn ; 2016 : .NET Core, cross-platform (Linux/macOS)
  - 2020 : .NET 5 unifie Framework, Core et Mono ; une version par an, LTS les années paires
  - 2025 : .NET 10 LTS / C# 14
- **Comment le code s'exécute** (parallèle avec la JVM) :
  - `.cs` → compilateur **Roslyn** → **IL** (CIL, ≈ bytecode) dans un **assembly** `.dll` (≈ `.jar`) avec ses métadonnées
  - **CLR** (≈ JVM) : **JIT** RyuJIT, Tiered compilation, GC générationnel
  - Alternatives : **Native AOT** (≈ GraalVM native-image), ReadyToRun
  - Démo : `dotnet build`, puis ouverture de la dll dans **ILSpy** / sharplab.io pour voir l'IL et le code « lowered » (par exemple ce que devient un `record` ou un `foreach`)
  - Écosystème : SDK vs runtime, NuGet (≈ Maven Central), `.csproj` (≈ `pom.xml`)
- Toolchain : `dotnet new`, `dotnet test`, `dotnet run app.cs` (fichier unique, .NET 10)
- `var`/types, `const`/`readonly`, interpolation `$"..."`, raw strings `"""`
- `if`, `switch` expression vs `switch` Java, boucles, `foreach`
- Méthodes : paramètres nommés et par défaut, `out`, expression-bodied `=>`
- Strings (immutables, `StringBuilder`), enums, `try/catch/finally`, `using`
- Nullabilité : `string?`, `?.`, `??`, `??=` vs `Optional` Java
- **Koans** : `01_Syntax`, `02_Strings`, `03_ControlFlow`, `04_Nullability`, `05_Exceptions`
- 🔮 **Preview** : **closed enums**, avec des `switch` exhaustifs sans branche `_` (comparés au `switch` sur un enum Java 21) — *retiré des slides de la séance 1 à la demande (pas de slide « futur »)*

### Séance 2 : Types & design objet (niveau 2)
*Objectif : modéliser avec des types qui disent ce qu'ils veulent dire.*
- Classes, **propriétés** (`get; init;`) vs getters/setters Java
- **Encapsulation** : `public`, `private` (défaut des membres), `protected` (dérivées seulement, sans l'accès « package » de Java), `internal` (défaut des classes, à l'échelle de l'assembly et non du dossier), `protected internal`, `private protected`, `file` ; `sealed` sur une classe et `sealed override` sur une méthode (≈ `final`) ; exposer une collection sans la laisser fuiter (`IReadOnlyList` + `AsReadOnly()`)
- `record` / `record struct` vs `record` Java 16+ ; `with`, égalité structurelle
- `struct` vs `class` (valeur vs référence), un point absent de Java
- **Paramètres `ref`, `out` et `in`** : Java passe tout par valeur (y compris les références) ; `ref` donne la variable de l'appelant (modifier une struct, remplacer un objet), `out` oblige la méthode à l'affecter (pattern `TryXxx`, `out var`, discard `out _`), `in` passe par référence en lecture seule
- Interfaces (default methods), héritage, `sealed`, `abstract`
- **Classes `static`** vs classe utilitaire Java (`final` + constructeur privé) : ni instanciables ni héritables, membres tous `static`, constructeur statique, `using static` (≈ `import static`) ; pas de static nested au sens Java (une classe imbriquée C# n'a jamais accès implicite à l'instance englobante). Ouvre la porte aux méthodes d'extension de la séance 3
- Generics réifiés (≠ type erasure Java), contraintes `where T :`
- Pattern matching : `is`, propriétés, positionnel, listes `[1, .., var last]`
- Composition plutôt qu'héritage
- **Koans** : `01_Proprietes`, `02_Encapsulation`, `03_ValeurOuReference` (dont `ref`/`out`), `04_Heritage`, `05_Records` (après l'héritage, pour parler de l'héritage de records), `06_ClassesStatiques`, `07_Generiques`, `08_PatternMatching`
- 🔮 **Preview** : **union types** (`union Shape(Circle, Square)`) et **closed classes** (hiérarchies fermées, exhaustivité vérifiée par le compilateur), comparés aux `sealed interface ... permits` de Java 17

### Séance 3 : Collections & LINQ (niveau 3)
*Objectif : transformer de la donnée brute en requêtes claires et composables.*
*Fil rouge : **le recensement de la horde**. Le QG des survivants a récupéré le registre des zombies, des survivants, des camps et des rencontres : à vous de l'exploiter pour planifier la survie.*
- `List<T>`, `Dictionary<K,V>`, `HashSet<T>`, `IReadOnlyList<T>`, collection expressions `[..]`
- Lambdas, `Func<>`/`Action<>` vs interfaces fonctionnelles Java (`Function`, `Predicate`, `Consumer`), closures (capture de variables, piège de la variable de boucle)
- LINQ vs Streams : `Where/Select/OrderBy/ThenBy/GroupBy/Any/All/Count/Sum/Aggregate/SelectMany/Join/Zip`, plus les ajouts récents `MaxBy/MinBy/DistinctBy/Chunk/CountBy/AggregateBy/Index`
- Syntaxe requête (`from z in horde where ... select ...`) en une slide, pour savoir la lire
- **Exécution différée** et `IEnumerable<T>`, `yield return` (piège de la double énumération)
- Méthodes d'extension (`this` dans une classe `static`, vue en séance 2) et extension members C# 14
- `IQueryable` en un mot (pont vers EF Core)
- **Connexion** : « Combien de Streams pour compter la horde ? ». Sur papier, une liste de 10 zombies et trois questions à résoudre en Java Streams (les affamés du centre-ville, le nombre par type, le plus affamé), puis on compare avec LINQ
- **Domaine** (`Domain.cs`, des records de la séance 2, données dans une classe `static Registre`) :
  - `enum TypeDeZombie { Rodeur, Coureur, Goule, Tank }`
  - `record Zombie(string Nom, TypeDeZombie Type, int Faim, string Quartier, DateOnly TransformeLe)`
  - `record Arme(string Nom, int Degats, int Munitions)`
  - `record Survivant(string Nom, int Age, string Camp, IReadOnlyList<string> Competences, Arme? Arme)`
  - `record Camp(string Nom, string Ville, int Capacite)`
  - `record Rencontre(string Survivant, string Zombie, DateOnly Date, Issue Issue)` avec `enum Issue { Fuite, Victoire, Morsure }`
- **Koans** :
  - `01_Lambdas` : `Func`/`Action`/`Predicate`, closures, une lambda qui capture un compteur de zombies abattus
  - `02_Collections` : `List`, `Dictionary` (`TryGetValue`, `GetValueOrDefault`), `HashSet` (quartiers contaminés), collection expressions
  - `03_LeRecensement` : 12 questions de difficulté croissante sur le registre :
    1. les noms des zombies du « Centre-ville » (`Where`, `Select`)
    2. la horde triée par faim décroissante, puis par nom (`OrderByDescending`, `ThenBy`)
    3. y a-t-il un Tank ? tous les survivants sont-ils majeurs ? (`Any`, `All`)
    4. le nombre de zombies par type (`GroupBy` puis `CountBy`)
    5. la faim moyenne et le zombie le plus affamé de chaque quartier (`Average`, `MaxBy`)
    6. toutes les compétences des survivants, sans doublon (`SelectMany`, `Distinct`)
    7. les munitions totales du camp « Alexandria » (`Sum`), puis la même chose avec `Aggregate`
    8. le survivant le mieux armé, sachant que certains n'ont pas d'arme (`MaxBy`, `?.`, `??`)
    9. les survivants avec la ville de leur camp, et les camps surpeuplés (`Join`, `GroupBy`)
    10. le bilan des rencontres de chaque survivant : victoires, fuites, morsures (`GroupBy` + projection en tuple)
    11. les zombies répartis en escouades de 3 pour les tours de garde (`Chunk`, `Zip`)
    12. le premier zombie transformé et les 3 plus récents (`MinBy`, `OrderBy` + `Take`, `Skip`)
  - `04_ExecutionDifferee` : un générateur de horde qui journalise chaque zombie produit, pour voir quand la requête s'exécute vraiment (et la double énumération)
  - `05_Yield` : une horde infinie générée avec `yield return`, domptée par `Take` et `TakeWhile`
  - `06_MethodesDExtension` : écrire ses propres opérateurs (`horde.Affames()`, `horde.DansLeQuartier("Port")`) et une propriété d'extension C# 14
- 🔮 **Preview** : **dictionary expressions** et arguments de collection expressions (`[with(comparer), ..]`) ; pattern matching sur des unions dans une requête LINQ

### Séance 4 : Async & concurrence (niveau 4)
*Objectif : écrire du code qui monte en charge sur l'I/O sans bloquer de threads.*
- `Task`/`Task<T>` vs `CompletableFuture` ; `async/await` vs virtual threads (Loom)
- `Task.WhenAll`, `Task.WhenAny`, timeouts
- `CancellationToken` (pas d'équivalent direct en Java)
- `IAsyncEnumerable<T>` + `await foreach`
- Accès concurrents : `lock`, `Interlocked`, `SemaphoreSlim`, `Channel<T>` (aperçu)
- Pièges : `.Result`/`.Wait()`, `async void`
- **Koans** : `01_TaskBasics`, `02_WhenAll`, `03_Cancellation`, `04_AsyncStreams`, `05_RaceCondition` (un compteur faux à corriger), `06_ProducerConsumer`
- 🔮 **Preview** : **runtime async** (.NET 11, async géré par le runtime plutôt que par une state machine générée par le compilateur), avec une comparaison de l'IL et des stack traces avant/après

### Séance 5 : Capstone, modéliser un domaine en C#
*Objectif : réinvestir les 4 séances dans un domain model complet, piloté par les tests.*
- **Domaine retenu : 🧟 Food-Truck Zombie.** Pendant l'apocalypse, un food truck vend des cerveaux à la horde.
- Briques attendues :
  - Value objects en `record` avec validation : `Fraîcheur` (décroît avec le temps, via immutabilité + `with`), `Prix` (en dents ? en doigts ?), `Quantité`
  - Agrégat `Commande` avec invariants (constructeur privé + factory `Create`), règles métier comme « les végan-zombies mangent du chou-fleur » ou « pas de cerveau périmé de plus de 3 jours »
  - Types de zombies et états de commande modélisés par une hiérarchie `abstract record` + pattern matching exhaustif (`Passée → EnPréparation → Servie | Dévorée(parLeCuisinier)`)
  - `Result<T>` explicite pour `RuptureDeStock` ou `ClientEncoreVivant` au lieu d'exceptions
  - Requêtes LINQ : « le cerveau le plus populaire », « le chiffre d'affaires par type de zombie »
  - Une horde qui passe des commandes concurrentes : race condition sur le stock à corriger (`lock` / `SemaphoreSlim`), repository `async` en mémoire et file de commandes en `IAsyncEnumerable`
- 🔮 **Preview, final** : refactorer `Result<T>` et les états de `Commande` en **union types** et **closed** hierarchies, puis comparer le code avant/après
- Déroulé : user stories données sous forme de tests (activés au fur et à mesure), en mob ou en binôme
- Clôture : rétro, plus la carte du Maturity Stack (niveaux 5 à 7) pour la suite

AJOUTER les monads, Functional Core - Imperative Shell à ce dernier moment

## Structure du repo à produire
```
csharp-101/
├── README.md                 # setup, déroulé, liens slides
├── csharp-101.slnx
├── s1-fundamentals/          # projet xUnit, 1 fichier de koans par thème
├── s2-types/
├── s3-linq/                  # + Domain.cs (le registre de la horde)
├── s4-async/
├── s5-domain-model/
├── solutions/                # ou branche git `solutions`
└── slides/                   # deck par séance (Java ↔ C#)
```
- Conventions des koans : `[Fact]` avec `___` à remplacer (le placeholder ne compile pas, ou `throw new NotImplementedException()`), un commentaire `// Java:` qui montre l'équivalent, et des noms de tests rédigés en phrases.
- Stack : **.NET 10 (LTS) / C# 14**, xUnit v3, FluentAssertions ou Shouldly (à choisir), avec Rider, VS Code + C# Dev Kit ou Visual Studio.

## Points à arbitrer (pour l'itération)
1. La séance 1 dure plus longtemps à cause de l'histoire et de la compilation : faut-il réduire les koans à 40 min ?
2. Les slides : deck dédié par séance, ou un deck unique façon fsharp.pdf ?
3. Les placeholders : du code qui ne compile pas (« fill the blank ») ou des tests en rouge à l'exécution ?
4. Faut-il prévoir des exercices bonus pour les plus rapides dans chaque séance ?

## Vérification
- `dotnet test` sur la branche/dossier `solutions` : tout est vert.
- `dotnet test` sur les exercices : tout est rouge, avec des messages explicites.
- Dry-run chronométré de chaque séance (environ 50 min de koans pour un dev Java).

## Avancement & décisions
- [x] **Séance 1** : 53 koans en 5 fichiers, slides Slidev `slides/s1-fundamentals.md`, démos `hello.cs` et `compilation`.
- [x] **Séance 2** : 70 koans en 8 fichiers (`solutions/s2-types`), slides `slides/s2-types.md`, sans démo ni slide preview (comme en séance 1).
- [ ] Séance 3 · [ ] Séance 4 · [ ] Séance 5

Décisions prises pendant la séance 1 :
- **Placeholders** : tests rouges à l'exécution (le code compile toujours). `___` est une propriété qui lève `BlankNotFilledException`, pour que le message d'échec ne souffle pas la réponse.
- **Source de vérité** : `solutions/`, avec les marqueurs `/*__*/…/*__*/` et `// >>> SOLUTION … // <<< SOLUTION`. Les exercices sont générés par `tools/generate-exercises.py`.
- **Stack** : xUnit v3 + **NFluent** pour les assertions (`Check.That(…).IsEqualTo(…)`, `Check.ThatCode(…).Throws<T>()`) sur Microsoft.Testing.Platform (activé dans `global.json`), avec gestion centralisée des versions NuGet.
- **Slides** : Slidev, thème maison inspiré de l'affiche de *Shaun of the Dead* (`theme.jpg`) : blanc, noir, rouge sang `#be1e2d` ; Inter + JetBrains Mono (Bebas Neue pour les surtitres) ; titres alignés à gauche avec un trait rouge ; couverture et sections avec du sang qui coule (`components/Drip.vue`), silhouettes en aplat rouge : main et cerveau de game-icons, zombie de fluent-emoji-high-contrast (`<Zombie kind="hand" pos="br" />`) ; colonnes Java | C# avec logos et sections `// EN MAJUSCULES` comme fp101 ; notes d'animation dans les notes présentateur. La connexion devient « Qu'est-ce que C# m'inspire ? » (post-its), et une partie « Anatomie d'un test » compare JUnit 5 + AssertJ et xUnit + NFluent sur un même test. Pas d'emojis dans le texte des slides.
- **Timing de la séance 1** : 40 min de koans, pour faire de la place à l'histoire et à la compilation.
- **Previews (octobre 2026)** : les union types sont en preview dans .NET 11. Les closed enums et les closed hierarchies ne sont encore que des *propositions*, donc présentées en teaser sans code compilé.
