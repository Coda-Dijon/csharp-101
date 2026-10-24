// Construit le site GitHub Pages : un dossier par deck Slidev + une page d'accueil pour choisir la séance.
//   BASE=/csharp-101/ node scripts/build-site.mjs   (BASE = chemin du site, "/" par défaut)
import { execFileSync } from 'node:child_process'
import { existsSync, mkdirSync, rmSync, writeFileSync } from 'node:fs'
import { join } from 'node:path'

const base = (process.env.BASE ?? '/').replace(/\/?$/, '/')
const out = 'dist'

// Le programme complet : une séance sans fichier de slides apparaît « à venir »
const sessions = [
  { slug: 's1-fundamentals', n: 1, title: 'Fondamentaux', topics: 'Histoire de .NET, compilation, syntaxe Java ↔ C#, anatomie d\'un test' },
  { slug: 's2-types', n: 2, title: 'Types & design objet', topics: 'Propriétés, encapsulation, struct vs class, ref/out, héritage, records, generics, pattern matching' },
  { slug: 's3-linq', n: 3, title: 'Collections & LINQ', topics: 'Lambdas, LINQ vs Streams sur une horde de zombies, exécution différée, méthodes d\'extension' },
  { slug: 's4-async', n: 4, title: 'Async & concurrence', topics: 'Task, async/await, CancellationToken, IAsyncEnumerable, race conditions' },
  { slug: 's5-domain-model', n: 5, title: 'Le Food-Truck Zombie', topics: 'Un domain model complet, piloté par les tests' },
]

rmSync(out, { recursive: true, force: true })
mkdirSync(out, { recursive: true })

for (const s of sessions) {
  s.available = existsSync(`${s.slug}.md`)
  if (!s.available) continue
  console.log(`▶ ${s.slug}`)
  execFileSync('npx', ['slidev', 'build', `${s.slug}.md`, '--base', `${base}${s.slug}/`, '--out', join(out, s.slug)], { stdio: 'inherit' })
}

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
  <div class="kicker">5 séances de 1h30</div>
  <h1>C# 101</h1>
  <p class="lead">Découvrir C# quand on vient de Java : des slides côte à côte, puis des koans à faire passer du rouge au vert.</p>
  <div class="grid">
    ${sessions.map(card).join('\n    ')}
  </div>
  <footer>Dans les slides : <kbd>→</kbd> / <kbd>←</kbd> pour avancer, <kbd>O</kbd> pour la vue d'ensemble, <kbd>F</kbd> pour le plein écran.<br>
    Slides sous licence <a href="https://creativecommons.org/licenses/by-sa/4.0/deed.fr">CC BY-SA 4.0</a>.</footer>
</main>
</body>
</html>
`)

// GitHub Pages ne doit pas passer le site à Jekyll
writeFileSync(join(out, '.nojekyll'), '')
console.log(`✓ site prêt dans ${out}/ (base ${base})`)
