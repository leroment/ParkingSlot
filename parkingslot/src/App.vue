<template>
  <v-app class="grey lighten-4">
    <navbar></navbar>
    <v-content>
      <router-view></router-view>
    </v-content>
    <tabbar v-show="login"></tabbar>
  </v-app>
</template>
<style>
@import url("./assets/style.css");
</style>
<script>
import navbar from "./components/navbar";
import tabbar from "./components/tabbar";
export default {
  components: {
    navbar,
    tabbar
  },
  computed: {
    login() {
      let status = this.$store.getters.ISLOGGEDIN;
      return status;
    }
  },
};
</script>