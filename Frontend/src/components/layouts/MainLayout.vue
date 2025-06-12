<template>
  <div class="flex">
    <Navbar class="fixed top-0 left-0 right-0 h-14 z-20  bg-sky-950 text-white" />

    <aside class="w-1/4 fixed top-14 left-0 min-h-screen bg-slate-800 text-white z-10">
      <slot name="sidebar" />
    </aside>

    <main class="w-1/2 mx-auto mt-14 px-4">
      <slot />
    </main>

    <aside class="w-1/4 fixed top-14 right-0 min-h-screen bg-gray-800 text-white">
      <slot name="rightsidebar" />
    </aside>
  </div>
</template>

<script setup>
import Navbar from '../common/Navbar.vue';
</script>
