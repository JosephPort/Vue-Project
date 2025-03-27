import { defineStore } from 'pinia';
import { getUserData, logout } from '../api/auth';

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: null,
  }),
  actions: {
    login(user) {
      this.user = user;
    },
    logout() {
      this.user = null;
      logout();
    },
    getUserData() {
      this.user = getUserData();
    },
  },
  getters: {
    isLoggedIn() {
      return !!this.user;
    },
  },
});