# Séance 2 : Types & design objet 🧟

## Pré-requis
- [SDK .NET 10](https://dotnet.microsoft.com/download) : `dotnet --version` doit afficher `10.x`
- Un IDE : Rider, VS Code + C# Dev Kit, ou Visual Studio

## Les koans
Même principe qu'en séance 1 : chaque koan est un test **rouge** à faire passer au **vert**.

```bash
cd s2-types/Koans
dotnet test
```

- **`___`** : remplacez le blanc par la valeur attendue, après l'avoir **prédite**.
- **`throw new NotImplementedException("🧟 À toi de jouer !")`** (sous un commentaire 🛠) : écrivez l'implémentation.

Les commentaires `// Java :` donnent l'équivalent Java, et les lignes `// ... -> ne compile pas` montrent ce que le compilateur refuse : essayez-les !

| Fichier | Thème |
|---|---|
| `01_Proprietes.cs` | propriétés, `init`, `required`, `field`, constructeurs primaires, indexeurs |
| `02_Encapsulation.cs` | `private`, `protected`, `internal`, `protected internal`, `private protected`, `file`, `sealed`, collections en lecture seule |
| `03_ValeurOuReference.cs` | `struct` vs `class`, paramètres `ref` et `out`, boxing, `readonly record struct`, surcharge d'opérateurs |
| `04_Heritage.cs` | interfaces, méthodes par défaut, `virtual`/`override`/`new`, `sealed`, `abstract`, composition |
| `05_Records.cs` | records, égalité structurelle, `with`, déconstruction, factory method plutôt qu'un constructeur qui lance, héritage de records |
| `06_ClassesStatiques.cs` | classes `static`, `using static`, constructeur statique, méthodes d'extension |
| `07_Generiques.cs` | generics réifiés, `typeof(T)`, contraintes `where`, variance, `INumber<T>` |
| `08_PatternMatching.cs` | patterns de type, de propriétés, positionnels, relationnels, de listes |
| `09_AToiDeModeliser.cs` | **exercice** : écrire soi-même un petit modèle (enum, struct, interface, classe abstraite, classes `sealed`) |

### L'exercice de modélisation

`09_AToiDeModeliser.cs` commence par un mini-énoncé : la carte de patrouille du bunker. Cette fois, pas de blanc : vous écrivez tous les types vous-même, dans ce fichier.
Ses tests sont désactivés tant que les types n'existent pas (sinon le projet ne compilerait plus). Quand vos types compilent, décommentez la ligne `#define MODELE_ECRIT` tout en haut du fichier : les tests s'activent.

💡 Astuce : lancez un seul fichier avec
`dotnet test -- --filter-class "Types.AProposDesRecords"`

Pas le temps de tout finir ? Priorité à `02_Encapsulation.cs`, `04_Heritage.cs`, `05_Records.cs` et `08_PatternMatching.cs` : ce sont les briques du domain model de la séance 5.
