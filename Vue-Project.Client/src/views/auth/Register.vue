<script setup>
import { useAuthStore } from '../../stores/auth'
import { useRouter } from 'vue-router'
import { register } from '../../api/auth'
import { ref } from 'vue'

const authStore = useAuthStore()
const router = useRouter()

const formData = ref({
  username: '',
  password: '',
  email: '',
  firstName: '',
  lastName: ''
})

if (authStore.isLoggedIn) {
  router.push('/')
}

const submitRegister = async (e) => {
  e.preventDefault()
  try {
    const request = await register(formData.value)
    console.log(request.data)
    authStore.login(request.data)
    router.push('/')
  } catch (e) {
    console.error(e.response.data)
  }
}
</script>

<template>
  <div>
    <h1>Register</h1>
    <form @submit="submitRegister">
      <input type="text" v-model="formData.username" placeholder="Username" />
      <input type="password" v-model="formData.password" placeholder="Password" />
      <input type="email" v-model="formData.email" placeholder="Email" />
      <input type="text" v-model="formData.firstName" placeholder="First Name" />
      <input type="text" v-model="formData.lastName" placeholder="Last Name" />
      <button type="submit">Register</button>
    </form>
  </div>
</template>

<style scoped>
</style>