// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from 'vue';
import App from './App';
import router from './router/index.js';

// Initialise all the Javascript framework here
import Vuetify from 'vuetify';

// Styling
import 'vuetify/dist/vuetify.min.css';

// Use the plugin
// Must be called before new Vue() function
Vue.use(Vuetify);

// Development mode
Vue.config.productionTip = false;

/* eslint-disable no-new */
new Vue({
  el: '#app',
  router,
  components: { App },
  template: '<App/>'
});
