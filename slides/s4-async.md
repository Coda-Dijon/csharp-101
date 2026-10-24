---
theme: default
title: "C# 101 · Séance 4 · Async & concurrence"
info: C# 101, pour développeurs Java. Séance 4 sur 6.
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

<div class="kicker mb-4">Séance 4 / 6</div>

# C# 101

## Async & concurrence

<Zombie kind="brain" pos="bl" :size="6" />
<Zombie kind="zombie" pos="br" :size="6" />

<!--
Séance 3 : LINQ et les séquences paresseuses (yield). Aujourd'hui : attendre sans bloquer.
On manipule de vraies entrées / sorties : des fichiers, et une base PostgreSQL démarrée par Testcontainers.
Fil rouge : le Winchester, un food truck pour zombies (le domaine complet arrive en séance 5).
Rappel logistique : Docker doit tourner sur les postes.
-->

---

# Au menu

| Temps | Phase |
|---|---|
| 0:00 | Combien de threads pour nourrir la horde ? |
| 0:10 | L'async avant `async` / `await` |
| 0:25 | Fichiers et base de données |
| 0:35 | Koans en binôme |
| 1:15 | Conclusions |

<Zombie kind="brain" pos="br" :size="5" />

---
layout: center
---

# Combien de threads pour nourrir la horde ?

<div class="text-center text-xl mt-4">

Le Winchester reçoit **1 000 commandes** en même temps.<br>
Chacune attend **200 ms** la réponse de PostgreSQL.

</div>

<div class="grid grid-cols-3 gap-4 mt-8 text-center">
  <div class="pill text-base">Combien de threads pour les servir toutes en même temps ?</div>
  <div class="pill text-base">Que fait un thread pendant qu'il attend la base ?</div>
  <div class="pill text-base">Comment feriez-vous en Java aujourd'hui ?</div>
</div>

<div v-click class="callout mt-8 text-center">

Avec du code bloquant : 1 000 threads qui attendent. Avec `await` : une poignée de threads du pool, rendus pendant chaque attente.

</div>

<Zombie kind="hand" pos="br" :size="4" />

<!--
Connexion (10 min), en binôme.
Réponses typiques côté Java : un pool de threads (et des requêtes qui patientent), CompletableFuture, WebFlux / R2DBC, et depuis Java 21 les virtual threads.
On garde la question « que fait le thread pendant l'attente ? » : toute la séance y répond.
-->
---
layout: section
---

<div class="kicker mb-3">Partie 01</div>

# L'async avant `async` / `await`

---
layout: two-cols
---

# Le scénario

```csharp
// LA VERSION SYNCHRONE : simple, mais bloquante
try
{
    var arrivage = File.ReadAllText("arrivage.csv");
    var plats = MettreAJourLeStock(arrivage);
    Console.WriteLine($"{plats} plats réapprovisionnés");
}
catch (IOException e)
{
    Console.WriteLine($"Livraison perdue : {e.Message}");
}
```

::right::

<div class="mt-20">

<v-clicks>

- Lire un fichier, puis écrire en base : deux **entrées / sorties**
- Pendant chacune, le thread **attend sans rien faire**
- Sur un serveur : une requête = un thread bloqué. 1 000 requêtes lentes = 1 000 threads
- Comment écrire la même chose **sans bloquer** ? Vingt ans d'histoire en quatre étapes

</v-clicks>

</div>

<!--
Le code est volontairement trivial : on va le réécrire avec chaque modèle asynchrone qu'a connu .NET.
Point clé : l'async ne sert pas à aller plus vite, il sert à ne pas immobiliser un thread pendant une attente.
-->

---

# 2002 · APM : `Begin` / `End` et callbacks

<div class="grid grid-cols-[3fr_2fr] gap-6">
<div>

```csharp {all|2-3|5|6-8|9-14|all}
var fichier = new FileStream("arrivage.csv", FileMode.Open,
    FileAccess.Read, FileShare.Read, 4096, useAsync: true);
var tampon = new byte[4096];
fichier.BeginRead(tampon, 0, tampon.Length, lecture =>
{
    var lus = fichier.EndRead(lecture);  // l'exception sort ICI
    var commande = new SqlCommand(MiseAJour(tampon, lus), connexion);
    commande.BeginExecuteNonQuery(ecriture =>
    {
        try { Console.WriteLine($"{commande.EndExecuteNonQuery(ecriture)} plats"); }
        catch (SqlException e) { /* sur un autre thread... */ }
        finally { fichier.Close(); connexion.Close(); }
    }, null);
}, null);
```

