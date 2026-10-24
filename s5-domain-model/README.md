# Séance 5 : Le Food-Truck Zombie, un domain model en TDD 🧟

**Le but : mettre en pratique tout ce qu'on a vu en quatre séances**, en concevant un domain model complet,
piloté par les tests. Pas de koan cette fois : c'est vous qui écrivez les tests, puis le code qui les fait passer.

## Pré-requis
- [SDK .NET 10](https://dotnet.microsoft.com/download) : `dotnet --version` doit afficher `10.x`
- Un IDE : Rider, VS Code + C# Dev Kit, ou Visual Studio
- Pas de Docker aujourd'hui : tout le domaine se teste en mémoire

## Le domaine : le Winchester
Pendant l'apocalypse, le Winchester vend des cerveaux à la horde. Les prix sont en dents, les cerveaux ne se
gardent pas plus de 3 jours, les zombies vegans ne mangent que du chou-fleur, et Shaun, qui imite très bien
les zombies, respire encore. Le cuisinier, lui, a tendance à dévorer les commandes.

```
s5-domain-model/
├── Winchester/          # le domaine : un fichier par histoire, à écrire (✍️)
│   └── Result.cs        #   🎁 fourni : Result<T>, pour dire « non, et voici pourquoi » sans exception
└── Winchester.Tests/    # les histoires, une par fichier : à vous d'écrire les tests (🔴)
    └── Verifications.cs #   🎁 fourni : DoitReussir() et DoitEchouer() pour tester un Result<T>
```

## Le cycle
Chaque fichier de test raconte une **histoire** (une user story et ses exemples) et se termine par une **liste de tests**.
Pour chaque test de la liste :

1. 🔴 **Rouge** : écrivez le test. Il échoue, ou ne compile pas : c'est normal, c'est voulu.
2. 🟢 **Vert** : écrivez le **minimum** de code dans `Winchester/` pour le faire passer.
3. 🔵 **Refactor** : nettoyez le code et les tests, sans changer de couleur.

```bash
cd s5-domain-model/Winchester.Tests
dotnet test           # ou dotnet watch test, pour relancer à chaque sauvegarde
```

Le premier test de l'histoire 1 est donné, en commentaire : décommentez-le pour démarrer. Il ne compile pas,
`Dents` n'existe pas encore : c'est le premier rouge.
En binôme, essayez le **ping-pong** : l'un écrit le test, l'autre le fait passer, puis on échange.

## Les histoires, et ce qu'elles réinvestissent

| Histoire | Ce qu'on construit | Réinvestit |
|---|---|---|
| `01_LePrixEnDents.cs` | `Dents`, un value object créé par une factory method | S2 `record struct`, constructeur privé, opérateurs · `Result<T>` |
| `02_UnCerveauFrais.cs` | `Plat` et sa date de préparation | S2 `record`, `enum` · S1 `DateOnly` |
| `03_ChacunSonRegime.cs` | les clients et leur régime | S2 hiérarchie, pattern matching · S3 méthode d'extension |
| `04_PasserUneCommande.cs` | l'agrégat `Commande` et ses refus | S2 constructeur privé, factory, generics · `Result<T>` |
| `05_LaVieDUneCommande.cs` | les états d'une commande | S2 hiérarchie d'états, `with` · `Bind` |
| `06_LesComptesDuSoir.cs` | les statistiques de la soirée | S3 LINQ, fonctions pures |
| `07_LeRushDeMinuit.cs` | le comptoir, face à la horde | S4 `async`, `SemaphoreSlim`, `IAsyncEnumerable` |

Pas le temps de tout finir ? Les histoires **1 à 5** forment le cœur du domaine ; 6 et 7 sont des bonus.
La solution complète est dans `solutions/s5-domain-model`.
