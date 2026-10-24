# Séance 4 : Async & concurrence 🧟

## Pré-requis
- [SDK .NET 10](https://dotnet.microsoft.com/download) : `dotnet --version` doit afficher `10.x`
- Un IDE : Rider, VS Code + C# Dev Kit, ou Visual Studio
- **Docker**, démarré : les koans lancent un vrai PostgreSQL avec [Testcontainers](https://dotnet.testcontainers.org/).
  Pour ne pas attendre le téléchargement pendant la séance : `docker pull postgres:17-alpine`

## Le fil rouge : le Winchester, food truck pour zombies
Un food truck qui vend des cerveaux à la horde (le domaine complet arrive en séance 5). Ses données sont dans PostgreSQL (`Koans/sql/init.sql`) : 5 plats (cerveaux et chou-fleur), 6 clients (dont Shaun, qui imite très bien les zombies) et 7 commandes. Le fichier `Koans/donnees/arrivage.csv` décrit la dernière livraison.

## Les koans
Même principe que les séances précédentes : chaque koan est un test **rouge** à faire passer au **vert**.

```bash
cd s4-async/Koans
dotnet test
```

Le premier lancement démarre PostgreSQL dans Docker (quelques secondes), puis la base est remise à zéro avant chaque test.

- **`___`** : remplacez le blanc par la valeur attendue, après l'avoir **prédite**.
- **`throw new NotImplementedException("🧟 À toi de jouer !")`** (sous un commentaire 🛠) : écrivez l'implémentation.

| Fichier | Thème |
|---|---|
| `01_LesFichiers.cs` | `Path`, `File`, lecture en streaming (`await foreach`), `await using`, `System.Text.Json` et ses pièges |
| `02_AsyncCommeDuSync.cs` | Npgsql en async : des requêtes enchaînées qui se lisent comme du code synchrone, `try/catch`, boucles, `Task.WhenAll` |
| `03_Dapper.cs` | `QueryAsync<T>`, paramètres et injection SQL, transactions |
| `04_LArrivage.cs` | un fichier, une base, un fichier : réceptionner une livraison et écrire l'inventaire |

💡 Astuce : lancez un seul fichier avec
`dotnet test -- --filter-class "Asynchrone.AProposDeLAsyncCommeDuSync"`

Pas le temps de tout finir ? Priorité à `02_AsyncCommeDuSync.cs`, puis `04_LArrivage.cs`.
