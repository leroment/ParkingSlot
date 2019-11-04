<template>
  <v-card>
    <CarparkFilter @change="onText" @clicked="onFilter"></CarparkFilter>
    <v-list>
      <v-list-item-group active-class="blue--text">
        <template v-for="item in carparkItem">
          <v-list-item :key="item.carparkId" @click.stop="displayCarparkInfo(item)">
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
                  @click.stop="favorite(item)"
                  color="grey lighten-1"
                >mdi-heart-outline</v-icon>
                <v-icon v-else @click.stop="unfavorite(item)" color="red">mdi-heart</v-icon>
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
    <v-dialog v-model="dialog" width="500">
      <v-card>
        <v-card-title
          class="headline blue font-weight-light darken-2"
          style="color:white"
          primary-title
        >Carpark Information</v-card-title>
        <v-card-text>
          <v-container>
            <v-row>
              <span class="body-1">Carpark ID: {{ viewingItem.carparkId }}</span>
            </v-row>
            <v-row>
              <span class="body-1">Carpark Name: {{ viewingItem.carparkName }}</span>
            </v-row>
            <v-row>
              <span class="body-1">Agency Type: {{ viewingItem.agencyType }}</span>
            </v-row>
            <v-row>
              <span class="body-1">Address: {{ viewingItem.address }}</span>
            </v-row>
            <v-row v-if="viewingItem.totalAvailableLots != '-1'">
              <span class="body-1">Total Available Lots: {{ viewingItem.totalAvailableLots }}</span>
            </v-row>
            <v-row v-if="viewingItem.totalLots != '-1'">
              <span class="body-1">Total Lots: {{ viewingItem.totalLots }}</span>
            </v-row>
            <v-row v-if="viewingItem.carAvailability != '-1'">
              <span class="body-1">Car Availability: {{ viewingItem.carAvailability }}</span>
            </v-row>
            <v-row v-if="viewingItem.mAvailability != '-1'">
              <span class="body-1">Motorcycle Availability: {{ viewingItem.mAvailability }}</span>
            </v-row>
            <v-row v-if="viewingItem.hvAvailability != '-1'">
              <span class="body-1">Heavy Vehicle Availability: {{ viewingItem.hvAvailability }}</span>
            </v-row>
            <v-row v-if="viewingItem.carCapacity != '-1'">
              <span class="body-1">Car Capacity: {{ viewingItem.carCapacity }}</span>
            </v-row>
            <v-row v-if="viewingItem.mCapacity != '-1'">
              <span class="body-1">Motorcycle Capacity: {{ viewingItem.mCapacity }}</span>
            </v-row>
            <v-row v-if="viewingItem.hvCapacity != '-1'">
              <span class="body-1">Heavy Vehicle Capacity: {{ viewingItem.hvCapacity }}</span>
            </v-row>
            <br />
            <v-row v-if="viewingItem.carRates != 0">
              <span style="text-decoration: underline !important;" class="body-1">Car Rates:</span>
            </v-row>
            <v-row v-for="rate in viewingItem.carRates" :key="rate.id">
              <span class="body-1">Time Range: {{rate.startTime}} - {{rate.endTime}}</span>
              <span class="body-1">Weekday Rate: {{rate.weekdayRate}} per 30 min</span>
              <span class="body-1">Saturday Rate: {{rate.satdayRate}} per 30 min</span>
              <span class="body-1 mb-5">Sunday/Public Holiday Rate: {{rate.sunPHRate}} per 30 min</span>
            </v-row>
            <v-row v-if="viewingItem.motorRates != 0">
              <span style="text-decoration: underline !important;" class="body-1">Motorcycle Rates:</span>
            </v-row>
            <v-row class="row" v-for="rate in viewingItem.motorRates" :key="rate.id">
              <span class="body-1">Time Range: {{rate.startTime}} - {{rate.endTime}}</span>
              <span class="body-1">Weekday Rate: {{rate.weekdayRate}} per 30 min</span>
              <span class="body-1">Saturday Rate: {{rate.satdayRate}} per 30 min</span>
              <span class="body-1 mb-5">Sunday/Public Holiday Rate: {{rate.sunPHRate}} per 30 min</span>
            </v-row>
            <v-row v-if="viewingItem.hvRates != 0">
              <span
                style="text-decoration: underline !important;"
                class="body-1"
              >Heavy Vehicle Rates:</span>
            </v-row>
            <v-row class="row" v-for="rate in viewingItem.hvRates" :key="rate.id">
              <span class="body-1">Time Range: {{rate.startTime}} - {{rate.endTime}}</span>
              <span class="body-1">Weekday Rate: {{rate.weekdayRate}} per 30 min</span>
              <span class="body-1">Saturday Rate: {{rate.satdayRate}} per 30 min</span>
              <span class="body-1 mb-5">Sunday/Public Holiday Rate: {{rate.sunPHRate}} per 30 min</span>
            </v-row>
          </v-container>
        </v-card-text>
        <v-card-actions>
          <div class="flex-grow-1"></div>
          <v-btn color="primary" text @click="dialog = false">Go Back</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
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
      dialog: false,
      carparkItem: [],
      viewingItem: {},
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
          if (response.data.length > 0) {
            cur.carparkItem = Object.assign({}, response.data);
          } else {
          }
        });
    },
    onFilter(filterConfig) {
      //Pass the updated filter config
      this.filterConfig = filterConfig;
      this.changeType();
    },
    infiniteHandler($state) {
      var ascending = "carparkname";
      if (this.filterConfig.IsAscending == false) {
        ascending = "carparkname desc";
      }
      this.axios
        .get("https://parkingslotapi.azurewebsites.net/api/carpark", {
          params: {
            Orderby: ascending,
            IsElectronic: this.filterConfig.IsElectronic,
            IsCentral: this.filterConfig.IsCentral,
            PageNumber: this.filterConfig.PageNumber,
            VehType: this.filterConfig.VehType,
            AgencyType: this.filterConfig.AgencyType,
            Radius: this.filterConfig.Radius
          }
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
    },
    displayCarparkInfo: function(item) {
      let cur = this;
      this.axios
        .get(
          `https://parkingslotapi.azurewebsites.net/api/CarparkRates/${item.id}`
        )
        .then(function(response) {
          var MotorcycleRates = [];
          var HVRates = [];
          var CarRates = [];
          if (response.status == "200") {
            response.data.forEach(rate => {
              if (rate.vehicleType == "Motorcycle") {
                MotorcycleRates.push({
                  startTime: rate.startTime,
                  endTime: rate.endTime,
                  weekdayRate: rate.weekdayRate,
                  satdayRate: rate.satdayRate,
                  sunPHRate: rate.sunPHRate
                });
              }
              if (rate.vehicleType == "Heavy Vehicle") {
                HVRates.push({
                  startTime: rate.startTime,
                  endTime: rate.endTime,
                  weekdayRate: rate.weekdayRate,
                  satdayRate: rate.satdayRate,
                  sunPHRate: rate.sunPHRate
                });
              }
              if (rate.vehicleType == "Car") {
                CarRates.push({
                  startTime: rate.startTime,
                  endTime: rate.endTime,
                  weekdayRate: rate.weekdayRate,
                  satdayRate: rate.satdayRate,
                  sunPHRate: rate.sunPHRate
                });
              }
            });

            if (CarRates == []) CarRates = 0; //So that can clear in v-if
            if (MotorcycleRates == []) MotorcycleRates = 0; //So can clear in v-if
            if (HVRates == []) HVRates = 0;

            item.motorRates = MotorcycleRates;
            item.carRates = CarRates;
            item.hvRates = HVRates;
          }

          cur.viewingItem = item;
          cur.dialog = true;
        });
    }
  }
};
</script>