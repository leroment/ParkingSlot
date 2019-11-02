<template>
  <v-content v-resize="onResize" column>
    <v-data-table
      :headers="headers"
      :items="users"
      item-key="id"
      sort-by="username"
      :options.sync="pagination"
      :server-items-length="totalUsers"
      :loading="loading"
      :hide-default-header="isMobile"
      :class="{mobile: isMobile}"
      v-model="selected"
    >
      <template v-slot:top>
        <v-toolbar flat>
          <v-dialog v-model="showCreate" max-width="1000px">
            <template v-slot:activator="{ on }">
              <v-spacer></v-spacer>
              <v-btn
                color="primary"
                dark
                class="ml-5 mb-2 mt-3"
                v-on="on"
                @click="createUser"
              >Create User</v-btn>
            </template>
            <v-card>
              <v-card-title>
                <span class="headline">Create User</span>
              </v-card-title>
              <v-divider></v-divider>
              <v-card-text class="mt-2">
                <v-container>
                  <v-form ref="createform">
                    <v-row>
                      <v-col cols="12" sm="6" md="6">
                        <v-text-field v-model="newUser.username" label="Username" required></v-text-field>
                      </v-col>
                      <v-col cols="12" sm="6" md="6">
                        <v-text-field v-model="newUser.phoneNumber" label="Phone Number" required></v-text-field>
                      </v-col>
                    </v-row>
                    <v-row>
                      <v-col cols="12">
                        <v-text-field v-model="newUser.email" label="Email"></v-text-field>
                      </v-col>
                    </v-row>
                    <v-row>
                      <v-col cols="12" sm="6" md="6">
                        <v-text-field v-model="newUser.firstName" label="First Name" required></v-text-field>
                      </v-col>
                      <v-col cols="12" sm="6" md="6">
                        <v-text-field v-model="newUser.lastName" label="Last Name" required></v-text-field>
                      </v-col>
                    </v-row>
                    <v-row>
                      <v-col cols="12" sm="6" md="6">
                        <v-text-field v-model="newUser.password" label="Password" required></v-text-field>
                      </v-col>
                      <v-col cols="12" sm="6" md="6">
                        <v-text-field
                          v-model="newUser.confirmPassword"
                          label="Confirm Password"
                          required
                        ></v-text-field>
                      </v-col>
                    </v-row>
                  </v-form>
                </v-container>
              </v-card-text>
              <v-card-actions>
                <div class="flex-grow-1"></div>
                <v-btn color="blue darken-1" text @click="cancelCreate">Cancel</v-btn>
                <v-btn color="blue darken-1" text @click="submitUser">Create</v-btn>
              </v-card-actions>
            </v-card>
          </v-dialog>
        </v-toolbar>
        <v-dialog v-model="showEdit" max-width="1000px">
          <v-card>
            <v-card-title>
              <span class="headline">Edit {{editUserInfo.username}}</span>
            </v-card-title>
            <v-divider></v-divider>
            <v-card-text class="mt-2">
              <v-container>
                <v-form ref="editform">
                  <v-row>
                    <v-col cols="12" sm="6" md="6">
                      <v-text-field
                        v-model="editUserInfo.phoneNumber"
                        label="Phone Number"
                        required
                      ></v-text-field>
                    </v-col>
                    <v-col cols="12" sm="6" md="6">
                      <v-text-field v-model="editUserInfo.email" label="Email"></v-text-field>
                    </v-col>
                  </v-row>
                  <v-row>
                    <v-col cols="12" sm="6" md="6">
                      <v-text-field v-model="editUserInfo.firstName" label="First Name" required></v-text-field>
                    </v-col>
                    <v-col cols="12" sm="6" md="6">
                      <v-text-field v-model="editUserInfo.lastName" label="Last Name" required></v-text-field>
                    </v-col>
                  </v-row>
                </v-form>
              </v-container>
            </v-card-text>
            <v-card-actions>
              <div class="flex-grow-1"></div>
              <v-btn color="blue darken-1" text @click="cancelEdit">Cancel</v-btn>
              <v-btn color="blue darken-1" text @click="submitEdit">Submit</v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
        <v-dialog v-model="showUser" max-width="1000px">
          <v-card>
            <v-card-title>
              <span class="headline">{{ viewUser.username }}</span>
            </v-card-title>
            <v-divider></v-divider>
            <v-card-text>
              <v-container>
                <v-row>
                  <v-col cols="12" sm="6" md="6">
                    <v-text-field v-model="viewUser.firstName" label="First Name" outlined readonly></v-text-field>
                  </v-col>
                  <v-col cols="12" sm="6" md="6">
                    <v-text-field v-model="viewUser.lastName" label="Last Name" outlined readonly></v-text-field>
                  </v-col>
                </v-row>
                <v-row>
                  <v-col cols="12" sm="6" md="6">
                    <v-text-field
                      v-model="viewUser.phoneNumber"
                      label="Phone Number"
                      outlined
                      readonly
                    ></v-text-field>
                  </v-col>
                  <v-col cols="12" sm="6" md="6">
                    <v-text-field v-model="viewUser.email" label="Email" outlined readonly></v-text-field>
                  </v-col>
                </v-row>
                <v-card-actions>
                  <div class="flex-grow-1"></div>
                </v-card-actions>
              </v-container>
            </v-card-text>
          </v-card>
        </v-dialog>
      </template>
      <template v-slot:body="{ items }">
        <tbody>
          <tr v-for="item in items" :key="item.id" v-on:click="selectUser(item)">
            <template v-if="!isMobile">
              <td>{{item.username}}</td>
              <td>{{item.firstName}}</td>
              <td>{{item.lastName}}</td>
              <td>{{item.email}}</td>
              <td>{{item.phoneNumber}}</td>
              <td>
                <v-layout align-center justify-center>
                  <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="blue darken-1" text @click.stop="viewItem(item)">View</v-btn>
                    <v-btn color="blue darken-1" text @click.stop="editUser(item)">Edit</v-btn>
                    <v-btn color="blue darken-1" text @click.stop="deleteUser(item)">Delete</v-btn>
                  </v-card-actions>
                </v-layout>
              </td>
            </template>
            <template v-else>
              <td>
                <ul class="flex-content" style="list-style-type: none;">
                  <li class="flex-item" data-label="Username">{{ item.username }}</li>
                  <li class="flex-item" data-label="First Name">{{ item.firstName }}</li>
                  <li class="flex-item" data-label="Last Name">{{ item.lastName }}</li>
                  <li class="flex-item" data-label="Email">{{ item.email }}</li>
                  <li class="flex-item" data-label="Phone Number">{{ item.phoneNumber }}</li>
                  <li class="flex-item" data-label="Status">
                    <v-card-actions>
                      <v-btn color="blue darken-1" text @click.stop="viewItem(item)">View</v-btn>
                      <v-btn color="blue darken-1" text @click.stop="editItem(item)">Edit</v-btn>
                      <v-btn color="blue darken-1" text @click.stop="deleteUser(item)">Delete</v-btn>
                    </v-card-actions>
                  </li>
                </ul>
              </td>
            </template>
          </tr>
        </tbody>
      </template>
    </v-data-table>
  </v-content>
