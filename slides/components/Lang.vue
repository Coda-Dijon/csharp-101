<script setup lang="ts">
// En-tête de colonne avec le logo du langage, comme les colonnes Kotlin | Scala | Clojure de fp101
defineProps<{ name: 'java' | 'csharp' }>()
</script>

<template>
  <div class="lang">
    <template v-if="name === 'java'"><logos-java class="logo" /><span>Java</span></template>
    <template v-else><logos-c-sharp class="logo" /><span>C#</span></template>
    <small v-if="$slots.default"><slot /></small>
  </div>
</template>

<style scoped>
.lang { display: flex; align-items: center; gap: 0.5rem; margin-bottom: 0.5rem; }
.logo { font-size: 1.6rem; }
small { font-size: 0.75rem; color: var(--muted); margin-left: 0.3rem; }
span { font-size: 1.15rem; font-weight: 800; letter-spacing: -0.01em; color: var(--ink); }
</style>