</div>
<div class="mt-2">

<v-clicks at="1">

- *Asynchronous Programming Model*, .NET 1.0
- `BeginXxx` démarre, `EndXxx` récupère le résultat
- Un callback par étape : la **pyramide**
- `try` / `finally` éclatés, ressources fermées à la main, le `catch` de l'appelant ne voit rien

</v-clicks>

</div>
</div>

<!--
IAsyncResult partout. Le try/catch qui entoure BeginRead n'attrape rien : l'erreur arrive plus tard, sur un thread du pool.
Ajouter une boucle (une ligne de CSV = une requête) devient un exercice de récursion.
-->

---

# 2005 · EAP : un événement `Completed`

<div class="grid grid-cols-[3fr_2fr] gap-6">
<div>

```csharp {all|1-2|3-11|12|all}
var web = new WebClient();
web.DownloadStringCompleted += (sender, e) =>
{
    if (e.Cancelled) return;
    if (e.Error != null)
    {
        Console.WriteLine($"Livraison perdue : {e.Error.Message}");
        return;
    }
    MettreAJourLeStockAsync(e.Result); // et rebelote : un autre événement
};
web.DownloadStringAsync(new Uri("https://fournisseur.example/arrivage.csv"));
```

</div>
<div class="mt-2">

<v-clicks at="1">

