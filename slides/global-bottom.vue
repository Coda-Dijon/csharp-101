<script setup lang="ts">
import { computed } from 'vue'
import { useNav } from '@slidev/client'

const { currentLayout, currentPage, total } = useNav()
const visible = computed(() => !['cover', 'section', 'end'].includes(currentLayout.value))
const poster = computed(() => ['cover', 'end'].includes(currentLayout.value))
</script>

<template>
  <footer v-if="visible" class="footer">
    <span><b>■</b> {{ $slidev.configs.title }}</span>
    <span class="right"><License :size="1.1" /> {{ currentPage }} / {{ total }}</span>
  </footer>
  <div v-else-if="poster" class="poster" :class="currentLayout">
    <License :size="2.2" />
    <span v-if="currentLayout === 'end'">Slides sous licence CC BY-SA 4.0</span>
  </div>
</template>

<style scoped>
.footer {
  position: absolute; left: 3.5rem; right: 3.5rem; bottom: 1rem;
  display: flex; justify-content: space-between; align-items: flex-end;
  font-size: 0.65rem; letter-spacing: 0.04em; color: var(--muted); pointer-events: none; z-index: 10;
}
.footer b { color: var(--blood); }
.right { display: flex; align-items: center; gap: 0.8rem; }

.poster {
  position: absolute; left: 0; right: 0; bottom: 1.6rem; z-index: 10;
  display: flex; flex-direction: column; align-items: center; gap: 0.9rem;
  font-size: 0.6rem; letter-spacing: 0.08em; text-transform: uppercase; color: var(--muted);
}
/* Fond rouge : le badge passe en blanc, le sang devient de l'os */
.poster.end { color: rgba(255, 255, 255, 0.75); --license-flesh: white; --license-eye: var(--blood); --license-glyph: white; }
</style>
