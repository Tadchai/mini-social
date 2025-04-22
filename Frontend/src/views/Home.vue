<template>
  <div>
    <h2>🏠 Home Page</h2>
    <br />
    <h3>Create Post</h3>
    <label>content</label><br />
    <input type="text" v-model="content" />
    <div class="upload-section">
      <input type="file" multiple accept="image/*" @change="handleFileChange" />

      <div v-if="previewImages.length" class="preview-grid">
        <img v-for="(img, index) in previewImages" :key="index" :src="img" class="preview-image" />
      </div>
    </div>
    <button @click="CreatePost">create post</button>
    <br /><br />
    <hr />
    <br />

    <div>
      <PostItem v-for="(post, index) in posts" :key="index" :post="post" />
      <div ref="loadMoreTrigger" style="height: 1px"></div>
    </div>

    <div v-if="isLoading">
      <p>กำลังโหลด...</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import PostItem from '../components/Post/PostItem.vue'
import { onBeforeUnmount, onMounted, ref } from 'vue'
import type { Post } from '../types/Post'
import { ApiResponse } from '../types/Response'

const API_URL = import.meta.env.VITE_API_URL
const content = ref<null | string>(null)
const selectedImages = ref([])
const previewImages = ref([])
const posts = ref<Post[]>([])
const lastCursor = ref<{ createdAt: string; id: number } | null>(null)
const isLoading = ref(false)
const hasMore = ref(true)
const loadMoreTrigger = ref<HTMLElement | null>(null)

const observer = ref<IntersectionObserver | null>(null)

function handleFileChange(event) {
  selectedImages.value = Array.from(event.target.files)
  previewImages.value = selectedImages.value.map((file) => URL.createObjectURL(file))
}

async function CreatePost() {
  try {
    const token = localStorage.getItem('token')

    const formData = new FormData()
    if (content.value !== null) {
      formData.append('content', content.value)
    }
    selectedImages.value.forEach((file) => {
      formData.append('Image', file)
    })

    const response = await fetch(`${API_URL}/Post/Create`, {
      method: 'POST',
      headers: { Authorization: `Bearer ${token}` },
      body: formData,
    })

    const result: ApiResponse<null> = await response.json()
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
  const token = localStorage.getItem('token')
  let url = `${API_URL}/Post/GetById?userId=1&pageSize=3`

  if (loadMore && lastCursor.value) {
    url += `&lastCreatedAt=${encodeURIComponent(lastCursor.value.createdAt)}&lastId=${lastCursor.value.id}`
  }

  try {
    const response = await fetch(url, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    })

    const result = await response.json()

    const newPosts: Post[] = result.data
    if (loadMore) {
      posts.value.push(...newPosts)
    } else {
      posts.value = newPosts
    }

    if (newPosts.length === 0 || !result.hasNextPage) {
      hasMore.value = false
    } else {
      lastCursor.value = {
        createdAt: result.lastCreatedAt,
        id: result.lastId,
      }
    }
  } catch (err) {
    console.error('Fetch error:', err)
  } finally {
    isLoading.value = false
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

<style scoped>
.upload-section {
  margin: 20px;
}
.preview-grid {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
  margin-top: 10px;
}
.preview-image {
  width: 120px;
  height: 120px;
  object-fit: cover;
  border-radius: 8px;
  box-shadow: 0 0 4px rgba(0, 0, 0, 0.1);
}
</style>
