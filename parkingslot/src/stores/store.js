import Vue from "vue";
import Vuex from "vuex";
import User from "./modules/user";
//Persist state management upon refresh
import createPersistedState from 'vuex-persistedstate';
//Encrypt localStorage data
import SecureLS from "secure-ls";
var ls = new SecureLS({ isCompression: false });

Vue.use(Vuex);

export default new Vuex.Store({
  modules: {
    user: User
  },
  plugins: [
    createPersistedState({
      storage: {
        getItem: key => ls.get(key),
        setItem: (key, value) => ls.set(key, value),
        removeItem: key => ls.remove(key)
      }
    })
  ],
});
