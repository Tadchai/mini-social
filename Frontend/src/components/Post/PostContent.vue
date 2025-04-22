<template>
  <div class="post-container">
    <p>{{ content }}</p>
    <div v-if="imageUrl.length" class="image-grid">
      <img
        v-for="(url, index) in imageUrl"
        :key="index"
        :src="url.imageUrl"
        alt="Post Image"
        class="image-item"
      />
    </div>
  </div>
</template>

<script setup>
defineProps({
  content: String,
  imageUrl: Array
})
</script>

<style scoped>
.post-container {
  margin-bottom: 1rem;
}

.image-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(160px, 1fr));
  gap: 12px;
  margin-top: 12px;
}

.image-item {
  width: 100%;
  height: auto;
  border-radius: 8px;
  object-fit: cover;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.1);
  transition: transform 0.3s;
}

.image-item:hover {
  transform: scale(1.03);
}
</style>
