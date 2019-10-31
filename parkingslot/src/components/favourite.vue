<template>
  <v-content>
    <v-card>
      <v-list two-line>
        <v-list-item-group>
          <template v-for="(item, index) in items">
            <v-list-item :key="item.carparkName" @click.stop="displayCarparkInfo(item)">
              <template>
                <v-list-item-content>
                  <v-list-item-title v-text="item.carparkName"></v-list-item-title>
                  <v-list-item-subtitle v-text="item.carparkLocation"></v-list-item-subtitle>
                </v-list-item-content>
                <v-list-item-action>
                  <v-icon @click.stop="unfavourite(item.id)">mdi-delete</v-icon>
                </v-list-item-action>
              </template>
            </v-list-item>
            <v-divider v-if="index + 1 < items.length" :key="index"></v-divider>
          </template>
        </v-list-item-group>
      </v-list>
      <v-dialog v-model="dialog" width="500">
        <v-card>
          <v-card-title class="headline grey lighten-2" primary-title>Carpark Information</v-card-title>

          <v-card-text>
            <br />
            {{"Carpark ID: " + viewingItem.carparkId}}
            <br />
            {{"Carpark Name: " + viewingItem.carparkName}}
            <br />
            {{"Agency Type: " + viewingItem.agencyType}}
            <br />
            {{"Carpark Location: " + viewingItem.carparkLocation}}
            <br />
            {{"Total Available Lots: " + viewingItem.totalAvailableLots}}
            <br />
            {{"Total Available Car Lots: " + viewingItem.carAvailability}}
            <br />
            {{"Total Available Motorcycle Lots: " + viewingItem.mAvailability}}
            <br />
            {{"Total Available Heavy Vehicle Lots: " + viewingItem.hvAvailability}}
            <br />
            {{"Total Lots: " + viewingItem.totalLots}}
            <br />
            {{"Total Car Lots: " + viewingItem.carCapacity}}
            <br />
            {{"Total Motorcycle Lots: " + viewingItem.mCapacity}}
            <br />
            {{"Total Heavy Vehicle Lots: " + viewingItem.hvCapacity}}
            <br />
            {{"Parking Rate: " +viewingItem.parkingFee}}
          </v-card-text>

          <v-card-actions>
            <div class="flex-grow-1"></div>
            <v-btn color="primary" text @click="dialog = false">Go Back</v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
    </v-card>
  </v-content>
</template>
<script>
export default {
  data() {
    return {
      dialog: false,
      selected: [],
      items: [],
      viewingItem: {}
    };
  },
  created: function() {
    this.fetchFavoritesData();
  },
  methods: {
    fetchFavoritesData: function() {
      //Retrieve favorites data (CarparkID)
      //Get request from backend first
      this.$store
        .dispatch("GETFAVORITES")
        .then(success => {
          var idArr = this.$store.getters.FAVORITES.slice();
          idArr.forEach(carparkID => {
            this.fetchCarparkInfo(carparkID);
          });
        })
        .catch(error => {
          console.log(error);
        });
    },
    unfavourite: function(carparkID) {
      var cur = this;
      //Delete Request
      if (confirm("Are you sure?")) {
        //Send Delete Request to Backend: If succesful, then delete from frontend
        this.$store
          .dispatch("DELETEFAVORITE", carparkID)
          .then(success => {
            cur.items.forEach((item, index) => {
              if (item.id == carparkID) {
                cur.items.splice(index, 1);
              }
            });
          })
          .catch(error => {
            console.log(error);
          });
      }
    },
    fetchCarparkInfo: function(carparkID) {
      //Retrieve Carpark information from GetCarpark API call using CarparkID
      let cur = this;
      this.axios
        .get(
          `https://parkingslotapi.azurewebsites.net/api/carpark/${carparkID}`
        )
        .then(function(response) {
          if (response.status == "200") {
            cur.items.push({
              id: response.data.id,
              carparkId: response.data.carparkId,
              carparkName: response.data.carparkName,
              lotType: response.data.lotType,
              agencyType: response.data.agencyType,
              carparkLocation: response.data.address,
              totalAvailableLots:
                response.data.totalAvailableLots != -1
                  ? response.data.totalAvailableLots
                  : "Not Available",
              totalLots:
                response.data.totalLots != -1
                  ? response.data.totalLots
                  : "Not Available",
              carAvailability:
                response.data.carAvailability != -1
                  ? response.data.carAvailability
                  : "Not Available",
              mAvailability:
                response.data.mAvailability != -1
                  ? response.data.mAvailability
                  : "Not Available",
              hvAvailability:
                response.data.hvAvailability != -1
                  ? response.data.hvAvailability
                  : "Not Available",
              carCapacity:
                response.data.carCapacity != -1
                  ? response.data.carCapacity
                  : "Not Available",
              mCapacity:
                response.data.mCapacity != -1
                  ? response.data.mCapacity
                  : "Not Available",
              hvCapacity:
                response.data.hvCapacity != -1
                  ? response.data.hvCapacity
                  : "Not Available",
              parkingFee: "$1/h"
            });
          }
        });
    },
    displayCarparkInfo: function(item) {
      this.viewingItem = item;
      this.dialog = true;
    }
  }
};
</script>
