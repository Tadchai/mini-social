<template>
  <div class="flex">
    <Navbar class="fixed top-0 left-0 right-0 h-14 bg-sky-950 text-white z-20" />

    <div class="w-1/4 min-h-screen fixed top-14 left-0 bg-sky-800 text-white z-10">
      <slot name="sidebar" />
    </div>

    <main class="w-3/4 mt-14 px-4 ml-auto">
      <slot />
    </main>
  </div>
</template>

<script setup>
import Navbar from '../common/Navbar.vue';
</script>
