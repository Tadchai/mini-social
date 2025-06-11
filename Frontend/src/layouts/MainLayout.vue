<template>
  <div class="flex">
    <Navbar class="fixed top-0 left-0 right-0 h-14 bg-sky-950 text-white z-20" />

    <Sidebar class="w-1/4 min-h-screen fixed top-14 left-0 bg-sky-800 text-white z-10" />

    <main class="w-1/2 mx-auto mt-14 px-4">
      <router-view />
    </main>

    <RightSidebar class="w-1/4 min-h-screen fixed top-14 right-0 bg-gray-900 text-white" />
  </div>
</template>

<script setup>
import Sidebar from '../components/Sidebar.vue'
import Navbar from '../components/Navbar.vue'
import RightSidebar from '../components/RightSidebar.vue'
</script>
