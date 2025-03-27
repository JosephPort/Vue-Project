<script setup>
import NavBar from './components/NavBar.vue'
import { RouterView } from 'vue-router'
import { useAuthStore } from './stores/auth'
const authStore = useAuthStore()

</script>

<template>
  <NavBar />
  <RouterView />
</template>

<style scoped>

</style>
