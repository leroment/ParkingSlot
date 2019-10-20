<template>
<v-content>
  <v-card>
    <v-list two-line>
      <v-list-item-group>
        <template v-for="(item, index) in items">
          <v-list-item :key="item.carparkName" @click="displayCarparkInfo(item)">
            <template>
              <v-list-item-content>
                <v-list-item-title v-text="item.carparkName"></v-list-item-title>
                <v-list-item-subtitle v-text="item.carparkLocation"></v-list-item-subtitle>
              </v-list-item-content>
              <v-list-item-action>
                <v-icon @click="unfavourite(item.id)">mdi-delete</v-icon>
              </v-list-item-action>
            </template>
          </v-list-item>
          <v-divider v-if="index + 1 < items.length" :key="index"></v-divider>
        </template>
      </v-list-item-group>
    </v-list>
    <v-dialog v-model="dialog" width="500">
      <v-card> 
        <v-card-title
          class="headline grey lighten-2"
          primary-title
        >Carpark Information</v-card-title> 
        
        <v-card-text>
        <br/>
        {{"Carpark Type: " + viewingItem.carparkType}} 
        <br/>
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
    this.fetchData();
  },
  methods: {
    fetchData: function() {
      //Get request from backend first

      //Append data to array
      this.items.push({
        id: "739",
        carparkName: "Harbourfront Centre1",
        carparkLocation: "1 Martime Square",
        carparkType: "Outdoor",
        parkingFee: "$1/h"
      });
      this.items.push({
        id: "749",
        carparkName: "Harbourfront Centre100",
        carparkLocation: "10 Martime Square",
        carparkType: "Multi-Storey",
        parkingFee: "$1.50/h"
      });
    },
    unfavourite: function(id) {
      //Delete Request
      if (confirm("Are you sure?")) {
        //Send Delete Request to Backend: If succesful, then delete from frontend
        this.items.forEach((item, index) => {
          if (item.id == id) {
            this.items.splice(index, 1);
          }
        });
      }
    },
    displayCarparkInfo: function(item) {
      this.viewingItem = item;
      this.dialog = true;
    }
  }
};
</script>
