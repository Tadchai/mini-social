<template>
  <div>
    <div class="text-gray-400 px-4 py-2 font-semibold">Contacts</div>

    <div v-if="isLoading" class="px-4 py-2 text-sm text-gray-500">Loading contacts...</div>
    <div v-else-if="errorMessage" class="px-4 py-2 text-red-400">{{ errorMessage }}</div>

    <div v-else>
      <div v-for="contact in contacts" :key="contact.id"
        class="flex items-center gap-2 px-4 py-2 hover:bg-gray-800 rounded-lg cursor-pointer">
        <svg xmlns="http://www.w3.org/2000/svg" class="w-6 h-6 text-gray-300" fill="currentColor" viewBox="0 0 24 24">
          <path
            d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10s10-4.48 10-10S17.52 2 12 2m0 4c1.93 0 3.5 1.57 3.5 3.5S13.93 13 12 13s-3.5-1.57-3.5-3.5S10.07 6 12 6m0 14c-2.03 0-4.43-.82-6.14-2.88a9.95 9.95 0 0 1 12.28 0C16.43 19.18 14.03 20 12 20" />
        </svg>
        <span class="text-white">{{ contact.username }}</span>
      </div>
    </div>
  </div>
</template>


<script setup lang="ts">
import { GetContact } from '@/services/followService'
import type { UserResponse } from '@/types/User'
import { onMounted, ref } from 'vue'

const isLoading = ref(false)
const errorMessage = ref<string | null>(null)

const contacts = ref<UserResponse[]>([])

async function fetchContacts() {
  isLoading.value = true
  errorMessage.value = null
  try {
    const result = await GetContact()
    if (result.statusCode === 200) {
      contacts.value = result.data ?? []
    } else {
      errorMessage.value = result.message ?? 'Something went wrong.'
    }
  } catch (err) {
    errorMessage.value = (err as Error).message
  } finally {
    isLoading.value = false
  }
}

onMounted(fetchContacts)
</script>
