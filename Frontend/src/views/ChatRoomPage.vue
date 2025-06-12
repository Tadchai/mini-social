<template>
  <div class="flex flex-col h-full">
    <div class="flex-1 overflow-y-auto p-4 space-y-2">
      <div v-if="isLoading" class="text-center text-gray-500">Loading messages...</div>
      <div v-else-if="errorMessage" class="text-center text-red-500">{{ errorMessage }}</div>
      <div v-else v-for="msg in messages" :key="msg.id" :class="[
        'rounded-2xl px-4 py-2 max-w-[70%] break-words',
        msg.senderId === currentUserId
          ? 'bg-green-100 self-end text-right'
          : 'bg-gray-200 self-start text-left'
      ]">
        <span class="block">{{ msg.content }}</span>
        <small class="text-xs text-gray-500">{{ formatTime(msg.createdAt) }}</small>
      </div>
    </div>

    <div class="flex items-center p-2 border-t gap-2">
      <input v-model="newMessage" @keyup.enter="send" placeholder="Type a message..." :disabled="isSending"
        class="flex-1 px-4 py-2 border border-gray-300 rounded-full focus:outline-none focus:ring-2 focus:ring-blue-400 disabled:opacity-50" />
      <button @click="send" :disabled="isSending"
        class="px-4 py-2 bg-blue-500 text-white rounded-full hover:bg-blue-600 disabled:opacity-50 transition">
        Send
      </button>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { useRoute } from 'vue-router'
import { formatTime } from '../utils/date'
import { useChat } from '../composables/useChat'

const route = useRoute()
const conversationId = parseInt(route.params.id as string)

const { messages, newMessage, isLoading, isSending, errorMessage, currentUserId, send } =
  useChat(conversationId)
</script>
