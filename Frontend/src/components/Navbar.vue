<template>
  <div class="flex items-center">
    <div class="w-1/4 flex items-center">
      <div class="text-white font-bold p-2.5">MiniSocial</div>

      <div class="relative w-full max-w-xs p-2.5" ref="searchBoxRef">
        <svg class="w-5 h-5 absolute left-3 top-1/2 transform -translate-y-1/2 text-white" fill="none"
          stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round"
            d="M21 21l-4.35-4.35m0 0A7.5 7.5 0 1010.5 18.5a7.5 7.5 0 006.15-3.85z" />
        </svg>
        <input type="text" placeholder="Search" v-model="search" @focus="showDropdown = true"
          class="w-full pl-10 pr-4 py-2 rounded-full bg-gray-500 text-white placeholder-white focus:outline-none focus:ring-2 focus:ring-blue-500" />
      </div>

      <ul v-if="search && results.length > 0 && showDropdown"
        class="absolute bg-white text-black w-1/4 rounded shadow-md max-h-60 overflow-y-auto left-0 top-14">
        <li v-for="user in results" :key="user.id" class="p-2 hover:bg-gray-200 cursor-pointer"
          @click="goToProfile(user.id)">
          {{ user.username }}
        </li>
      </ul>
    </div>

    <div class="w-1/2"></div>

    <div class="w-1/4"></div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { fetchSearch } from '@/services/searchService'
import type { UserResponse } from '@/types/User'
import { useRouter } from 'vue-router'
import { onClickOutside } from '@vueuse/core'

const router = useRouter()

const isLoading = ref(false)
const showDropdown = ref(false)
const searchBoxRef = ref<HTMLElement | null>(null)
const results = ref<UserResponse[] | null>([])
const search = ref('')

onClickOutside(searchBoxRef, () => {
  showDropdown.value = false
  search.value = ''
})

function goToProfile(userId: number) {
  router.push(`/profile/${userId}`)
}

async function performSearch() {
  if (!search.value.trim()) {
    results.value = []
    return
  }

  isLoading.value = true
  try {
    const result = await fetchSearch(search.value)

    if (result.statusCode === 200) {
      results.value = result.data
    } else {
      console.error(result.message)
    }
  } catch (err) {
    console.error('Fetch error:', err)
  } finally {
    isLoading.value = false
  }
}

watch(search, performSearch)
</script>
