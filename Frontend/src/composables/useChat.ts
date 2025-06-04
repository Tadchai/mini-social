import { ref, onMounted } from 'vue'
import { jwtDecode } from 'jwt-decode'
import {
  startSignalRConnection,
  joinConversation,
  sendMessage,
  onReceiveMessage,
} from '@/services/signalrService'
import { GroupMessages } from '@/services/groupService'
import type { MessagesResponse } from '@/types/Group'

export function useChat(conversationId: number) {
  const messages = ref<MessagesResponse[]>([])
  const newMessage = ref('')
  const isLoading = ref(false)
  const isSending = ref(false)
  const errorMessage = ref<string | null>(null)
  const currentUserId = ref(0)

  const token = localStorage.getItem('token')
  if (token) {
    const decoded = jwtDecode<{ sub: string }>(token)
    currentUserId.value = parseInt(decoded.sub)
    console.log(currentUserId.value)
  }

  async function loadMessages() {
    isLoading.value = true
    errorMessage.value = null
    try {
      const result = await GroupMessages(conversationId)
      if (result.statusCode === 200 && result.data) {
        messages.value = result.data
      } else {
        errorMessage.value = result.message || 'Unknown error occurred.'
      }
    } catch (error) {
      console.error('LoadMessages error:', error)
      errorMessage.value = (error as Error).message
    } finally {
      isLoading.value = false
    }
  }

  async function send() {
    if (newMessage.value.trim() === '' || isSending.value) return
    try {
      isSending.value = true
      await sendMessage(conversationId, newMessage.value)
      newMessage.value = ''
    } catch (error) {
      console.error('Send message error:', error)
      errorMessage.value = 'Failed to send message.'
    } finally {
      isSending.value = false
    }
  }

  onMounted(async () => {
    await startSignalRConnection()
    await joinConversation(conversationId)
    await loadMessages()

    onReceiveMessage((msg) => {
      if (msg.conversationId === conversationId) {
        messages.value.push(msg)
      }
    })
  })

  return {
    messages,
    newMessage,
    isLoading,
    isSending,
    errorMessage,
    currentUserId,
    send,
  }
}
