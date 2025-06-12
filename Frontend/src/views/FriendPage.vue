<template>
  <CustomLayout>
    <template #sidebar>
      <FriendSidebar />
    </template>

    <div v-if="isLoading">Loading messages...</div>
    <div v-else-if="errorMessage">{{ errorMessage }}</div>

    <div v-if="!isLoading && !errorMessage && follows.length === 0">
      <p>You are not following anyone.</p>
    </div>

    <div v-for="follow in follows" :key="follow.userId">
      {{ follow.username }}
      <button @click="createPrivateConversation(follow.userId)">Chat</button>
    </div>
  </CustomLayout>
</template>

<script setup lang="ts">
import CustomLayout from '@/components/layouts/CustomLayout.vue'
import FriendSidebar from '@/components/layouts/sidebars/FriendSidebar.vue'
import { useRouter } from 'vue-router'
import { onMounted, ref } from 'vue'
import { GetFollow } from '../services/followService'
import type { Follow } from '../types/Follow'
import { CreatePrivate } from '../services/groupService'



const router = useRouter()

const follows = ref<Follow[]>([])
const isLoading = ref(false)
const errorMessage = ref('')

async function fetchFollow() {
  isLoading.value = true
  errorMessage.value = ''
  try {
    const result = await GetFollow()
    follows.value = result.data ?? []
  } catch (error: any) {
    console.error('Error fetching follow:', error)
    errorMessage.value = error.message || 'Failed to fetch follow data.'
  } finally {
    isLoading.value = false
  }
}

async function createPrivateConversation(targetUserId: number) {
  try {
    const result = await CreatePrivate(targetUserId)

    if ((result.statusCode === 200 || result.statusCode === 201) && result.data) {
      router.push({ path: `/chat/${result.data}` })
    } else {
      alert(result.message || 'Failed to create chat.')
    }
  } catch (error: any) {
    console.error('Error:', error)
    alert(error.message || 'Failed to create private chat.')
  }
}

onMounted(fetchFollow)
</script>
