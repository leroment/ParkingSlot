<template>
  <v-content>
    <v-container fluid>
      <v-layout column>
        <v-card>
          <v-alert type="error" :value="passwordError">Confirm Password is not the same as new password!</v-alert>
          <v-alert :value="notifyStatus" dismissible type="success">Your password have been changed successfully!</v-alert>
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
            <v-btn color="primary" @click="changePassword">
              <v-icon left dark>mdi-check</v-icon>Change Password
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
      userPassword: {
        newpassword: "",
        cfmnewpassword: ""
      },
      notifyStatus: false,
      passwordError: false,
    };
  },
  created: function() {
    var userId = this.$route.params.userId;
    var token = this.$route.params.token;
    this.axios.post(
      "https://parkingslotapi.azurewebsites.net/api/users/ConfirmPassword",
      {
        Id: this.$route.params.userId,
        Token: token
      }
    ).then(response => {
        console.log(response.status);
        if(response.status == "200"){
           //route user to the resetpassword page
        }
        else{
          //route user to page not found
           this.$router.push("/main");
        }
    });
  },
  methods: {
    changePassword() {
      //update password
    }
  }
};
</script>
