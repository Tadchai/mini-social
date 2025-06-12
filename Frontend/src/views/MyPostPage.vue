<template>
  <MainLayout>
    <template #sidebar>
      <HomeSidebar />
    </template>

    <h2>MyPosts</h2>
    <button @click="openModal">Create Your Post !!</button>
    <BaseModal ref="modal">
      <template #header> Create Post </template>

      <label>content</label><br />
      <input class="input-text" type="text" v-model="content" />
      <div class="m-5">
        <input class="input-file" type="file" multiple accept="image/*" @change="handleFileChange" />

        <div v-if="previewImages.length" class="flex flex-wrap gap-2.5 mt-2.5">
          <img v-for="(img, index) in previewImages" :key="index" :src="img"
            class="size-40 object-cover rounded-lg shadow-md" />
        </div>
      </div>
      <template #footer>
        <button @click="CreatePost" class="btn">create post</button>
        <button @click="closeModal" class="btn">close</button>
      </template>
    </BaseModal>

    <div>
      <PostItem v-for="(post, index) in posts" :key="index" :post="post" />
      <div ref="loadMoreTrigger" style="height: 1px"></div>
    </div>

    <div v-if="isLoading">
      <p>กำลังโหลด...</p>
    </div>

    <template #rightsidebar>
      <RightSidebar />
    </template>
  </MainLayout>
</template>

<script setup lang="ts">
import MainLayout from '@/components/layouts/MainLayout.vue';
import HomeSidebar from '@/components/layouts/sidebars/HomeSidebar.vue';
import RightSidebar from '@/components/common/RightSidebar.vue';
import BaseModal from '@/components/common/BaseModal.vue';
import PostItem from '@/components/Post/PostItem.vue'
import { onBeforeUnmount, onMounted, ref, useTemplateRef } from 'vue'
import type { Post } from '../types/Post'
import { createPost, fetchMyPosts } from '../services/postService'

const modal = useTemplateRef<InstanceType<typeof BaseModal>>('modal')

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
    const result = await fetchMyPosts(3, loadMore ? (lastCursor.value ?? undefined) : undefined)

    if (result.statusCode == 200) {
      const newPosts: Post[] = result.data ?? []
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
