<script setup>
import { useAuthStore } from '../stores/auth'
import AuthHome from './AuthHome.vue'
import NotAuthHome from './NonAuthHome.vue'
const authStore = useAuthStore()

const isLoggedIn = authStore.isLoggedIn
</script>

<template>
  <div v-if="!isLoggedIn">
    <AuthHome />
  </div>
  <div v-else>
    <NotAuthHome />
  </div>
</template>

<style scoped>
</style>