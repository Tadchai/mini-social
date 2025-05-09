import * as signalR from '@microsoft/signalr'
const API_URL = import.meta.env.VITE_API_URL

let connection: signalR.HubConnection

export async function startSignalRConnection() {
  connection = new signalR.HubConnectionBuilder()
    .withUrl(`${API_URL}/chathub`)
    .withAutomaticReconnect()
    .build()

  await connection.start()
  console.log('SignalR Connected')
}

export async function joinConversation(conversationId: number) {
  await connection.invoke('JoinConversation', conversationId)
}

export async function sendMessage(
  conversationId: number,
  senderId: number,
  content: string,
  type: number = 0,
) {
  await connection.invoke('SendMessage', conversationId, senderId, content, type)
}

export async function onReceiveMessage(callback: (message: any) => void) {
  connection.on('ReceiveMessage', callback)
}
