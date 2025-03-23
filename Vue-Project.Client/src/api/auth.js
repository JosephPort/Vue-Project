import { useStore } from 'pinia';
import store from '../stores/auth';

const authStore = useStore(store);

export default authStore;