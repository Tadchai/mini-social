<template>
  <aside class="w-1/4 bg-sky-900 text-white h-screen">
    <div class="space-y-1">
      <router-link to="/">
        <div class="text-center py-2 hover:bg-sky-800">Home</div>
      </router-link>
      <router-link to="/about">
        <div class="text-center py-2 hover:bg-sky-800">About</div>
      </router-link>
      <router-link to="/follow">
        <div class="text-center py-2 hover:bg-sky-800">Follow</div>
      </router-link>
      <router-link to="/myposts">
        <div class="text-center py-2 hover:bg-sky-800">MyPosts</div>
      </router-link>
      <div @click="logout" class="text-center py-2 cursor-pointer hover:bg-sky-800">Logout</div>
    </div>
  </aside>
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
