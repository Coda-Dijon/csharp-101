<script setup lang="ts">
import { computed } from 'vue'
import { useNav } from '@slidev/client'

// Pied de page discret : titre de la séance et numéro de slide, masqué sur les slides pleine page
const { currentLayout, currentPage, total } = useNav()
const visible = computed(() => !['cover', 'section', 'end'].includes(currentLayout.value))
</script>

<template>
  <footer v-if="visible" class="footer">
    <span><b>■</b> {{ $slidev.configs.title }}</span>
    <span>{{ currentPage }} / {{ total }}</span>
  </footer>
</template>

<style scoped>
.footer {
  position: absolute; left: 3.5rem; right: 3.5rem; bottom: 1rem;
  display: flex; justify-content: space-between;
  font-size: 0.65rem; letter-spacing: 0.04em; color: var(--muted); pointer-events: none; z-index: 10;
}
.footer b { color: var(--blood); }
</style>
