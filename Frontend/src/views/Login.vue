<template>
  <div>
    <h2>Login</h2>
    <form @submit.prevent="login">
      <input v-model="username" placeholder="username" />
      <input v-model="password" type="password" placeholder="Password" />
      <br />
      <button type="submit">Login</button>
    </form>
    <hr />
    <button @click="openModal">Register</button>
    <Modal ref="modal">
      <h2>Register</h2>
      <form @submit.prevent="Register">
        <div>Username</div>
        <input v-model="username_reg" placeholder="username" />
        <div>Email</div>
        <input v-model="email_reg" type="email" placeholder="Email" />
        <div>Password</div>
        <input v-model="password_reg" type="password" placeholder="Password" />
        <div>ConfirmPassword</div>
        <input v-model="confirmpassword_reg" type="password" placeholder="Confirmpassword" />
        <br />
        <button type="submit">Register</button>
        <button @click="closeModal">close</button>
      </form>
    </Modal>
  </div>
</template>

<script setup lang="ts">
import { ref, useTemplateRef } from 'vue'
import type { ApiLoginResponse, ApiResponse } from '../types/Response'
import { useRouter } from 'vue-router'
import Modal from '../components/Modal.vue'

const API_URL = import.meta.env.VITE_API_URL
const username = ref<String>('')
const password = ref<String>('')
const username_reg = ref<String>('')
const email_reg = ref<String>('')
const password_reg = ref<String>('')
const confirmpassword_reg = ref<String>('')
const router = useRouter()
const modal = useTemplateRef<InstanceType<typeof Modal>>('modal')

function openModal() {
  modal.value?.openModal()
}
function closeModal() {
  username_reg.value = ''
  email_reg.value = ''
  password_reg.value = ''
  confirmpassword_reg.value = ''
  modal.value?.closeModal()
}

async function login() {
  try {
    const response = await fetch(`${API_URL}/Auth/Login`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        username: username.value,
        password: password.value,
      }),
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
async function Register() {
  try {
    const response = await fetch(`${API_URL}/Auth/Register`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        username: username_reg.value,
        email: email_reg.value,
        password: password_reg.value,
        confirmPassword: confirmpassword_reg.value,
      }),
    })
    const result: ApiResponse<ApiLoginResponse> = await response.json()
    if (result.statusCode === 201) {
      alert(result.message)
      closeModal()
    } else {
      alert(result.message)
    }
  } catch (error) {
    console.error('Error:', error)
  }
}
</script>
