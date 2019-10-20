<template>
  <v-app class="grey lighten-4">
    <navbar></navbar>
      <router-view></router-view>
    <tabbar v-show="this.$store.getters.ISLOGGEDIN"></tabbar>
  </v-app>
</template>
<style>
@import url("./assets/style.css");
</style>
<script>
import navbar from "./components/layouts/navbar";
import tabbar from "./components/layouts/tabbar";
export default {
  components: {
    navbar,
    tabbar
  },
};
</script>