<template>
  <div class=" bg-white p-4 rounded-lg shadow-md mb-4">
    <!-- <PostHeader :user="post.user" :createdAt="post.createdAt" /> -->
    <PostContent :content="post.content" :imageUrl="post.imageUrl" />
    <PostActions />
  </div>
</template>

<script setup>
import PostHeader from './PostHeader.vue'
import PostContent from './PostContent.vue'
import PostActions from './PostActions.vue'

defineProps({
  post: Object
})
</script>
