// Construit le site GitHub Pages : un dossier par deck Slidev + une page d'accueil pour choisir la séance.
//   BASE=/csharp-101/ node scripts/build-site.mjs   (BASE = chemin du site, "/" par défaut)
import { execFileSync } from 'node:child_process'
import { existsSync, mkdirSync, rmSync, writeFileSync } from 'node:fs'
import { join } from 'node:path'

const base = (process.env.BASE ?? '/').replace(/\/?$/, '/')
const out = 'dist'

// Le programme complet : une séance sans fichier de slides apparaît « à venir »
// goal et objectives alimentent la section « Plan de cours » de la page d'accueil
const sessions = [
  { slug: 's1-fundamentals', n: 1, title: 'Fondamentaux', topics: 'Histoire de .NET, compilation, syntaxe Java ↔ C#, anatomie d\'un test',
    goal: "Écrire, lire et déboguer du C# en confiance.",
    objectives: [
      "Situer .NET et C# dans leur histoire, face à Java",
      "Expliquer le chemin du code : Roslyn, IL, assembly, CLR et JIT (≈ bytecode, jar, JVM)",
      "Utiliser la toolchain <code>dotnet</code> : <code>new</code>, <code>build</code>, <code>test</code>, <code>run</code>",
      "Écrire la syntaxe de base : <code>var</code>, interpolation, <code>switch</code> expression, paramètres nommés",
      "Manipuler les chaînes, les enums et les exceptions",
      "Se protéger des <code>null</code> avec les types nullables, <code>?.</code> et <code>??</code>",
    ] },
  { slug: 's2-types', n: 2, title: 'Types & design objet', topics: 'Propriétés, encapsulation, struct vs class, ref/out, opérateurs, héritage, records, generics, pattern matching',
    goal: "Modéliser avec des types qui disent ce qu'ils veulent dire.",
    objectives: [
      "Remplacer getters et setters par des propriétés (<code>get</code>, <code>init</code>)",
      "Choisir le bon niveau d'encapsulation (<code>private</code>, <code>internal</code>, <code>sealed</code>…)",
      "Distinguer <code>struct</code> et <code>class</code>, valeur et référence, et passer par <code>ref</code> / <code>out</code>",
      "Surcharger les opérateurs d'un type valeur (<code>+</code>, <code>&lt;</code>…), et savoir quand s'en abstenir",
      "Rendre visible une construction qui peut échouer avec une factory method",
      "Utiliser héritage, interfaces, records, classes <code>static</code> et generics réifiés",
      "Décomposer des données avec le pattern matching",
      "Concevoir soi-même un petit modèle objet",
    ] },
  { slug: 's3-linq', n: 3, title: 'LINQ & principes fonctionnels', topics: 'Func vs Action, lambdas, fonctions pures, LINQ vs Streams sur une horde de zombies, exécution différée',
    goal: "Transformer de la donnée en requêtes claires et composables.",
    objectives: [
      "Manipuler les fonctions comme des valeurs : <code>Func</code>, <code>Action</code>, lambdas, closures",
      "Appliquer les principes fonctionnels : fonctions pures, immuabilité, composition",
      "Choisir la bonne collection : <code>List</code>, <code>Dictionary</code>, <code>HashSet</code>",
      "Traduire des Streams Java en requêtes LINQ, du <code>Where</code> au <code>GroupBy</code>",
      "Comprendre l'exécution différée et générer des séquences avec <code>yield return</code>",
      "Écrire ses propres opérateurs avec les méthodes d'extension",
    ] },
  { slug: 's4-async', n: 4, title: 'Async & concurrence', topics: 'L\'async avant async/await, Task, fichiers et PostgreSQL en async avec Testcontainers, Dapper',
    goal: "Écrire du code qui monte en charge sur l'I/O sans bloquer de threads.",
    objectives: [
      "Distinguer process, thread, ThreadPool et <code>Task</code>",
      "Comprendre ce qu'<code>async</code> / <code>await</code> a remplacé (callbacks, événements, <code>ContinueWith</code>)",
      "Lire et écrire des fichiers en async, sérialiser en JSON",
      "Interroger PostgreSQL en async avec Npgsql et Dapper, testé avec Testcontainers",
      "Annuler une opération avec <code>CancellationToken</code>",
      "Protéger un accès concurrent et faire communiquer producteur et consommateur",
    ] },
  { slug: 's5-domain-model', n: 5, title: 'Le Food-Truck Zombie', topics: 'Concevoir un domain model en TDD en réinvestissant les séances 1 à 4 : value objects, Result<T>, états, LINQ, concurrence',
    goal: "Réinvestir les séances 1 à 4 en concevant un domain model en TDD.",
    objectives: [
      "Piloter le design par les tests : liste de tests, rouge, vert, refactor",
      "Modéliser des value objects et un agrégat qui protègent leurs invariants",
      "Rendre les états illégaux irreprésentables",
      "Signaler les refus métier avec <code>Result&lt;T&gt;</code> plutôt qu'avec des exceptions",
      "Interroger le domaine avec LINQ et gérer la concurrence d'un rush",
    ] },
  { slug: 's6-aspnet', n: 6, title: 'L\'écosystème & ASP.NET Core', topics: 'Minimal API sur le Food-Truck Zombie, injection de dépendances, NuGet, OpenAPI avec Scalar, OpenTelemetry',
    goal: "Exposer le domaine en API HTTP et découvrir l'écosystème .NET.",
    objectives: [
      "Faire le parallèle entre ASP.NET Core et Spring Boot",
      "Configurer l'injection de dépendances et choisir la durée de vie des services",
      "Écrire une Minimal API et la tester avec <code>WebApplicationFactory</code>",
      "Traduire les erreurs métier en réponses HTTP (<code>ProblemDetails</code>)",
      "Structurer une application en architecture hexagonale (ports, adaptateurs, repositories)",
      "S'orienter dans NuGet : OpenAPI, OpenTelemetry, EF Core, Polly… et vérifier les licences",
    ] },
]

