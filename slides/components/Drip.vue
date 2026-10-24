<script setup lang="ts">
// Bandeau de sang qui coule depuis le haut de la slide, clin d'œil à la cravate de Shaun of the Dead
const drips = [
  { x: 60, w: 14, h: 62 }, { x: 150, w: 9, h: 38 }, { x: 230, w: 18, h: 96 }, { x: 330, w: 8, h: 30 },
  { x: 410, w: 12, h: 70 }, { x: 520, w: 20, h: 118 }, { x: 600, w: 8, h: 44 }, { x: 690, w: 14, h: 80 },
  { x: 790, w: 9, h: 34 }, { x: 870, w: 16, h: 64 }, { x: 950, w: 8, h: 40 },
]
const drops = [{ x: 529, y: 140, r: 7 }, { x: 239, y: 114, r: 5 }, { x: 697, y: 98, r: 4 }]
</script>

<template>
  <svg class="drip" viewBox="0 0 1000 150" preserveAspectRatio="none" aria-hidden="true">
    <rect x="0" y="0" width="1000" height="22" />
    <rect v-for="(d, i) in drips" :key="i" :x="d.x" y="0" :width="d.w" :height="d.h" :rx="d.w / 2" />
    <circle v-for="(d, i) in drops" :key="`d${i}`" :cx="d.x" :cy="d.y" :r="d.r" />
  </svg>
</template>

<style scoped>
.drip { position: absolute; top: 0; left: 0; width: 100%; height: 22%; fill: var(--blood); pointer-events: none; }
</style>
