<template>
  <v-content>
    <v-card>
      <v-list two-line>
        <v-list-item-group multiple active-class="blue--text">
          <template v-for="item in carparkItem">
            <v-list-item :key="item.carparkName">
              <template>
                <v-list-item-content>
                  <v-list-item-title v-text="item.carparkName"></v-list-item-title>
                  <v-list-item-subtitle v-text="item.address"></v-list-item-subtitle>
                </v-list-item-content>
                <v-list-item-action>
                  <v-list-item-action-text v-text="item.totalAvailableLots"></v-list-item-action-text>
                  <v-icon
                    v-if="item.favorite"
                    @click="favorite(item.id)"
                    color="grey lighten-1"
                  >mdi-heart-outline</v-icon>
                  <v-icon v-else @click="unfavorite(item.id)" color="red">mdi-heart</v-icon>
                </v-list-item-action>
              </template>
            </v-list-item>
          </template>
        </v-list-item-group>
      </v-list>
    </v-card>
  </v-content>
</template>


<script>
export default {
  data() {
    return {
      selected: [2],
      carparkItem: [],
      userFavorites: [],
      filterConfig: {
        IsAscending: true,
        PageSize: 20,
        PageNumber: 1,
        VehType: "All",
        AgencyType: "All",
        Price: "",
        StartDateTime: "",
        EndDateTime: "",
        IsMinPrice: true
      }
    };
  },
  mounted: function() {
    this.getUserFavorites();
    this.getCarparkList();
  },
  methods: {
    getCarparkList: function() {
      let cur = this;
      this.axios
        .get("https://parkingslotapi.azurewebsites.net/api/carpark", {
          params: cur.filterConfig
        })
        .then(function(response) {
          cur.carparkItem = response.data;
        });
    },
    getUserFavorites: function() {
      let cur = this;
      this.axios
        .get(
          "https://parkingslotapi.azurewebsites.net/api/users/" +
            this.$store.getters.USERID +
            "/favorites"
        )
        .then(function(response) {
          for (var i = 0; i < response.data.length; i++) {
            //push each carpark id into the array
            cur.userFavorites.push(response.data[i].carparkId);
          }
        });
    },
    addUserFavoritesToList: function() {
      for (var i = 0; i < this.carparkItem.length; i++) {
        //if favorite list contains the same id as the list fetch, set the list favorite key to true
        if (this.userFavorites.includes(this.carparkItem[i].carparkId)) {
          this.carparkItem.favorite = true;
        } else {
          this.carparkItem.favorite = false;
        }
      }
    },
    favorite: function(carparkId) {
      //Update favorite on the backend
      //If status code is 200,favorite/unfavorite by adding/removing the carpark id from the userFavorites array
      //else show error msg
      //update the value on
      console.log("liked");
    },
    unfavorite: function(carparkId) {
      console.log("unliked");
    }
  }
};
</script>