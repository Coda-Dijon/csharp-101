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
- `record` / `record struct` vs `record` Java 16+ ; `with`, égalité structurelle
- `struct` vs `class` (valeur vs référence), un point absent de Java
- Interfaces (default methods), héritage, `sealed`, `abstract`
- **Classes `static`** vs classe utilitaire Java (`final` + constructeur privé) : ni instanciables ni héritables, membres tous `static`, constructeur statique, `using static` (≈ `import static`) ; pas de static nested au sens Java (une classe imbriquée C# n'a jamais accès implicite à l'instance englobante). Ouvre la porte aux méthodes d'extension de la séance 3
- Generics réifiés (≠ type erasure Java), contraintes `where T :`
- Pattern matching : `is`, propriétés, positionnel, listes `[1, .., var last]`
- Composition plutôt qu'héritage
- **Koans** : `01_Properties`, `02_Records`, `03_StructVsClass`, `04_Interfaces`, `05_StaticClasses`, `06_Generics`, `07_PatternMatching`
- 🔮 **Preview** : **union types** (`union Shape(Circle, Square)`) et **closed classes** (hiérarchies fermées, exhaustivité vérifiée par le compilateur), comparés aux `sealed interface ... permits` de Java 17

### Séance 3 : Collections & LINQ (niveau 3)
*Objectif : transformer de la donnée brute en requêtes claires et composables.*
- `List<T>`, `Dictionary<K,V>`, `HashSet<T>`, `IReadOnlyList<T>`, collection expressions `[..]`
- Lambdas, `Func<>`/`Action<>` vs interfaces fonctionnelles Java
- LINQ vs Streams : `Where/Select/GroupBy/Any/All/Aggregate/OrderBy/SelectMany`
- **Exécution différée** et `IEnumerable<T>`, `yield return` (piège de la double énumération)
- Méthodes d'extension (`this` dans une classe `static`, vue en séance 2) et extension members C# 14
- `IQueryable` en un mot (pont vers EF Core)
- **Koans** : reprise du domaine Person/Pet/Park de fp101 (questions 1 à 11, `Domain.kt` → records C#), plus `DeferredExecution`, `Yield`, `ExtensionMethods`
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
├── s3-linq/                  # + Domain.cs (Person/Pet/Park)
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
- [ ] Séance 2 · [ ] Séance 3 · [ ] Séance 4 · [ ] Séance 5

Décisions prises pendant la séance 1 :
- **Placeholders** : tests rouges à l'exécution (le code compile toujours). `___` est une propriété qui lève `BlankNotFilledException`, pour que le message d'échec ne souffle pas la réponse.
- **Source de vérité** : `solutions/`, avec les marqueurs `/*__*/…/*__*/` et `// >>> SOLUTION … // <<< SOLUTION`. Les exercices sont générés par `tools/generate-exercises.py`.
- **Stack** : xUnit v3 + **NFluent** pour les assertions (`Check.That(…).IsEqualTo(…)`, `Check.ThatCode(…).Throws<T>()`) sur Microsoft.Testing.Platform (activé dans `global.json`), avec gestion centralisée des versions NuGet.
- **Slides** : Slidev, thème maison inspiré de l'affiche de *Shaun of the Dead* (`theme.jpg`) : blanc, noir, rouge sang `#be1e2d` ; Inter + JetBrains Mono (Bebas Neue pour les surtitres) ; titres alignés à gauche avec un trait rouge ; couverture et sections avec du sang qui coule (`components/Drip.vue`), silhouettes en aplat rouge : main et cerveau de game-icons, zombie de fluent-emoji-high-contrast (`<Zombie kind="hand" pos="br" />`) ; colonnes Java | C# avec logos et sections `// EN MAJUSCULES` comme fp101 ; notes d'animation dans les notes présentateur. La connexion devient « Qu'est-ce que C# m'inspire ? » (post-its), et une partie « Anatomie d'un test » compare JUnit 5 + AssertJ et xUnit + NFluent sur un même test. Pas d'emojis dans le texte des slides.
- **Timing de la séance 1** : 40 min de koans, pour faire de la place à l'histoire et à la compilation.
- **Previews (octobre 2026)** : les union types sont en preview dans .NET 11. Les closed enums et les closed hierarchies ne sont encore que des *propositions*, donc présentées en teaser sans code compilé.
