<template>
  <div>
    <h2>Login</h2>
    <form @submit.prevent="login">
      <input v-model="username" placeholder="username" />
      <input v-model="password" type="password" placeholder="Password" />
      <button type="submit">Login</button>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import type { ApiLoginResponse, ApiResponse } from '../types/Response'
import { useRouter } from 'vue-router';

const API_URL = import.meta.env.VITE_API_URL;
const username = ref<String>('')
const password = ref<String>('')
const router = useRouter()

async function login(){
  try {
    const response = await fetch(`${API_URL}/Auth/Login`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        username: username.value,
        password: password.value
      })
    })
    const result: ApiResponse<ApiLoginResponse> = await response.json()
    if (result.statusCode === 200 && result.data?.token) {
      alert(result.message)
      localStorage.setItem('token', result.data.token)
      router.push('/')
    } else {
      alert(result.message)
    }
  } catch (error) {
    console.error('Error:', error)
  }

}
</script>
