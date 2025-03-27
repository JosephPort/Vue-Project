<script setup>
import { useAuthStore } from '../stores/auth'
import { RouterLink } from 'vue-router'
const authStore = useAuthStore();

const isLoggedIn = authStore.isLoggedIn;
</script>

<template>
  <nav>
    <div v-if="isLoggedIn">
      <button @click="authStore.logout">Logout</button>
    </div>
    <div v-else>
      <RouterLink to="/auth/login">Login</RouterLink>
      <RouterLink to="/auth/register">Register</RouterLink>
    </div>
  </nav>
</template>

<style scoped>
</style>