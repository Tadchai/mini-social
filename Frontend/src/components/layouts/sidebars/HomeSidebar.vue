<template>
  <div>
    <div class="space-y-1">
      <router-link to="/">
        <div class="text-center py-2 hover:bg-sky-800">Home</div>
      </router-link>
      <router-link to="/about">
        <div class="text-center py-2 hover:bg-sky-800">About</div>
      </router-link>
      <router-link to="/friends">
        <div class="text-center py-2 hover:bg-sky-800">Friend</div>
      </router-link>
      <router-link to="/myposts">
        <div class="text-center py-2 hover:bg-sky-800">MyPosts</div>
      </router-link>
      <div @click="logout" class="text-center py-2 cursor-pointer hover:bg-sky-800">Logout</div>
    </div>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router'

const router = useRouter()

function logout()
{
  localStorage.removeItem('token')
  router.push('/login')
}
</script>
