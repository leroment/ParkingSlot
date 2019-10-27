<template>
  <v-content>
    <CarparkFilter @clicked="onFilter"></CarparkFilter>
    <v-card>
      <v-list two-line>
        <v-list-item-group active-class="blue--text">
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
          <infinite-loading
            spinner="spiral"
            ref="InfiniteLoading"
            :identifier="infiniteId"
            @infinite="infiniteHandler"
          >
            <div slot="no-more">-- End of Carpark List --</div>
            <div slot="no-results">-- No Carpark --</div>
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
import CarparkFilter from "./utils/filter";
export default {
  components: {
    CarparkFilter
  },
  data() {
    return {
      carparkItem: [],
      infiniteId: +new Date(),
      filterConfig: this.$store.getters.FILTER
    };
  },
  mounted: function() {
    //reset page number back to 1
    this.filterConfig.PageNumber = 1;
    this.getUserFavorites();
  },
  methods: {
    onFilter(filterConfig) {
      //Pass the updated filter config
      this.filterConfig = filterConfig;
      this.changeType();
    },
    infiniteHandler($state) {
      let cur = this;
      this.axios
        .get("https://parkingslotapi.azurewebsites.net/api/carpark", {
          params: this.filterConfig
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
    changeType() {
      this.filterConfig.PageNumber = 1;
      this.carparkItem = [];
      this.infiniteId += 1;
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