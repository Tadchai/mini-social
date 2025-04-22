<template>
  <div>
    <button @click="like">👍 Like</button>
    <button @click="comment">💬 Comment</button>
    <button @click="share">🔄 Share</button>
  </div>
</template>

<script setup>
const like = () => {
  alert("Liked!");
}
const comment = () => {
  alert("Comment clicked!");
}
const share = () => {
  alert("Shared!");
}
</script>
