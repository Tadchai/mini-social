<template>
  <div>
    <div v-for="(follow, index) in follows" :key="index">
     {{ follow.userId }} {{ follow.userName }}
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'

const API_URL = import.meta.env.VITE_API_URL
const follows = ref([])

async function fetchFollow() {
  try {
    let token = localStorage.getItem('token')
    const response = await fetch(`${API_URL}/Follow/Get`, {
      headers: { Authorization: `Bearer ${token}` },
    })
    const data = await response.json()
    follows.value = data.data
  } catch (error) {
    console.error('Error fetching:', error)
  }
}

onMounted(fetchFollow)
</script>