</template>

<script>
export default {
  data: () => ({
    headers: [
      {
        text: "Username",
        value: "username",
        width: 150
      },
      { text: "First Name", width: 150, value: "firstName" },
      { text: "Last Name", width: 150, value: "lastName" },
      { text: "Email", width: 150, value: "email" },
      { text: "Phone Number", width: 150, value: "phoneNumber" },
      {
        text: "Actions",
        value: "actions",
        align: "center",
        filterable: false,
        sortable: false
      }
    ],
    users: [],
    selected: [],
    totalUsers: 0,
    pagination: {},
    PageNumber: 1,
    sortingName: "username",
    loading: true,
    isMobile: false,
    showCreate: false,
    showUser: false,
    showEdit: false,
    newUser: {},
    viewUser: {},
    editUserInfo: {}
  }),
  watch: {
    handler() {
      this.fetchUsers();
    },
    deep: true
  },
  created: function() {
    this.fetchUsers();
  },
  computed: {
    options(nv) {
      return {
        ...this.pagination
      };
    }
  },
  methods: {
    onResize() {
      if (window.innerWidth < 769) this.isMobile = true;
      else this.isMobile = false;
    },
    selectUser(user) {
      var userId = user.id;
      if (this.selected.includes(userId)) {
        // Removing the event id
        this.selected.splice(this.selected.indexOf(userId), 1);
      } else {
        this.selected.push(userId);
      }
    },
    fetchUsers() {
      let cur = this;
      this.loading = true;

      const { sortBy, sortDesc, itemsPerPage } = this.pagination;
      console.log(this.pagination);
      this.axios
        .get("https://parkingslotapi.azurewebsites.net/api/users", {
          headers: { Authorization: "Bearer " + cur.$store.getters.TOKEN },
          params: {
            PageNumber: cur.PageNumber,
            //OrderBy: ""
          }
        })
        .then(function(response) {
            console.log(response);
          cur.users = response.data;
          cur.totalUsers = 12;
          setTimeout(() => {
            cur.loading = false;
          }, 500);
        });
    },
    viewItem(item) {
      this.viewUser = Object.assign({}, item);
      this.showUser = true;
    },
    getUserIndex(userId) {
      let userIndex = this.users
        .map(user => {
          return user.id;
        })
        .indexOf(userId);
      return userIndex;
    },
    createUser() {
      this.showCreate = true;
    },
    submitUser() {
      console.log(this.newUser);
      if (this.newUser.Password != this.newUser.ConfirmPassword) {
        console.log("Password do not match");
      } else {
        this.$store
          .dispatch("REGISTER", this.newUser)
          .then(success => {
            this.users.push(this.newUser);
            this.showCreate = false;
            console.log("User have been created");
          })
          .catch(error => {
            console.log(error);
            console.log("Bad request");
          });
      }
    },
    cancelCreate() {
      for (let keys in this.newUser) delete this.newUser[keys];
      this.showCreate = false;
    },
    cancelEdit() {
      this.showEdit = false;
    },
    editUser(item) {
      this.editUserInfo = Object.assign({}, item);
      this.showEdit = true;
    },
    submitEdit() {
      if (this.$refs.editform.validate()) {
        var cur = this;
        this.$store
          .dispatch("UPDATE", {
            FirstName: this.editUserInfo.firstName,
            LastName: this.editUserInfo.lastName,
            Email: this.editUserInfo.email,
            PhoneNumber: this.editUserInfo.contact,
            UserId: this.editUserInfo.id
          })
          .then(success => {
              console.log(success);
            cur.showEdit = false;
            console.log("Profile have been successfully updated!");
          });
      } else {
        console.log("Form not validated");
      }
    },
    deleteUser(item) {
      var result = confirm("Delete " + item.username + "?");
      if (result == true) {
        let cur = this;
        this.axios
          .delete(
            "https://parkingslotapi.azurewebsites.net/api/users/" + item.id,
            {
              headers: { Authorization: "Bearer " + cur.$store.getters.TOKEN }
            }
          )
          .then(function(response) {
            const index = cur.getUserIndex(item.id);
            if (~index) {
              cur.users.splice(index, 1);
            }
            console.log("User have been deleted!");
          });
      }
    }
  }
};
</script>
<style>
@import url("../assets/table.css");
</style>