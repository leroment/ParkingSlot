import Vue from 'vue';
import Router from 'vue-router';
const Main = () => import('@/components/main.vue');
const Login = () => import('@/components/login.vue');
const Register = () => import('@/components/register.vue');
const Profile = () => import('@/components/profile.vue');
const Map = () => import('@/components/map.vue');

Vue.use(Router);

export default new Router({
  routes: [
    {
      name: 'main',
      path: '',
      component: Main
    },
    {
      name: 'login',
      path: '/login',
      component: Login
    },
    {
      name: 'register',
      path: '/register',
      component: Register
    },
    {
      name: 'profile',
      path: '/profile',
      component: Profile
    },
    {
      name: 'map',
      path: '/map',
      component: Map
    }
  ],
  mode: 'history'
});
