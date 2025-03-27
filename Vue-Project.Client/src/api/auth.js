import axios from 'axios';
import { useAuthStore } from '../stores/auth';

const getUserData = async () => {
  const authStore = useAuthStore();
  try {
    const userData = await fetchUserData();
    authStore.login(userData);
  } catch (error) {
    try {
      await fetchRefreshToken();
      const userData = await fetchUserData();
      authStore.login(userData);
    } catch (e) {
      console.error(e.response.data);
      return null;
    }
  }
};

const fetchRefreshToken = async () => {
  await axios.post('https://localhost:7167/api/auth/refresh', {}, {
    withCredentials: true
  });
};

const fetchUserData = async () => {
  const response = await axios.get('https://localhost:7167/api/auth/user', {
    withCredentials: true
  });
  return response.data;
};

const login = async (username, password) => {
  const response = await axios.post('https://localhost:7167/api/auth/login', {
    username,
    password
  }, {
    withCredentials: true
  });
  return response.data;
}

const register = async (data) => {
  const response = await axios.post('https://localhost:7167/api/auth/register', data, {
    withCredentials: true
  });
  return response.data;
}

const logout = async () => {
  await axios.post('https://localhost:7167/api/auth/logout', {}, {
    withCredentials: true
  });
}
export { getUserData, login, register, logout };