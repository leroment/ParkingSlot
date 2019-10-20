<template>
  <v-content>
    <v-container fluid>
      <v-layout column>
        <v-card>
          <v-alert :value="notifyStatus" dismissible type="success">{{notifyText}}</v-alert>
          <v-card-text v-show="updateAccount">
            <v-flex class="mb-6">
              <v-avatar size="96" class="mr-4">
                <img src="https://randomuser.me/api/portraits/men/81.jpg" alt="Avatar" />
              </v-avatar>
            </v-flex>
            <v-text-field name="username" label="Username" v-model="userProfile.username"></v-text-field>
            <v-text-field name="firstName" label="First Name" v-model="userProfile.firstName"></v-text-field>
            <v-text-field name="lastName" label="Last Name" v-model="userProfile.lastName"></v-text-field>
            <v-text-field name="email" label="Email Address" v-model="userProfile.Email"></v-text-field>
            <v-text-field name="contact" label="Contact Number" v-model="userProfile.Contact"></v-text-field>
          </v-card-text>
          <v-card-text v-show="!updateAccount">
            <v-text-field
              name="password"
              v-model="userPassword.currentpassword"
              label="Current password"
              type="password"
            ></v-text-field>
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
          <v-card-actions v-show="updateAccount">
            <v-btn color="primary" @click="updateProfile">
              <v-icon left dark>mdi-check</v-icon>Save Changes
            </v-btn>
            <v-btn color="warning" @click="reset">
              <v-icon left dark>mdi-lock-reset</v-icon>Change Password
            </v-btn>
          </v-card-actions>
          <v-card-actions v-show="!updateAccount">
            <v-btn color="primary" @click="updatePassword">
              <v-icon left dark>mdi-check</v-icon>Update Password
            </v-btn>
            <v-btn color="warning" @click="reset">
              <v-icon left dark>mdi-arrow-left</v-icon>Back
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
      userProfile: {
        username: this.$store.getters.USERNAME,
        firstName: this.$store.getters.FIRSTNAME,
        lastName: this.$store.getters.LASTNAME,
        Email: this.$store.getters.EMAIL,
        Contact: this.$store.getters.PHONENO
      },
      userPassword: {
        currentpassword: "",
        newpassword: "",
        cfmnewpassword: ""
      },
      updateAccount: true,
      notifyStatus: false,
      notifyText: ""
    };
  },
  methods: {
    updateProfile() {
      /* Axios Post Req to update */
      /* change state back to*/
      /* update vuex state for username */
      this.notifyStatus = true;
      this.notifyText = "Profile have been successfully updated!";
    },
    updatePassword() {
      /* Axios Post Req to update */
      /* change state back to*/
      this.notifyStatus = true;
      this.notifyText = "Password have been successfully updated!";
      this.updateAccount = true;
    },
    reset() {
      this.updateAccount = !this.updateAccount;
      this.notifyStatus = false;
    }
  }
};
</script>
