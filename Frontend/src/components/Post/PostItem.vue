<template>
  <div class="post">
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

<style scoped>
.post {
  background-color: white;
  padding: 16px;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  margin-bottom: 16px;
}
</style>
