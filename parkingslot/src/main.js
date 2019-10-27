import Vue from 'vue'
import App from './App.vue'
import router from './router'

//Initialize all the Javascript Libraries here
/* 1. Promise based HHTP client for the browser */
import axios from 'axios'
import VueAxios from 'vue-axios'
/* 2. Vue Material Design Component Framework*/
import vuetify from './plugins/vuetify';
/* 3. Vuex state management */
import store from './stores/store'
/* 4. Vue2-google-maps */
import * as VueGoogleMaps from 'vue2-google-maps'
/* 5. Gmap clusters */
import GmapCluster from 'vue2-google-maps/dist/components/cluster'
/* 6. Infinite loading */
import InfiniteLoading from 'vue-infinite-loading';

// Install Javascript Libraries
// Must be called before new Vue()
Vue.use(VueAxios, axios);
Vue.use(VueGoogleMaps, {
  installComponents: true,
  load: {
    key: 'AIzaSyA78_ixDHcOmN-gmzu5nYVkBcFD0A8vkw8',
    libraries: 'places'
  }
})
Vue.component('GmapCluster', GmapCluster)
Vue.use(InfiniteLoading, {
  props: {
    spinner: 'default',
    /* other props need to configure */
  },
  system: {
    throttleLimit: 500,
    /* other settings need to configure */
  },
});

Vue.config.productionTip = false

new Vue({
  router,
  vuetify,
  store,
  render: h => h(App)
}).$mount('#app')
