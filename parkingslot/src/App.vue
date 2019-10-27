<template>
  <v-app class="grey lighten-4 box">
    <navbar></navbar>
    <router-view class="content"></router-view>
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
  }
};
</script>
<style>
.box {
  height: 100% !important;
  width: 100% !important;
}
#google-map-container {
  margin-top: 48px;
  margin-bottom: 42px;
}
</style>