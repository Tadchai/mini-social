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
<template>
  <div class="modal-overlay" v-if="showModal">
    <div class="modal">
      <slot></slot>

      <div class="modal-footer">
        <slot name="footer"></slot>
      </div>
    </div>
  </div>
</template>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
}
.modal {
  background: white;
  padding: 20px;
  border-radius: 8px;
}
.modal-footer {
  margin-top: 10px;
  text-align: right;
}
</style>
