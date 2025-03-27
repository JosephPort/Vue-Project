<script setup>
import { useAuthStore } from '../../stores/auth'
import { useRouter, RouterLink } from 'vue-router'
import { login } from '../../api/auth'
import { ref } from 'vue'

const authStore = useAuthStore()
const router = useRouter()

const username = ref('')
const password = ref('')

const isLoggedIn = authStore.isLoggedIn

const submitLogin = async (e) => {
  e.preventDefault()
  try {
    const request = await login(username.value, password.value)
    console.log(request)
    authStore.login(request)
    router.push('/')
  } catch (e) {
    console.error(e)
  }
}
</script>

<template>
  <div v-if="!isLoggedIn">
    <h1>Login</h1>
    <form @submit.prevent="submitLogin">
      <input v-model="username" type="text" placeholder="Username" required />
      <input v-model="password" type="password" placeholder="Password" required />
      <button type="submit">Login</button>
    </form>
  </div>
  <div v-else>
    <h1>You are already logged in</h1>
    <RouterLink to="/">Go to Home</RouterLink>
  </div>
</template>

<style scoped>
</style>