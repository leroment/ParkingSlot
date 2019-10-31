<template>
  <v-card>
    <CarparkFilter @change="onText" @clicked="onFilter"></CarparkFilter>
    <v-list>
      <v-list-item-group active-class="blue--text">
        <template v-for="item in carparkItem">
          <v-list-item :key="item.id">
            <template>
              <v-list-item-content>
                <v-list-item-title v-text="item.carparkName"></v-list-item-title>
                <v-list-item-subtitle v-text="item.address"></v-list-item-subtitle>
              </v-list-item-content>
              <v-list-item-action>
                <v-list-item-action-text
                  v-if="item.totalAvailableLots != '-1'"
                  v-text="item.totalAvailableLots"
                ></v-list-item-action-text>
                <v-icon
                  v-if="item.favorite == false || item.favorite == undefined"
                  @click="favorite(item)"
                  color="grey lighten-1"
                >mdi-heart-outline</v-icon>
                <v-icon v-else @click="unfavorite(item)" color="red">mdi-heart</v-icon>
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
  },
  methods: {
    onText(input) {
      var value = {
        SearchQuery: input
      };
      let cur = this;
      this.axios
        .get("https://parkingslotapi.azurewebsites.net/api/carpark", {
          params: value
        })
        .then(function(response) {
          cur.carparkItem = Object.assign({}, response.data);
        });
    },
    onFilter(filterConfig) {
      //Pass the updated filter config
      this.filterConfig = filterConfig;
      this.changeType();
    },
    infiniteHandler($state) {
      this.axios
        .get("https://parkingslotapi.azurewebsites.net/api/carpark", {
          params: this.filterConfig
        })
        .then(({ data }) => {
          setTimeout(() => {
            if (data.length) {
              this.filterConfig.PageNumber += 1;
              this.carparkItem.push(...data);
              //Check for user favorites for every page
              this.getUserFavorites();
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
      this.$store.dispatch("GETFAVORITES").then(userFavorites => {
        for (var j = 0; j < userFavorites.data.length; j++) {
          var id = userFavorites.data[j].carparkId;
          var index = this.getCarparkPos(id);
          if (index != -1) {
            var item = this.carparkItem[index];
            item.favorite = true;
            this.$set(this.carparkItem, index, item);
          }
        }
      });
    },
    getCarparkPos(id) {
      var foundIndex = this.carparkItem
        .map(function(x) {
          return x.id;
        })
        .indexOf(id);
      return foundIndex;
    },
    favorite: function(item) {
      this.$store.dispatch("ADDFAVORITES", item.id).then(success => {
        if (success) {
          var index = this.getCarparkPos(item.id);
          item.favorite = true;
          this.$set(this.carparkItem, index, item);
        }
      });
    },
    unfavorite: function(item) {
      this.$store.dispatch("DELETEFAVORITE", item.id).then(success => {
        if (success) {
          var index = this.getCarparkPos(item.id);
          item.favorite = false;
          this.$set(this.carparkItem, index, item);
        }
      });
    }
  }
};
</script>