<template>
  <div class="layout">
    <Sidebar class="sidebar"/>
    <div class="main-content">
      <Navbar class="navbar"/>
      <div class="page-content">
        <router-view />
      </div>
    </div>
  </div>
</template>

<script setup>
import Sidebar from '../components/Sidebar.vue'
import Navbar from '../components/Navbar.vue'
</script>

<style scoped>
.layout {
  display: flex;
  flex-direction: row;
}

.sidebar {
  width: 200px;
  min-height: 100vh;
  position: fixed;
  top: 60px;
  left: 0;
}

.navbar {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  height: 60px;
  z-index: 1000;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.main-content {
  margin-left: 200px;
  margin-top: 60px;
  flex: 1;
  padding: 20px;
}

@media (max-width: 768px) {
  .sidebar {
    display: none;
  }
  .main-content {
    margin-left: 0;
  }
}
</style>
