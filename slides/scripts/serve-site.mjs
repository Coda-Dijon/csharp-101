// Sert le site construit par build-site.mjs (dossier dist/) sur http://localhost:4173
//   PORT=8080 node scripts/serve-site.mjs
import { createReadStream, existsSync, statSync } from 'node:fs'
import { createServer } from 'node:http'
import { extname, join, normalize } from 'node:path'

const root = 'dist'
const port = Number(process.env.PORT ?? 4173)
const types = {
  '.html': 'text/html; charset=utf-8', '.js': 'text/javascript', '.css': 'text/css', '.json': 'application/json',
  '.svg': 'image/svg+xml', '.png': 'image/png', '.jpg': 'image/jpeg', '.woff2': 'font/woff2', '.ico': 'image/x-icon',
}

if (!existsSync(root)) {
  console.error('Pas de dossier dist/ : lancez d\'abord npm run build:site')
  process.exit(1)
}

createServer((req, res) => {
  // normalize empêche de sortir de dist/ avec des ../
  let path = join(root, normalize(decodeURIComponent(new URL(req.url, 'http://x').pathname)))
  if (existsSync(path) && statSync(path).isDirectory()) path = join(path, 'index.html')
  if (!existsSync(path)) {
    res.writeHead(404).end('404')
    return
  }
  res.writeHead(200, { 'Content-Type': types[extname(path)] ?? 'application/octet-stream' })
  createReadStream(path).pipe(res)
}).listen(port, () => console.log(`✓ site sur http://localhost:${port}/`))
