<template>
  <v-card>
    <CarparkFilter @change="onText" @clicked="onFilter"></CarparkFilter>
    <v-list>
      <v-list-item-group active-class="blue--text">
        <template v-for="item in carparkItem">
          <v-list-item :key="item.carparkName">
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
    onText(test) {
      console.log(test);
      var value = {
        SearchQuery: test
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
    getCarparkItemIndex(id) {
      var foundIndex = this.carparkItem
        .map(function(x) {
          return x.id;
        })
        .indexOf(id);
      return foundIndex;
    },
    getUserFavorites: function() {
      //fetch from store
      var cur = this;
      cur.$store.dispatch("GETFAVORITES").then(userFavorites => {
        for (var i = 0; i < cur.carparkItem.length; i++) {
          for (var j = 0; j < userFavorites.data.length; j++) {
            if (
              JSON.stringify(cur.carparkItem[i].id) ===
              JSON.stringify(userFavorites.data[j].carparkId)
            ) {
              //set data to true
              //cur.carparkItem[i].favorite = true;
              //cur.carparkId.favorite.$set(i, true);
            }
          }
        }
      });
    },
    favorite: function(item) {
      //console.log(item.id);
      this.$store.dispatch("ADDFAVORITES", item.id).then(success => {
        if (success) {
          var index = this.getCarparkItemIndex(item.id);
          this.$set(this.carparkItem[index], favorite, true);
          //this.carparkItem[index].favorite = true;
        }
      });
    },
    unfavorite: function(item) {
      //fetch from store
      //console.log("unliked");
    }
  }
};
</script>