# Séance 3 : LINQ & principes fonctionnels 🧟

## Pré-requis
- [SDK .NET 10](https://dotnet.microsoft.com/download) : `dotnet --version` doit afficher `10.x`
- Un IDE : Rider, VS Code + C# Dev Kit, ou Visual Studio

## Le fil rouge : le recensement de la horde
Le QG des survivants a récupéré le registre de la horde : des zombies, des survivants, des camps et les rencontres des derniers jours. Tout est dans `Koans/Domain.cs`, sous forme de `record` (séance 2) rangés dans une classe `static Registre`.

## Les koans
Même principe que les séances précédentes : chaque koan est un test **rouge** à faire passer au **vert**.

```bash
cd s3-linq/Koans
dotnet test
```

- **`___`** : remplacez le blanc par la valeur attendue, après l'avoir **prédite**.
- **`throw new NotImplementedException("🧟 À toi de jouer !")`** (sous un commentaire 🛠) : écrivez l'implémentation.

| Fichier | Thème |
|---|---|
| `01_FuncEtAction.cs` | `Func`, `Action`, `Predicate` vs interfaces fonctionnelles Java, method groups, closures, fonctions qui renvoient des fonctions |
| `02_PrincipesFonctionnels.cs` | fonctions pures, immuabilité (`with`, `ImmutableList`), `Aggregate`, curryfication, composition |
| `03_Collections.cs` | `List`, `Dictionary` (`TryGetValue`, `GetValueOrDefault`), `HashSet`, `Queue` |
| `04_LeRecensement.cs` | **12 questions** sur le registre : `Where`, `Select`, `OrderBy`, `GroupBy`, `CountBy`, `SelectMany`, `Join`, `Zip`, `Chunk`, `MaxBy`... |
| `05_ExecutionDifferee.cs` | quand une requête s'exécute vraiment, double énumération |
| `06_Yield.cs` | `yield return`, séquences infinies, `TakeWhile`, `yield break` |
| `07_MethodesDExtension.cs` | ses propres opérateurs, propriétés d'extension C# 14, réécrire `Where` et `Select` |

💡 Astuce : lancez un seul fichier avec
`dotnet test -- --filter-class "Fonctionnel.LeRecensement"`

Pas le temps de tout finir ? Priorité à `01_FuncEtAction.cs`, `04_LeRecensement.cs` et `05_ExecutionDifferee.cs`.
