<script setup lang="ts">
// Petite bulle d'explication, utilisée pour "décortiquer" du code
defineProps<{ n?: string | number }>()
</script>

<template>
  <div class="note"><b v-if="n" class="num">{{ n }}</b><div><slot /></div></div>
</template>

<style scoped>
.note {
  display: flex; gap: 0.6rem; align-items: flex-start;
  background: var(--mist); border-left: 3px solid var(--blood);
  padding: 0.4rem 0.75rem; margin: 0.35rem 0; border-radius: 0 8px 8px 0; font-size: 0.8rem; line-height: 1.4;
}
.num {
  flex: none; width: 1.35rem; height: 1.35rem; border-radius: 50%;
  background: var(--blood); color: white; display: grid; place-items: center; font-size: 0.72rem;
}
</style>
