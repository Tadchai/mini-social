<template>
  <div class="mb-4">
    <p>{{ content }}</p>
    <div v-if="imageUrl.length" class="grid gap-3 mt-3 grid-cols-[repeat(auto-fill,minmax(160px,1fr))]">
      <img v-for="(url, index) in imageUrl" :key="index" :src="url.imageUrl" alt="Post Image"
        class="w-full h-auto rounded-lg object-cover shadow-md " />
    </div>
  </div>
</template>

<script setup>
defineProps({
  content: String,
  imageUrl: Array,
})
</script>