rmSync(out, { recursive: true, force: true })
mkdirSync(out, { recursive: true })

for (const s of sessions) {
  s.available = existsSync(`${s.slug}.md`)
  if (!s.available) continue
  console.log(`▶ ${s.slug}`)
  execFileSync('npx', ['slidev', 'build', `${s.slug}.md`, '--base', `${base}${s.slug}/`, '--out', join(out, s.slug)], { stdio: 'inherit' })
}

const plan = s => `<li class="session" id="seance-${s.n}">
      <span class="num">0${s.n}</span>
      <div>
        <h3>${s.title}</h3>
        <p class="goal">${s.goal}</p>
        <ul>${s.objectives.map(o => `<li>${o}</li>`).join('')}</ul>
      </div>
    </li>`

const card = s => s.available
  ? `<a class="card" href="${s.slug}/"><span class="num">0${s.n}</span><h2>${s.title}</h2><p>${s.topics}</p><span class="go">Voir les slides →</span></a>`
  : `<div class="card soon"><span class="num">0${s.n}</span><h2>${s.title}</h2><p>${s.topics}</p><span class="go">À venir</span></div>`

writeFileSync(join(out, 'index.html'), `<!doctype html>
<html lang="fr">
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<title>C# 101</title>
<link rel="preconnect" href="https://fonts.googleapis.com">
<link href="https://fonts.googleapis.com/css2?family=Bebas+Neue&family=Inter:wght@400;600;800;900&display=swap" rel="stylesheet">
<style>
  :root { --blood: #be1e2d; --blood-soft: #fbeaec; --ink: #141414; --muted: #8a8a8a; --line: #e7e7e7; }
  * { box-sizing: border-box; }
  body { margin: 0; font-family: Inter, sans-serif; color: var(--ink); background: #fff; }
  .drip { display: block; width: 100%; height: 90px; fill: var(--blood); }
  main { max-width: 1040px; margin: 0 auto; padding: 1rem 1rem 4rem; }
  .kicker { font-family: 'Bebas Neue', sans-serif; letter-spacing: .18em; font-size: 1.2rem; }
  h1 { font-size: clamp(3rem, 10vw, 5.5rem); font-weight: 900; letter-spacing: -.05em; margin: .2rem 0 .5rem; }
  .lead { color: var(--muted); font-size: 1.15rem; margin: 0 0 2.5rem; }
  .lead a { color: var(--blood); font-weight: 600; text-decoration: none; }
  .grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(280px, 1fr)); gap: 1rem; }
  .card { display: flex; flex-direction: column; gap: .4rem; padding: 1.3rem; border: 1px solid var(--line);
          border-radius: 14px; color: inherit; text-decoration: none; transition: transform .15s, border-color .15s; }
  a.card:hover { transform: translateY(-3px); border-color: var(--blood); }
  .num { font-family: 'Bebas Neue', sans-serif; font-size: 1.6rem; color: var(--blood); letter-spacing: .08em; }
  .card h2 { margin: 0; font-size: 1.35rem; font-weight: 800; letter-spacing: -.02em; }
  .card p { margin: 0; color: #555; line-height: 1.5; flex: 1; }
  .go { margin-top: .6rem; font-weight: 600; color: var(--blood); }
  .soon { opacity: .45; }
  .soon .go { color: var(--muted); }
  footer { margin-top: 3rem; color: var(--muted); font-size: .85rem; }
  footer a { color: var(--blood); }
  .plan { margin-top: 4rem; }
  .plan > h2 { font-size: 2rem; font-weight: 900; letter-spacing: -.03em; margin: 0 0 .4rem; }
  .plan > p { color: var(--muted); margin: 0 0 1.5rem; }
  .plan ol { list-style: none; margin: 0; padding: 0; }
  .session { display: grid; grid-template-columns: 3rem 1fr; gap: .5rem; padding: 1.3rem 0; border-top: 1px solid var(--line); }
  .session h3 { margin: .15rem 0 .2rem; font-size: 1.2rem; font-weight: 800; }
  .session .goal { margin: 0 0 .5rem; font-weight: 600; }
  .session ul { margin: 0; padding-left: 1.1rem; color: #555; line-height: 1.6; }
  code { font-size: .88em; background: var(--blood-soft); border-radius: 4px; padding: 0 .25rem; }
  kbd { border: 1px solid var(--line); border-radius: 4px; padding: 0 .3rem; font-size: .8rem; }
</style>
</head>
<body>
<svg class="drip" viewBox="0 0 1000 90" preserveAspectRatio="none" aria-hidden="true">
  <rect x="0" y="0" width="1000" height="14"/>
  <rect x="60" y="0" width="14" height="40" rx="7"/><rect x="230" y="0" width="18" height="62" rx="9"/>
  <rect x="410" y="0" width="12" height="46" rx="6"/><rect x="520" y="0" width="20" height="78" rx="10"/>
  <rect x="690" y="0" width="14" height="52" rx="7"/><rect x="870" y="0" width="16" height="42" rx="8"/>
</svg>
<main>
  <div class="kicker">6 séances de 1h30</div>
  <h1>C# 101</h1>
  <p class="lead">Découvrir C# quand on vient de Java : des slides côte à côte, puis des koans à faire passer du rouge au vert. <a href="#plan">Voir le plan de cours ↓</a></p>
  <div class="grid">
    ${sessions.map(card).join('\n    ')}
  </div>
  <section class="plan" id="plan">
    <h2>Plan de cours</h2>
    <p>À la fin de chaque séance, vous savez :</p>
    <ol>
    ${sessions.map(plan).join('\n    ')}
    </ol>
  </section>
  <footer>Dans les slides : <kbd>→</kbd> / <kbd>←</kbd> pour avancer, <kbd>O</kbd> pour la vue d'ensemble, <kbd>F</kbd> pour le plein écran.<br>
    Slides sous licence <a href="https://creativecommons.org/licenses/by-sa/4.0/deed.fr">CC BY-SA 4.0</a>.</footer>
</main>
</body>
</html>
`)

// GitHub Pages ne doit pas passer le site à Jekyll
writeFileSync(join(out, '.nojekyll'), '')
console.log(`✓ site prêt dans ${out}/ (base ${base})`)
