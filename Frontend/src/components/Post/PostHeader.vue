<template>
  <div class="flex items-center mb-2">
    <img :src="user.avatar" alt="Avatar" class="w-10 h-10 rounded-full mr-2" />
    <div>
      <p class="font-bold">{{ user.name }}</p>
      <p class="text-sm text-gray-500">{{ createdAt }}</p>
    </div>
  </div>
</template>

<script setup>
defineProps({
  user: Object,
  createdAt: String
})
</script>