- *Event-based Asynchronous Pattern*, .NET 2.0
- On s'abonne **avant** de lancer
- Les erreurs arrivent dans `e.Error`, plus d'exception
- Pratique pour les interfaces graphiques (le callback revient sur le thread de l'UI), pénible à composer

</v-clicks>

</div>
</div>

<!--
EAP, c'est le modèle de WinForms et du BackgroundWorker. Le code reste découpé en morceaux, et enchaîner deux opérations EAP impose d'imbriquer les abonnements.
-->

---

# 2010 · TPL : des `Task` et `ContinueWith`

<div class="grid grid-cols-[3fr_2fr] gap-6">
<div>

```csharp {all|1|2-3|4-10|all}
LireArrivageAsync("arrivage.csv")                       // Task<string>
    .ContinueWith(lecture => MettreAJourAsync(lecture.Result))
    .Unwrap()                                           // Task<Task<int>> -> Task<int>
    .ContinueWith(ecriture =>
    {
        if (ecriture.IsFaulted)                         // AggregateException
            Console.WriteLine(ecriture.Exception!.InnerException!.Message);
        else
            Console.WriteLine($"{ecriture.Result} plats");
    });
```

</div>
<div class="mt-2">

<v-clicks at="1">

- *Task Parallel Library*, .NET 4.0
- Une `Task<T>` est une **valeur** : on peut la passer, la combiner (`WhenAll`)
- On chaîne au lieu d'imbriquer, comme `thenCompose` en Java
- Mais `Unwrap`, `.Result`, `AggregateException`... et toujours pas de `try` / `catch` ni de boucle

</v-clicks>

</div>
</div>

<!--
C'est exactement le niveau de CompletableFuture en Java 8 (2014) : thenApply, thenCompose, exceptionally.
La Task est la brique qui manquait : async/await sera construit dessus.
-->

---
layout: two-cols-header
---

# 2012 · `async` / `await`

::left::

<div class="kicker mb-2">Synchrone</div>

```csharp {all|4-5|all}{at:1}
void Receptionner(string chemin)
{
    try {
        var arrivage = File.ReadAllText(chemin);
        var plats = MettreAJour(arrivage);
        Console.WriteLine($"{plats} plats");
    }
    catch (IOException e) { Console.WriteLine(e.Message); }
}
```

::right::

<div class="kicker mb-2">Asynchrone</div>

```csharp {all|4-5|all}{at:1}
async Task ReceptionnerAsync(string chemin)
{
    try {
        var arrivage = await File.ReadAllTextAsync(chemin);
        var plats = await MettreAJourAsync(arrivage);
        Console.WriteLine($"{plats} plats");
    }
    catch (IOException e) { Console.WriteLine(e.Message); }
}
```

::bottom::

<div v-click="2" class="callout mt-2">

De l'async qui se lit comme du sync : même forme, mêmes `try` / `catch`, `using`, `foreach`, `if`. Pendant chaque `await`, le thread est **rendu** au pool : il sert d'autres requêtes.

</div>

<!--
C# 5, .NET 4.5 (Visual Studio 2012). Inspiré des async workflows de F# (2007).
Les différences : async, Task, await et le suffixe Async. C'est tout. L'exception remonte au await comme une exception synchrone.
C'est ce que les koans de 02_AsyncCommeDuSync font manipuler sur PostgreSQL.
-->

---

# Vingt ans d'asynchrone

<div class="grid grid-cols-4 gap-3 mt-6 text-center">
  <div class="callout"><div class="kicker">2002</div>APM<br><span class="small muted">Begin / End, callbacks</span></div>
  <div class="callout"><div class="kicker">2005</div>EAP<br><span class="small muted">événements Completed</span></div>
  <div class="callout"><div class="kicker">2010</div>TPL<br><span class="small muted">Task, ContinueWith</span></div>
  <div class="callout"><div class="kicker">2012</div><strong>async / await</strong><br><span class="small muted">C# 5</span></div>
</div>

<div v-click class="mt-8">

| Ailleurs | Année |
|---|---|
| F# async workflows (l'inspiration) | 2007 |
| Java 8 : `CompletableFuture` | 2014 |
| Python 3.5 : `async` / `await` | 2015 |
| JavaScript ES2017 : `async` / `await` | 2017 |
| Java 21 : virtual threads, du code bloquant sur des threads légers | 2023 |

</div>

<!--
async/await est l'un des exports les plus réussis de C# : JavaScript, Python, Rust, Swift, Kotlin (sous forme de coroutines) l'ont repris.
Java a choisi une autre voie avec Loom : garder le code bloquant, mais rendre les threads très légers. On compare sur la slide suivante.
-->

---
layout: two-cols-header
---

# Et en Java ?

::left::

<Lang name="java" />

```java {all|1-5|7-12|all}{at:1}
// JAVA 8 : CompletableFuture, on chaîne
lireArrivage(chemin)
    .thenCompose(a -> mettreAJourLeStock(a))
    .thenAccept(p -> System.out.println(p + " plats"))
    .exceptionally(e -> { log(e); return null; });

// JAVA 21 : on bloque... un thread virtuel, léger
Thread.startVirtualThread(() -> {
    var arrivage = Files.readString(chemin);
    var plats = mettreAJourLeStock(arrivage);
    System.out.println(plats + " plats");
});
```

::right::

<Lang name="csharp" />

```csharp {all|1-4|6-8|all}{at:1}
// C# : await, même structure que le synchrone
var arrivage = await File.ReadAllTextAsync(chemin);
var plats = await MettreAJourLeStockAsync(arrivage);
Console.WriteLine($"{plats} plats");

// le prix : « async » se propage dans toute la pile
// d'appels (la « couleur » des fonctions)
async Task<int> MettreAJourLeStockAsync(string a) { ... }
```

<!--
Deux philosophies : C# rend l'attente explicite (await, Task dans les signatures), Java 21 la cache derrière des threads virtuels.
L'inconvénient d'async/await : une méthode qui appelle une méthode async devient async à son tour (« What color is your function? », Bob Nystrom).
L'avantage : on voit où le code peut se suspendre, et Task.WhenAll compose des opérations parallèles très simplement.
-->

---
layout: two-cols
---

# Sous le capot

```csharp
// une machine à états générée par Roslyn (simplifiée)
struct ReceptionnerAsync_Etat : IAsyncStateMachine
{
    int etat; TaskAwaiter<string> attente;

    void MoveNext()
    {
        if (etat == 0) {
            attente = LireAsync(chemin).GetAwaiter();
            if (!attente.IsCompleted) {
                etat = 1;  // on reprendra ici
                builder.AwaitOnCompleted(ref attente,
                                         ref this);
                return;    // le thread est libéré
            }
        }
        var arrivage = attente.GetResult(); // ...
    }
}
```

::right::

<div class="mt-20">

<v-clicks>

- Comme `yield return` (séance 3) : le compilateur découpe la méthode en **étapes**
- Chaque `await` est un point de reprise
- Si la `Task` est déjà terminée, on continue **sans** rien suspendre
- Sinon, la suite s'enregistre comme callback... le code de 2002, écrit par le compilateur
- Async **≠** multithread : pendant l'attente d'une I/O, **aucun** thread ne travaille

</v-clicks>

</div>

<!--
Le vrai code généré est plus long (builder, gestion des exceptions, contexte de synchronisation). On peut le voir avec ILSpy, comme en séance 1.
La preview de la séance : le runtime async de .NET 11, qui déplace cette machine à états dans le runtime.
-->

---
layout: section
---

<div class="kicker mb-3">Partie 02</div>

# Fichiers et base de données

---
layout: two-cols-header
---

# Les fichiers

::left::

<Lang name="java" />

```java {all|1-3|5-7|9-11|all}{at:1}
// TOUT LE FICHIER
var texte = Files.readString(Path.of("cri.txt"));
Files.writeString(Path.of("cri.txt"), "Braaains");

// LIGNE PAR LIGNE : un Stream à fermer
try (var lignes = Files.lines(chemin)) {
    lignes.skip(1).forEach(this::livrer); }

// JSON : Jackson, une dépendance
var mapper = new ObjectMapper();
mapper.writeValue(new File("inventaire.json"), stock);
```

::right::

<Lang name="csharp" />

```csharp {all|1-3|5-7|9-11|all}{at:1}
// TOUT LE FICHIER
var texte = await File.ReadAllTextAsync("cri.txt");
await File.WriteAllTextAsync("cri.txt", "Braaains");

// LIGNE PAR LIGNE : un IAsyncEnumerable
await foreach (var ligne in File.ReadLinesAsync(chemin))
    Livrer(ligne);

// JSON : System.Text.Json, intégré au framework
await using var flux = File.Create("inventaire.json");
await JsonSerializer.SerializeAsync(flux, stock);
```

::bottom::

<div v-click="4" class="callout small mt-2">

`await using` ≈ try-with-resources, en asynchrone. Pièges de `System.Text.Json` : PascalCase par défaut (`JsonSerializerOptions.Web` pour le camelCase), accents échappés, propriétés inconnues ignorées.

</div>

---
layout: two-cols-header
---

# La base de données

::left::

<Lang name="java" />

```java {all|1-6|8-10|all}{at:1}
// JDBC : bloquant
try (var c = dataSource.getConnection();
     var ps = c.prepareStatement(SQL)) {
    ps.setString(1, "Cerveau");
    var rs = ps.executeQuery();
    while (rs.next()) noms.add(rs.getString(1));
}

// JDBCTEMPLATE (Spring)
jdbcTemplate.query("SELECT ... WHERE id = ?",
    (rs, i) -> new Plat(rs.getInt("id"), ...), id);
```

::right::

<Lang name="csharp" />

```csharp {all|1-6|8-10|all}{at:1}
// ADO.NET (Npgsql) : async de bout en bout
await using var cmd = source.CreateCommand(SQL);
cmd.Parameters.AddWithValue("Cerveau");

await using var lu = await cmd.ExecuteReaderAsync();
while (await lu.ReadAsync()) noms.Add(lu.GetString(0));


// DAPPER : des méthodes d'extension sur la connexion
var plat = await connexion.QuerySingleAsync<Plat>(
    "SELECT ... WHERE id = @id", new { id });
```

::bottom::

<div v-click="3" class="callout small mt-2">

ORM complet : **Entity Framework Core** (≈ JPA / Hibernate), avec LINQ traduit en SQL grâce à `IQueryable` (séance 3).

</div>

<!--
Npgsql est le driver PostgreSQL de .NET, maintenu en open source (et largement contribué par Microsoft).
Les colonnes de JDBC commencent à 1, celles d'ADO.NET à 0 : un classique.
Dapper vient de Stack Overflow, qui l'a écrit pour ses propres besoins de performance.
-->

---
layout: two-cols-header
---

# Testcontainers : la même bibliothèque

::left::

<Lang name="java" />

```java
@Testcontainers
class StockTest {
    @Container
    static PostgreSQLContainer<?> postgres =
        new PostgreSQLContainer<>("postgres:17-alpine");

    @Test
    void lit_le_stock() {
        var url = postgres.getJdbcUrl();
        ...
    }
}
```

::right::

<Lang name="csharp" />

```csharp
public sealed class BaseDeDonnees : IAsyncLifetime
{
    readonly PostgreSqlContainer _conteneur =
        new PostgreSqlBuilder("postgres:17-alpine")
            .Build();

    public async ValueTask InitializeAsync()
        => await _conteneur.StartAsync();

    public string ChaineDeConnexion
        => _conteneur.GetConnectionString();
}
```

::bottom::

<div class="callout small mt-2">

Un vrai PostgreSQL dans Docker, démarré pour les tests et jeté ensuite. Dans les koans, il est partagé par une *collection fixture* xUnit, et la base est réinitialisée avant chaque test.

</div>

---

# Les différences qui piquent

| Java | C# | Piège |
|---|---|---|
| `CompletableFuture` | `Task` | `await` « déballe » la `Task` ; sans `await`, on n'a qu'une promesse |
| virtual threads | `async` / `await` | `async` se propage dans toute la pile d'appels |
| `future.get()` | `.Result` / `.Wait()` | bloque le thread : on attend avec `await` |
| try-with-resources | `using` / `await using` | `StreamWriter` n'écrit rien avant `Dispose` |
| `rs.getString(1)` | `lecteur.GetString(0)` | les colonnes ADO.NET commencent à 0 |
| Jackson, camelCase | `System.Text.Json`, PascalCase | `JsonSerializerOptions.Web` |
| une `Connection` par thread | une connexion = une requête à la fois | `Task.WhenAll` : une connexion chacune (`DataSource`) |

<Zombie kind="eye" pos="br" :size="3.5" />

---
layout: section
---

<div class="kicker mb-3">Partie 03</div>

# À vous de jouer

<Zombie pos="br" :size="8" />

---

# Le Winchester

<div class="grid grid-cols-2 gap-8">
<div>

```sql
plats     (id, nom, categorie, prix_en_dents, stock)
clients   (id, nom, type, est_vivant)
commandes (id, client_id, plat_id, quantite, passee_le)
```

- 5 plats : des cerveaux, du chou-fleur, et un Tartare de neurones en rupture
- 6 clients, dont deux vegans, et Shaun, qui imite très bien les zombies
- 7 commandes, 174 dents de chiffre d'affaires

</div>
<div>

```
s4-async/Koans/
├── sql/init.sql        les tables et les données
├── donnees/
│   └── arrivage.csv    la dernière livraison
└── 01_ ... 04_*.cs     les koans
```

<div class="callout mt-4">

**Docker doit tourner.** Le premier `dotnet test` démarre PostgreSQL (quelques secondes), puis la base est remise à zéro avant chaque test.

</div>

</div>
</div>

---

# 40 minutes, en binôme

| Fichier | Thème |
|---|---|
| `01_LesFichiers.cs` | `Path`, `File`, `await foreach`, `await using`, `System.Text.Json` |
| `02_AsyncCommeDuSync.cs` | **des requêtes enchaînées qui se lisent comme du code synchrone** |
| `03_Dapper.cs` | `QueryAsync<T>`, injection SQL, transactions |
| `04_LArrivage.cs` | un fichier, une base, un fichier |

<div class="callout mt-6">

On change de pilote à chaque fichier · priorité à **02**, puis **04**

</div>

<Zombie kind="hand" pos="br" :size="4" />

<!--
Pièges habituels : count(*) renvoie un long (02), une connexion ne traite qu'une requête à la fois (02), l'injection SQL par concaténation (03), SortedDictionary pour trier l'inventaire (04).
-->

---

# Conclusions

<v-clicks>

- Correction live de `FicheAsync` : trois requêtes, zéro callback
- **Retour à la connexion** : que fait le thread pendant l'attente ?
- **Tour de table** : quel `CompletableFuture` de votre code Java deviendrait plus simple avec `await` ?

</v-clicks>

---
layout: end
---

<div class="text-8xl mb-4"><game-icons-brain class="inline" /></div>

# Braaains !

<div class="kicker mt-2">Prochaine séance · Le Food-Truck Zombie</div>
