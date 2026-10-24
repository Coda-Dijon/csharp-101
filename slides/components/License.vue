<script setup lang="ts">
// Badge CC BY-SA version zombie : mordu en haut à droite, le sang coule dessous
withDefaults(defineProps<{ size?: number }>(), { size: 1.4 })

const drips = [
  { x: 14, w: 4, h: 12 }, { x: 31, w: 3, h: 7 }, { x: 46, w: 5, h: 17 }, { x: 63, w: 3, h: 9 },
  { x: 79, w: 4, h: 14 }, { x: 98, w: 3, h: 6 }, { x: 110, w: 5, h: 11 },
]
</script>

<template>
  <a class="license" href="https://creativecommons.org/licenses/by-sa/4.0/deed.fr" target="_blank"
     title="Creative Commons Attribution - Partage dans les mêmes conditions 4.0" :style="{ height: `${size}rem` }">
    <svg viewBox="0 0 130 56" role="img" aria-label="Licence CC BY-SA 4.0">
      <mask id="cc-bite">
        <rect width="130" height="56" fill="white" />
        <circle cx="127" cy="2" r="5" /><circle cx="119" cy="-1" r="4" /><circle cx="128" cy="11" r="3.5" />
      </mask>
      <g mask="url(#cc-bite)" class="flesh">
        <rect x="1" y="1" width="128" height="36" rx="7" />
        <rect v-for="(d, i) in drips" :key="i" :x="d.x" y="30" :width="d.w" :height="7 + d.h" :rx="d.w / 2" />
      </g>
      <circle class="flesh" cx="47.5" cy="53" r="2.5" />

      <!-- cc -->
      <circle class="eye" cx="20" cy="19" r="14" />
      <text class="glyph" x="20" y="25.5" text-anchor="middle">cc</text>
      <!-- BY : l'auteur, bras tendus, en quête de cerveaux -->
      <circle class="eye" cx="52" cy="19" r="12" />
      <circle class="glyph" cx="50" cy="11.5" r="2.6" />
      <path class="glyph" d="M47.5 15h5v1h6v2.2h-6v1h5.5v2.2h-5.5v1.6h-1v6h-3v-6h-1z" />
      <!-- SA : ça revient toujours -->
      <circle class="eye" cx="81" cy="19" r="12" />
      <path class="loop" d="M75.6 16.2a6.2 6.2 0 1 1 0 5.6" />
      <path class="glyph" d="M72.6 13.6l6 0l-3 4.4z" />
      <!-- 4.0 -->
      <text class="version" x="111" y="25">4.0</text>
    </svg>
  </a>
</template>

<style scoped>
.license { display: inline-block; line-height: 0; border: none !important; pointer-events: auto; }
svg { height: 100%; width: auto; overflow: visible; transform: rotate(-3deg); }
.flesh { fill: var(--license-flesh, var(--blood)); }
.eye { fill: var(--license-eye, white); }
.glyph { fill: var(--license-glyph, var(--ink)); }
.loop { fill: none; stroke: var(--license-glyph, var(--ink)); stroke-width: 2.6; }
text.glyph { font: 900 17px Inter, sans-serif; letter-spacing: -1px; }
.version { fill: var(--license-eye, white); font: 700 13px 'Bebas Neue', sans-serif; letter-spacing: 1px; text-anchor: middle; }
</style>
