<template>
  <div v-if="showModal" class="fixed inset-0 bg-black/50 flex items-center justify-center z-60">
    <div class=" bg-white p-5 rounded-lg w-full max-w-3xl min-w-md">

      <div class="text-xl font-semibold mb-4">
        <slot name="header"></slot>
      </div>

      <slot></slot>

      <div class=" mt-2.5 text-right">
        <slot name="footer"></slot>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, defineExpose } from 'vue'

const showModal = ref<boolean>(false)

function openModal() {
  showModal.value = true
}
function closeModal() {
  showModal.value = false
}

defineExpose({ openModal, closeModal })
</script>
