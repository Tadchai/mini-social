<template>
  <div class="chat-container">
    <div class="messages">
      <div v-if="isLoading">Loading messages...</div>
      <div v-else-if="errorMessage">{{ errorMessage }}</div>
      <div v-else v-for="msg in messages" :key="msg.id"
        :class="['message', msg.senderId === currentUserId ? 'sent' : 'received']">
        <span>{{ msg.content }}</span>
        <small>{{ formatTime(msg.createdAt) }}</small>
      </div>
    </div>
    <div class="input-area">
      <input v-model="newMessage" @keyup.enter="send" placeholder="Type a message..." :disabled="isSending" />
      <button @click="send" :disabled="isSending">Send</button>
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

<style scoped>
.chat-container {
  display: flex;
  flex-direction: column;
  height: 100%;
}

.messages {
  flex: 1;
  overflow-y: auto;
  padding: 1rem;
}

.message {
  margin-bottom: 0.5rem;
  padding: 0.5rem 1rem;
  border-radius: 20px;
  max-width: 70%;
  word-wrap: break-word;
}

.sent {
  background-color: #dcf8c6;
  align-self: flex-end;
}

.received {
  background-color: #ececec;
  color: black;
  align-self: flex-start;
}

.input-area {
  display: flex;
  padding: 0.5rem;
}

input {
  flex: 1;
  padding: 0.5rem;
  border-radius: 20px;
  border: 1px solid #ccc;
}

button {
  margin-left: 0.5rem;
  padding: 0.5rem 1rem;
  border-radius: 20px;
}
</style>
