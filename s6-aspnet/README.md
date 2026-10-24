# Séance 6 : L'écosystème & ASP.NET Core 🧟

## Pré-requis
- [SDK .NET 10](https://dotnet.microsoft.com/download) : `dotnet --version` doit afficher `10.x`
- Un IDE : Rider, VS Code + C# Dev Kit, ou Visual Studio
- **Docker**, démarré : les koans lancent un vrai PostgreSQL avec [Testcontainers](https://dotnet.testcontainers.org/).
  Pour ne pas attendre le téléchargement pendant la séance : `docker pull postgres:17-alpine`

## Le fil rouge : le Winchester, food truck pour zombies
Le domaine de la séance 5, en version simplifiée et stocké dans PostgreSQL comme en séance 4 : des plats (cerveaux et chou-fleur), des clients (dont Shaun, qui imite très bien les zombies) et des commandes.

```
s6-aspnet/
├── FoodTruck/          # l'API, en architecture hexagonale
│   ├── Domaine/        #   le modèle, Cuisine.Decider et les ports (interfaces des repositories)
│   ├── Application/    #   le cas d'usage PrendreUneCommande
│   ├── Infrastructure/ #   les adaptateurs PostgreSQL (Dapper)
│   ├── Program.cs      #   le branchement des ports (avec leurs durées de vie) et les routes de la Minimal API
│   └── sql/init.sql    #   le schéma et les données
├── Koans/              # les tests : injection de dépendances, API
└── compose.yaml        # pour la démo : PostgreSQL + le dashboard Aspire
```

## Les koans
Même principe que les séances précédentes : chaque koan est un test **rouge** à faire passer au **vert**.

```bash
cd s6-aspnet/Koans
dotnet test
```

- **`___`** : remplacez le blanc par la valeur attendue, après l'avoir **prédite**.
- **`throw new NotImplementedException("🧟 À toi de jouer !")`** (sous un commentaire 🛠) : écrivez l'implémentation. Pour les fichiers 02 à 04, une partie du code à écrire est dans le projet `FoodTruck`.

| Fichier | Thème |
|---|---|
| `01_InjectionDeDependances.cs` | `AddSingleton` / `AddScoped` / `AddTransient`, la captive dependency, `IOptions<T>`, les durées de vie du FoodTruck |
| `02_MinimalApi.cs` | l'API démarrée en mémoire avec `WebApplicationFactory`, routes, query string, OpenAPI |
| `03_ArchitectureHexagonale.cs` | ports et adaptateurs : un repository en mémoire, le cas d'usage testé sans base ni serveur |
| `04_ExposerLeFoodTruck.cs` | la règle métier (fonction pure), `POST /commandes`, `ProblemDetails`, cinq zombies pour trois cerveaux |

💡 Astuce : lancez un seul fichier avec
`dotnet test -- --filter-class "Ecosysteme.AProposDesMinimalApi"`

Pas le temps de tout finir ? Priorité à `03_ArchitectureHexagonale.cs` et `04_ExposerLeFoodTruck.cs`.

## La démo : l'API, Scalar et OpenTelemetry
```bash
docker compose up -d                 # PostgreSQL + dashboard Aspire
dotnet run --project FoodTruck       # http://localhost:5080/scalar pour explorer l'API
```
Le dashboard Aspire (http://localhost:18888) affiche les traces, métriques et logs envoyés par OpenTelemetry : chaque requête HTTP avec les requêtes SQL qu'elle a déclenchées. `docker compose down` pour tout arrêter.
