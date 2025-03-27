<script setup>
import { useAuthStore } from '../stores/auth'
const authStore = useAuthStore()
</script>

<template>
  <div>
    <h1>Auth Home</h1>
    <p>
      Welcome, {{ authStore.user?.username }}
    </p>
  </div>
</template>

<style scoped>
</style>