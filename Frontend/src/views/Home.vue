<template>
  <div>
    <button @click="openModal" class="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 transition">
      Create Your Post !!
    </button>

    <Modal ref="modal">
      <h3 class="text-xl font-semibold mb-4">Create Post</h3>

      <label class="block text-sm font-medium mb-1">Content</label>
      <input type="text" v-model="content"
        class="w-full px-4 py-2 border border-gray-300 rounded-md mb-4 focus:outline-none focus:ring-2 focus:ring-blue-500" />

      <div class="my-5">
        <input type="file" multiple accept="image/*" @change="handleFileChange" class="block w-full text-sm text-gray-700 file:mr-4 file:py-2 file:px-4
                 file:rounded-md file:border-0 file:text-sm file:font-semibold
                 file:bg-blue-500 file:text-white hover:file:bg-blue-600" />

        <div v-if="previewImages.length" class="flex flex-wrap gap-2 mt-3">
          <img v-for="(img, index) in previewImages" :key="index" :src="img"
            class="w-[120px] h-[120px] object-cover rounded shadow-sm" />
        </div>
      </div>

      <button @click="CreatePost" class="bg-green-600 text-white px-4 py-2 rounded hover:bg-green-700 mr-2">
        Create Post
      </button>
      <button @click="closeModal" class="bg-gray-500 text-white px-4 py-2 rounded hover:bg-gray-600">
        Close
      </button>
    </Modal>

    <div>
      <PostItem v-for="(post, index) in posts" :key="index" :post="post" />
      <div ref="loadMoreTrigger" class="h-px"></div>
    </div>

    <div v-if="isLoading" class="mt-4 text-center text-gray-600">
      <p>กำลังโหลด...</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import Modal from '@/components/Modal.vue'
import PostItem from '@/components/Post/PostItem.vue'
import { onBeforeUnmount, onMounted, ref, useTemplateRef } from 'vue'
import type { Post } from '../types/Post'
import { createPost, fetchFollowPosts } from '../services/postService'

const modal = useTemplateRef<InstanceType<typeof Modal>>('modal')

const content = ref<null | string>(null)
const selectedImages = ref([])
const previewImages = ref([])
const posts = ref<Post[]>([])
const lastCursor = ref<null | string>(null)
const isLoading = ref(false)
const hasMore = ref(true)
const loadMoreTrigger = ref<HTMLElement | null>(null)

const observer = ref<IntersectionObserver | null>(null)

function openModal() {
  modal.value?.openModal()
}

function closeModal() {
  content.value = null
  selectedImages.value = []
  previewImages.value = []
  modal.value?.closeModal()
}

function handleFileChange(event) {
  selectedImages.value = Array.from(event.target.files)
  previewImages.value = selectedImages.value.map((file) => URL.createObjectURL(file))
}

async function CreatePost() {
  try {
    const result = await createPost(content.value, selectedImages.value)
    if (result.statusCode === 201) {
      alert(result.message)
      content.value = ''
      selectedImages.value = []
      previewImages.value = []
      fetchPosts()
    } else {
      alert(result.message)
    }
  } catch (error) {
    console.error('Error:', error)
  }
}

async function fetchPosts(loadMore = false) {
  if (isLoading.value || (!hasMore.value && loadMore)) return

  isLoading.value = true
  try {
    const result = await fetchFollowPosts(3, loadMore ? (lastCursor.value ?? undefined) : undefined)

    if (result.statusCode == 200) {
      const newPosts: Post[] = result.data
      if (loadMore) {
        posts.value.push(...newPosts)
      } else {
        posts.value = newPosts
      }

      hasMore.value = result.hasNextPage
      lastCursor.value = result.lastCursor ?? null
    } else {
      console.error(result.message)
    }

    isLoading.value = false
  } catch (err) {
    console.error('Fetch error:', err)
  }
}

onMounted(() => {
  fetchPosts()

  observer.value = new IntersectionObserver((entries) => {
    if (entries[0].isIntersecting && hasMore.value && !isLoading.value) {
      fetchPosts(true)
    }
  })

  if (loadMoreTrigger.value) {
    observer.value.observe(loadMoreTrigger.value)
  }
})

onBeforeUnmount(() => {
  if (observer.value && loadMoreTrigger.value) {
    observer.value.unobserve(loadMoreTrigger.value)
  }
})
</script>
