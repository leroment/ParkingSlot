  
<template>
  <div class="navbar">
    <!-- Navbar -->
    <v-app-bar app dense fixed dark color="primary">
      <v-app-bar-nav-icon @click="navdraw = !navdraw"></v-app-bar-nav-icon>
      <v-spacer></v-spacer>
      <v-toolbar-title>
        <router-link to="/" tag="span" class="tb-title">ParkingSlot</router-link>
      </v-toolbar-title>
      <div class="flex-grow-1"></div>
      <template>
        <v-btn icon>
          <v-icon>mdi-comment-question-outline</v-icon>
        </v-btn>
      </template>
    </v-app-bar>
    <v-navigation-drawer absolute temporary v-model="navdraw" app>
      <template v-slot:prepend v-if="login">
        <v-list-item two-line>
          <v-list-item-avatar>
            <img src="https://randomuser.me/api/portraits/men/81.jpg" />
          </v-list-item-avatar>
          <v-list-item-content>
            <v-list-item-title>{{ username }}</v-list-item-title>
            <v-list-item-subtitle>Logged In</v-list-item-subtitle>
          </v-list-item-content>
        </v-list-item>
        <v-divider></v-divider>
      </template>
      <!-- Login view sidebar -->
      <v-list dense v-if="login">
        <v-list-item @click="redirect('home')">
          <v-list-item-icon>
            <v-icon>mdi-home</v-icon>
          </v-list-item-icon>
          <v-list-item-content>
            <v-list-item-title>Home</v-list-item-title>
          </v-list-item-content>
        </v-list-item>
        <v-list-item @click="redirect('profile')">
          <v-list-item-icon>
            <v-icon>mdi-face</v-icon>
          </v-list-item-icon>
          <v-list-item-content>
            <v-list-item-title>Profile</v-list-item-title>
          </v-list-item-content>
        </v-list-item>
        <v-list-item @click="redirect('favourite')">
          <v-list-item-icon>
            <v-icon>mdi-heart</v-icon>
          </v-list-item-icon>
          <v-list-item-content>
            <v-list-item-title>Favourite</v-list-item-title>
          </v-list-item-content>
        </v-list-item>
        <v-list-item @click="redirect('map')">
          <v-list-item-icon>
            <v-icon>mdi-map-marker</v-icon>
          </v-list-item-icon>
          <v-list-item-content>
            <v-list-item-title>Map</v-list-item-title>
          </v-list-item-content>
        </v-list-item>
        <v-divider></v-divider>
        <v-list-item @click="logout()">
          <v-list-item-icon>
            <v-icon>mdi-lock-open</v-icon>
          </v-list-item-icon>
          <v-list-item-content>
            <v-list-item-title>Logout</v-list-item-title>
          </v-list-item-content>
        </v-list-item>
        <br />
      </v-list>

      <!-- Not logged in view sidebar -->
      <v-list dense v-if="!login">
        <v-list-item @click="redirect('login')">
          <v-list-item-icon>
            <v-icon>mdi-account</v-icon>
          </v-list-item-icon>
          <v-list-item-content>
            <v-list-item-title>Login</v-list-item-title>
          </v-list-item-content>
        </v-list-item>
        <v-list-item @click="redirect('register')">
          <v-list-item-icon>
            <v-icon>mdi-playlist-plus</v-icon>
          </v-list-item-icon>
          <v-list-item-content>
            <v-list-item-title>Register</v-list-item-title>
          </v-list-item-content>
        </v-list-item>
        <v-divider></v-divider>
        <v-list-item @click="redirect('information')">
          <v-list-item-icon>
            <v-icon>mdi-information</v-icon>
          </v-list-item-icon>
          <v-list-item-content>
            <v-list-item-title>More information</v-list-item-title>
          </v-list-item-content>
        </v-list-item>
      </v-list>
    </v-navigation-drawer>
  </div>
</template>

<script>
export default {
  data() {
    return {
      navdraw: false
    };
  },
  computed: {
    login() {
      let status = this.$store.getters.ISLOGGEDIN;
      return status;
    },
    username() {
      let username = this.$store.getters.USERNAME;
      return username;
    }
  },
  methods: {
    redirect(url) {
      this.$router.push("/" + url);
    },
    logout() {
      this.$store.commit("SETAUTHSTATUS", false);
      this.$router.push({ path: "/login" });
    }
  }
};
</script>