<template>
  <aside class="sidebar">
    <ul>
      <router-link to="/"><li>Home</li></router-link>
      <router-link to="/about"><li>About</li></router-link>
      <router-link to="/follow"><li>Follow</li></router-link>
      <router-link to="/myposts"><li>MyPosts</li></router-link>
      <li @click="logout">Logout</li>
    </ul>
  </aside>
</template>

<script setup>
import { useRouter } from 'vue-router';

const router = useRouter();

function logout(){
  localStorage.removeItem('token');
  router.push('/login')
}
</script>

<style scoped>
.sidebar {
  width: 200px;
  background-color: #34495e;
  color: white;
  height: 100vh;
}

.sidebar ul {
  list-style: none;
  padding: 0;
}

.sidebar li {
  text-align: center;
  padding: 10px;
}

.sidebar li:hover {
  background-color: #415e78;
}
a {
  color: white;
  text-decoration: none;
}
</style>
