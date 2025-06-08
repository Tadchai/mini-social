<template>
  <div class="max-w-md w-full mx-auto rounded-lg border border-black p-6">
    <h2 class="text-2xl font-semibold mb-4">Login</h2>
    <form @submit.prevent="login" class="space-y-4">
      <div>
        <label for="username" class="block text-sm font-medium">Username:</label>
        <input id="username" v-model="username" placeholder="username"
          class="w-full px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500" />
      </div>

      <div>
        <label for="password" class="block text-sm font-medium">Password:</label>
        <input id="password" v-model="password" type="password" placeholder="Password"
          class="w-full px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500" />
      </div>

      <button type="submit" class="w-full py-2 bg-blue-600 text-white rounded-md hover:bg-blue-800 transition">
        Login
      </button>
    </form>

    <hr class="my-6" />

    <button @click="openModal" class="w-full py-2 bg-gray-500 text-white rounded-md hover:bg-gray-700 transition">
      Register
    </button>

    <Modal ref="modal">
      <h2 class="text-xl font-semibold mb-4">Register</h2>
      <form @submit.prevent="Register" class="space-y-4">
        <div>
          <label for="username_reg" class="block text-sm font-medium">Username:</label>
          <input id="username_reg" v-model="username_reg" placeholder="username"
            class="w-full px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500" />
        </div>

        <div>
          <label for="email_reg" class="block text-sm font-medium">Email:</label>
          <input id="email_reg" v-model="email_reg" type="email" placeholder="Email"
            class="w-full px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500" />
        </div>

        <div>
          <label for="password_reg" class="block text-sm font-medium">Password:</label>
          <input id="password_reg" v-model="password_reg" type="password" placeholder="Password"
            class="w-full px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500" />
        </div>

        <div>
          <label for="confirmpassword_reg" class="block text-sm font-medium">Confirm Password:</label>
          <input id="confirmpassword_reg" v-model="confirmpassword_reg" type="password" placeholder="Confirmpassword"
            class="w-full px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500" />
        </div>

        <div class="flex gap-2 mt-4">
          <button type="submit" class="flex-1 py-2 bg-green-600 text-white rounded-md hover:bg-green-800 transition">
            Register
          </button>
          <button @click="closeModal" type="button"
            class="flex-1 py-2 bg-gray-400 text-white rounded-md hover:bg-gray-600 transition">
            Close
          </button>
        </div>
      </form>
    </Modal>
  </div>
</template>

<script setup lang="ts">
import { ref, useTemplateRef } from 'vue'
import { useRouter } from 'vue-router'
import Modal from '../components/Modal.vue'
import { loginUser, registerUser } from '../services/authService'

const router = useRouter()
const modal = useTemplateRef<InstanceType<typeof Modal>>('modal')

const username = ref<string>('')
const password = ref<string>('')
const username_reg = ref<string>('')
const email_reg = ref<string>('')
const password_reg = ref<string>('')
const confirmpassword_reg = ref<string>('')

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
    const result = await loginUser(username.value, password.value)
    if (result.statusCode === 200 && result.data) {
      alert(result.message)
      localStorage.setItem('token', result.data)
      router.push('/')
    } else {
      alert(result.message)
    }
  } catch (error) {
    console.error('Login error:', error)
  }
}

async function Register() {
  try {
    const result = await registerUser(
      username_reg.value,
      email_reg.value,
      password_reg.value,
      confirmpassword_reg.value,
    )
    if (result.statusCode === 201) {
      alert(result.message)
      closeModal()
    } else {
      alert(result.message)
    }
  } catch (error) {
    console.error('Register error:', error)
  }
}
</script>
