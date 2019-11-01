<template>
  <v-content>
    <v-container fluid>
      <v-layout column>
        <v-card>
          <v-alert
            type="error"
            :value="passwordError"
          >Confirm Password is not the same as new password!</v-alert>
          <v-alert
            :value="notifyStatus"
            dismissible
            type="success"
          >Your password have been changed successfully!</v-alert>
          <v-card-text>
            <v-text-field
              name="password"
              v-model="userPassword.newpassword"
              label="New password"
              type="password"
            ></v-text-field>
            <v-text-field
              name="cfmpassword"
              v-model="userPassword.cfmnewpassword"
              label="Confirm new password"
              type="password"
            ></v-text-field>
          </v-card-text>
          <v-card-actions>
            <v-btn color="primary" v-if="!updated" @click="changePassword">
              <v-icon left dark>mdi-check</v-icon>Change Password
            </v-btn>
            <v-btn color="primary" v-if="updated" to="/login">
              <v-icon left dark>mdi-check</v-icon>Login
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-layout>
    </v-container>
  </v-content>
</template>

<script>
export default {
  data() {
    return {
      updated: false,
      userPassword: {
        newpassword: "",
        cfmnewpassword: ""
      },
      notifyStatus: false,
      passwordError: false
    };
  },
  created: function() {
    var token = this.$route.params.token;
    this.$store.dispatch("CHECKTOKEN", token).catch(error => {
      this.$router.push("/main");
      this.error = true;
    });
  },
  methods: {
    changePassword() {
      //update password
      var userId = this.$route.params.userId;
      var update = {
        Id: userId,
        NewPassword: this.userPassword.newpassword
      };
      this.$store.dispatch("SETNEWPASSWORD", update).then(response => {
        if (response == true) {
          this.notifyStatus = true;
          this.updated = true;
        }
      });
    }
  }
};
</script>
