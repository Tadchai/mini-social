<template>
  <div class="container">
    <h2>Login</h2>
    <form @submit.prevent="login">
      <label for="username">Username:</label><br />
      <input id="username" v-model="username" placeholder="username" /><br />
      <label for="password">Password:</label><br />
      <input id="password" v-model="password" type="password" placeholder="Password" />
      <br />
      <button type="submit">Login</button>
    </form>
    <hr />
    <button @click="openModal">Register</button>
    <Modal ref="modal">
      <h2>Register</h2>
      <form @submit.prevent="Register">
        <label for="username_reg">Username:</label>
        <input id="username_reg" v-model="username_reg" placeholder="username" />
        <label for="email_reg">Email:</label>
        <input id="email_reg" v-model="email_reg" type="email" placeholder="Email" />
        <label for="password_reg">Password:</label>
        <input id="password_reg" v-model="password_reg" type="password" placeholder="Password" />
        <label for="confirmpassword_reg">ConfirmPassword:</label>
        <input
          id="confirmpassword_reg"
          v-model="confirmpassword_reg"
          type="password"
          placeholder="Confirmpassword"
        />
        <br />
        <button type="submit">Register</button>
        <button @click="closeModal">close</button>
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

<style scoped>
.container {
  max-width: 400px;
  width: 100%;
  border-radius: 8px;
  border: 1px solid black;
  padding: 20px;
}

input {
  width: 100%;
  padding: 10px;
  margin-bottom: 15px;
  border-radius: 8px;
  border: 1px solid #ccc;
  box-sizing: border-box;
}

button {
  padding: 10px 20px;
  border: none;
  background-color: #2e86de;
  color: white;
  border-radius: 8px;
  cursor: pointer;
  margin-top: 10px;
}

button:hover {
  background-color: #1b4f72;
}
</style>
