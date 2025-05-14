<template>
  <div class="chat-container">
    <div class="messages">
      <div
        v-for="msg in messages"
        :key="msg.id"
        :class="['message', msg.senderId === currentUserId ? 'sent' : 'received']"
      >
        <span>{{ msg.content }}</span>
        <small>{{ formatTime(msg.createdAt) }}</small>
      </div>
    </div>
    <div class="input-area">
      <input v-model="newMessage" @keyup.enter="send" placeholder="Type a message..." />
      <button @click="send">Send</button>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { jwtDecode } from 'jwt-decode'
import {
  startSignalRConnection,
  joinConversation,
  sendMessage,
  onReceiveMessage,
} from '../services/signalr'
import type { TokenPayload } from '../types/Chat'

const route = useRoute()
const conversationId = parseInt(route.params.id as string)

const token = localStorage.getItem('token')
const currentUserId = ref(0)

if (token) {
  const decoded = jwtDecode<TokenPayload>(token)
  currentUserId.value = parseInt(decoded.nameid)
}

const messages = ref<any[]>([])
const newMessage = ref('')

async function send() {
  if (newMessage.value.trim() === '') return
  await sendMessage(conversationId, newMessage.value)
  newMessage.value = ''
}

function formatTime(timestamp: string) {
  return new Date(timestamp).toLocaleTimeString()
}

onMounted(async () => {
  await startSignalRConnection()
  await joinConversation(conversationId)

  onReceiveMessage((msg) => {
    if (msg.conversationId === conversationId) {
      messages.value.push(msg)
    }
  })
})
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
