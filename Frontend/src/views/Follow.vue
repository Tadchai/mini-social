<template>
  <div>
    <div v-if="follows.length === 0">
      <p>You are not following anyone.</p>
    </div>
    <div v-for="(follow, index) in follows" :key="index">
      {{ follow.userName }}
      <button @click="createPrivateConversation(follow.userId)">Chat</button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'
import { onMounted, ref } from 'vue'

const API_URL = import.meta.env.VITE_API_URL
const router = useRouter()

const follows = ref<any[]>([])
const token = localStorage.getItem('token')

async function fetchFollow() {
  if (!token) return
  try {
    const response = await fetch(`${API_URL}/Follow/Get`, {
      headers: { Authorization: `Bearer ${token}` },
    })
    const result = await response.json()
    follows.value = result.data
  } catch (error) {
    console.error('Error fetching:', error)
  }
}

async function createPrivateConversation(targetUserId: number) {
  if (!token) return
  try {
    const response = await fetch(`${API_URL}/Chat/CreatePrivate`, {
      method: 'POST',
      headers: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ targetUserId }),
    })
    const result = await response.json()

    if ((result.statusCode === 200 || result.statusCode === 201) && result.id) {
      router.push({ path: `/chat/${result.id}` });
    } else {
      alert(result.message)
    }
  } catch (error) {
    console.error('Error:', error)
  }
}

onMounted(fetchFollow)
</script>
