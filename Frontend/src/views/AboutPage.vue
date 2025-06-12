<template>
  <MainLayout>
    <template #sidebar>
      <HomeSidebar />
    </template>

    <h2>ℹ️ About Page</h2>

    <template #rightsidebar>
      <RightSidebar />
    </template>
  </MainLayout>
</template>

<script setup lang="ts">
import MainLayout from '@/components/layouts/MainLayout.vue';
import HomeSidebar from '@/components/layouts/sidebars/HomeSidebar.vue';
import RightSidebar from '@/components/common/RightSidebar.vue';
</script>
