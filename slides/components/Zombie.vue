<script setup lang="ts">
// Une silhouette de zombie (ou un cerveau...) qui s'invite dans un coin de la slide,
// en aplat de couleur comme la cravate de l'affiche
withDefaults(defineProps<{
  kind?: 'zombie' | 'hand' | 'brain' | 'skull' | 'tombstone' | 'eye'
  pos?: 'br' | 'bl' | 'tr' | 'tl'
  size?: number
}>(), { kind: 'zombie', pos: 'br', size: 7 })
</script>

<template>
  <div class="zombie" :class="pos" :style="{ fontSize: `${size}rem` }">
    <fluent-emoji-high-contrast-person-zombie v-if="kind === 'zombie'" />
    <game-icons-raise-zombie v-else-if="kind === 'hand'" />
    <game-icons-brain v-else-if="kind === 'brain'" />
    <game-icons-half-dead v-else-if="kind === 'skull'" />
    <game-icons-bleeding-eye v-else-if="kind === 'eye'" />
    <game-icons-tombstone v-else />
  </div>
</template>

<style scoped>
.zombie { position: absolute; line-height: 1; pointer-events: none; z-index: 2; color: var(--blood); }
.br { right: 3rem; bottom: 2.6rem; }
.bl { left: 3rem; bottom: 2.6rem; }
.tr { right: 3rem; top: 1.8rem; }
.tl { left: 3rem; top: 1.8rem; }
</style>
