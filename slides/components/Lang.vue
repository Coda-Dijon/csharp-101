<script setup lang="ts">
// En-tête de colonne avec une icône zombie par langage, comme les colonnes Kotlin | Scala | Clojure de fp101 :
// Java, le zombie qui traîne des pieds ; C#, celui qui sort de terre
defineProps<{ name: 'java' | 'csharp' }>()
</script>

<template>
  <div class="lang">
    <template v-if="name === 'java'"><game-icons-shambling-zombie class="logo" /><span>Java</span></template>
    <template v-else><game-icons-raise-zombie class="logo" /><span>C#</span></template>
    <small v-if="$slots.default"><slot /></small>
  </div>
</template>

<style scoped>
.lang { display: flex; align-items: center; gap: 0.5rem; margin-bottom: 0.5rem; }
.logo { font-size: 1.6rem; color: var(--blood); }
small { font-size: 0.75rem; color: var(--muted); margin-left: 0.3rem; }
span { font-size: 1.15rem; font-weight: 800; letter-spacing: -0.01em; color: var(--ink); }
</style>
