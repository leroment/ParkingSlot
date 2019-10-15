<template>
  <v-container fill-height>
    <v-layout align-center justify-center>
      <v-flex style="overflow:auto" xs12 sm8 md4>
        <v-card>
          <v-card-text>
            <v-container>
              <form @submit.prevent="onLogin">
                <v-layout row>
                  <v-flex xs12>
                    <v-alert
                      dismissible
                      type="error"
                      :value="error"
                    >The username/password is incorrect</v-alert>
                    <v-text-field
                      name="username"
                      label="Username/Email"
                      id="username"
                      v-model="username"
                      type="text"
                      required
                    ></v-text-field>
                  </v-flex>
                </v-layout>
                <v-layout row>
                  <v-flex xs12>
                    <v-text-field
                      name="password"
                      label="Password"
                      id="password"
                      v-model="password"
                      type="password"
                      required
                    ></v-text-field>
                  </v-flex>
                </v-layout>
                <v-layout>
                  <v-flex xs12>
                    <v-btn color="primary" type="submit">Login</v-btn>
                  </v-flex>
                  <v-flex align-end>
                    <v-btn color="primary" type="submit" to="/main">Back</v-btn>
                  </v-flex>
                </v-layout>
              </form>
            </v-container>
          </v-card-text>
        </v-card>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
/* eslint-disable no-unused-vars */
export default {
  data() {
    return {
      username: "",
      password: "",
      confirmPassword: "",
      error: false
    };
  },
  methods: {
    onLogin() {
      /* Send a post req to the backend server */
      /* Set localStorage loginStatus to true */
      /* Redirect the user into the app */
      this.$store
        .dispatch("LOGIN", {
          Username: this.username,
          Password: this.password
        })
        .then(success => {
          this.$router.push("/home");
        })
        .catch(error => {
          this.error = true;
        });
    }
  }
};
</script>
