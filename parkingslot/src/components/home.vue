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
          <infinite-loading spinner="spiral" :identifier="infiniteId" @infinite="infiniteHandler">
            <div slot="no-more">-- End of Event List --</div>
            <div slot="no-results">-- No Events --</div>
          </infinite-loading>
          <a id="back-to-top" href="#" class="btn btn-primary btn-lg back-to-top" role="button">
            <span class="fas fa-arrow-up"></span>
          </a>
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
      },
      infiniteId: +new Date()
    };
  },
  mounted: function() {
    this.getUserFavorites();
  },
  methods: {
    infiniteHandler($state) {
      let cur = this;
      this.axios
        .get("https://parkingslotapi.azurewebsites.net/api/carpark", {
          params: cur.filterConfig
        })
        .then(({ data }) => {
          setTimeout(() => {
            if (data.length) {
              this.filterConfig.PageNumber += 1;
              this.carparkItem.push(...data);
              $state.loaded();
            } else {
              $state.complete();
            }
          }, 500);
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