import Vue from 'vue';
import Router from 'vue-router';
import store from './stores/store'
const Main = () => import('@/components/main.vue');
const Home = () => import('@/components/home.vue');
const Login = () => import('@/components/auth/login.vue');
const Register = () => import('@/components/auth/register.vue');
const ForgetPassword = () => import('@/components/auth/forgetpassword.vue');
const Profile = () => import('@/components/profile.vue');
const Favourite = () => import('@/components/favourite.vue');
const Map = () => import('@/components/map.vue');

Vue.use(Router);

const ifNotAuthenticated = (to, from, next) => {
  if (store.getters.ISLOGGEDIN == false) {
    next()
    return
  }
  next('/')
}

const ifAuthenticated = (to, from, next) => {
  if (store.getters.ISLOGGEDIN) {
    next()
    return
  }
  next('/login')
}

const redirect = (to, from, next) => {
  if (store.getters.ISLOGGEDIN) {
    next('/home')
    return
  }
  next('/main')
}

export default new Router({
  routes: [
    {
      name: 'default',
      path: '',
      component: Main,
      beforeEnter: redirect
    },
    {
      name: 'main',
      path: '/main',
      component: Main,
    },
    {
      name: 'login',
      path: '/login',
      component: Login,
      beforeEnter: ifNotAuthenticated,
    },
    {
      name: 'register',
      path: '/register',
      component: Register,
      beforeEnter: ifNotAuthenticated,
    },
    {
      name: 'home',
      path: '/home',
      component: Home,
      beforeEnter: ifAuthenticated,
    },
    {
      name: 'profile',
      path: '/profile',
      component: Profile,
      beforeEnter: ifAuthenticated,
    },
    {
      name: 'favourite',
      path: '/favourite',
      component: Favourite,
      beforeEnter: ifAuthenticated,
    },
    {
      name: 'map',
      path: '/map',
      component: Map,
      beforeEnter: ifAuthenticated,
    },
    {
      name: 'resetpassword',
      path: '/resetpassword/:userId/:token',
      component: ForgetPassword,
      beforeEnter: ifNotAuthenticated,
    },
    { path: '*', redirect: '/home' }
  ],
  mode: 'history'
});