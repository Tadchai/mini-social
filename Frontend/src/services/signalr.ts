import * as signalR from '@microsoft/signalr'
const API_URL = import.meta.env.VITE_API_URL

let connection: signalR.HubConnection

export async function startSignalRConnection() {
  connection = new signalR.HubConnectionBuilder()
    .withUrl(`${API_URL}/chathub`, {
      accessTokenFactory: () => localStorage.getItem('token') || '',
      withCredentials: true,
      transport: signalR.HttpTransportType.WebSockets,
    })
    .withAutomaticReconnect()
    .build()

  await connection.start()
  console.log('SignalR Connected')
}

export async function joinConversation(conversationId: number) {
  console.log("Joining conversation:", conversationId);
  await connection.invoke('JoinConversation', conversationId)
}

export async function sendMessage(conversationId: number, content: string) {
  await connection.invoke('SendMessage', conversationId, content)
}

export async function onReceiveMessage(callback: (message: any) => void) {
  connection.on('ReceiveMessage', callback)
}
