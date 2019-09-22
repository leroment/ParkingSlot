import Vue from 'vue';
import Router from 'vue-router';
const Main = () => import('@/components/main.vue');
const Home = () => import('@/components/home.vue');
const Login = () => import('@/components/login.vue');
const Register = () => import('@/components/register.vue');
const Profile = () => import('@/components/profile.vue');
const Favourite = () => import('@/components/favourite.vue');
const Map = () => import('@/components/map.vue');

Vue.use(Router);


export default new Router({
  routes: [
    {
      name: 'default',
      path: '',
      component: Main
    },
    {
      name: 'main',
      path: '/main',
      component: Main
    },
    {
      name: 'home',
      path: '/home',
      component: Home
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
      name: 'favourite',
      path: '/favourite',
      component: Favourite
    },
    {
      name: 'map',
      path: '/map',
      component: Map
    }
  ],
  mode: 'history'
});
