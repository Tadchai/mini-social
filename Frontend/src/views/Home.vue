<template>
  <div>
    <button @click="openModal">Create Your Post !!</button>
    <Modal ref="modal">
      <h3>Create Post</h3>
      <label>content</label><br />
      <input type="text" v-model="content" />
      <div class="upload-section">
        <input type="file" multiple accept="image/*" @change="handleFileChange" />

        <div v-if="previewImages.length" class="preview-grid">
          <img
            v-for="(img, index) in previewImages"
            :key="index"
            :src="img"
            class="preview-image"
          />
        </div>
      </div>
      <button @click="CreatePost">create post</button>
      <button @click="closeModal">close</button>
    </Modal>

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
