# Séance 1 : Fondamentaux

## Pré-requis
- [SDK .NET 10](https://dotnet.microsoft.com/download) : `dotnet --version` doit afficher `10.x`
- Un IDE : Rider, VS Code + C# Dev Kit, ou Visual Studio

## Les koans
Un koan est un petit test **rouge**. Votre mission : le faire passer au **vert**.

```bash
cd s1-fundamentals/Koans
dotnet test
```

Deux types d'exercices :
- **`___`** : remplacez le blanc par la valeur attendue. Essayez de **prédire** la valeur avant de lancer le test : le message d'erreur ne vous la donnera pas 😉
- **`throw new NotImplementedException("🧟 À toi de jouer !")`** (sous un commentaire 🛠) : écrivez l'implémentation.

Les assertions utilisent [NFluent](https://www.n-fluent.net/), qui se lit comme une phrase :

```csharp
Check.That(horde.Length).IsEqualTo(3);
Check.ThatCode(() => 42 / zero).Throws<DivideByZeroException>();
```

Les commentaires `// Java :` montrent l'équivalent Java pour vous raccrocher à ce que vous connaissez.

| Fichier | Thème |
|---|---|
| `01_Syntaxe.cs` | var, types, interpolation, méthodes, tuples, tableaux |
| `02_Chaines.cs` | égalité, immutabilité, ranges, StringBuilder |
| `03_ControleEtEnums.cs` | boucles, switch, switch expressions, enums |
| `04_Nullabilite.cs` | `?`, `?.`, `??`, `??=`, `Nullable<T>` |
| `05_Exceptions.cs` | try/catch/finally, filtres `when`, `using`, `checked` |

💡 Astuce : lancez un seul fichier avec
`dotnet test -- --filter-class "Fundamentals.AProposDeLaSyntaxe"`

## Démos
- `demos/hello.cs` : une app C# sans projet → `dotnet run demos/hello.cs`
- `demos/compilation` : voir l'IL généré avec ILSpy, sharplab.io ou le IL Viewer de Rider (instructions dans `Program.cs`)
