<template>
  <v-container fill-height>
    <v-layout align-center justify-center>
      <v-flex style="overflow:auto" xs12 sm8 md4>
        <v-card>
          <v-card-text>
            <v-alert dismissible type="error" :value="passwordError">The password does not match</v-alert>
            <v-alert dismissible type="error" :value="usernameError">This username is already taken</v-alert>
            <v-container>
              <form @submit.prevent="register">
                <v-layout row>
                  <v-flex xs12>
                    <v-text-field
                      name="firstname"
                      label="First Name"
                      id="firstName"
                      v-model="firstName"
                      type="text"
                      required
                    ></v-text-field>
                  </v-flex>
                </v-layout>
                <v-layout row>
                  <v-flex xs12>
                    <v-text-field
                      name="lastname"
                      label="Last Name"
                      id="lastName"
                      v-model="lastName"
                      type="text"
                      required
                    ></v-text-field>
                  </v-flex>
                </v-layout>
                <v-layout row>
                  <v-flex xs12>
                    <v-text-field
                      name="username"
                      label="Username"
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
                      name="email"
                      label="Email"
                      id="email"
                      v-model="email"
                      type="email"
                      required
                    ></v-text-field>
                  </v-flex>
                </v-layout>
                <v-layout row>
                  <v-flex xs12>
                    <v-text-field
                      name="phoneNo"
                      label="Phone Number"
                      id="phoneNo"
                      v-model="phoneNo"
                      type="tel"
                      pattern="[0-9]{8}" required
                    ></v-text-field>
                  </v-flex>
                </v-layout>
                <v-layout row>
                  <v-flex xs12>
                    <v-text-field
                      name="role"
                      label="Role"
                      id="role"
                      v-model="role"
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
                <v-layout row>
                  <v-flex xs12>
                    <v-text-field
                      name="confirmPassword"
                      label="Confirm Password"
                      id="confirmPassword"
                      v-model="confirmPassword"
                      type="password"
                    ></v-text-field>
                  </v-flex>
                </v-layout>
                <v-layout row>
                  <v-flex xs12>
                    <v-btn color="primary" type="submit">Sign up</v-btn>
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
      firstName: "",
      lastName: "",
      username: "",
      email: "",
      phoneNo: "",
      role: "",
      password: "",
      confirmPassword: "",
      passwordError: false,
      usernameError: false
    };
  },
  methods: {
    register() {
      if (this.password != this.confirmPassword){
        this.passwordError = true;
        this.usernameError = false;
        return;
      }
      this.passwordError = false;
      this.$store
        .dispatch("REGISTER", {
          FirstName: this.firstName,
          LastName: this.lastName,
          Username: this.username,
          Email: this.email,
          PhoneNumber: this.phoneNo,
          Role: this.role,
          Password: this.password
        })
        .then(success => {
          this.$router.push("/login");
        })
        .catch(error => {
          if (error.response.status == 400){
            this.usernameError = true;
          }
          else this.usernameError = true;
        });
    }
  }
};
</script>
