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
                    v-if="!item.favorite"
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
      carparkItem: [],
      filterConfig: {
        IsAscending: true,
        PageSize: 20,
        PageNumber: 1,
        VehType: "",
        AgencyType: "",
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
          cur.addUserFavoritesToList;
        });
    },
    getUserFavorites: function() {
      //fetch from store
    },
    addUserFavoritesToList: function() {
      //update the list favorites
    },
    favorite: function(carparkId) {
      //fetch from store
      console.log("liked");
    },
    unfavorite: function(carparkId) {
      //fetch from store
      console.log("unliked");
    }
  }
};
</script>