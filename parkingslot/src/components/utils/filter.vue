<template>
  <v-flex xs12>
    <v-card-text>
      <v-menu offset-y>
        <template v-slot:activator="{ on }">
          <v-layout align-end justify-end>
            <v-btn class="ma-2" v-on="on" @click="openFilter" pull-right outlined fab color="teal">
              <v-icon v-if="isFiltered">mdi-filter</v-icon>
              <v-icon v-if="!isFiltered">mdi-filter-outline</v-icon>
            </v-btn>
          </v-layout>
        </template>
        <v-card max-width="500" outlined>
          <v-list-item>
            <v-list-item-content>
              <div class="filterheader mb-4">Filter carparks by:</div>
              <v-col cols="12" sm="6" md="6">
                <v-switch v-model="filterConfig.IsAscending" label="Ascending Order"></v-switch>
              </v-col>
              <v-col cols="12" sm="6" md="6">
                <v-switch v-model="filterConfig.IsMinPrice" label="Sort Min Price"></v-switch>
              </v-col>
              <v-col cols="12" sm="6" md="6">
                <v-select :items="vehType" v-model="filterConfig.VehType" label="Vehicle Type" solo></v-select>
              </v-col>
              <v-col cols="12" sm="6" md="6">
                <v-select
                  :items="agencyType"
                  v-model="filterConfig.AgencyType"
                  label="Agency Type"
                  solo
                ></v-select>
              </v-col>
              <v-col cols="12" sm="6" md="6">
                <v-menu v-model="startDateMenu" :close-on-content-click="false" max-width="290">
                  <template v-slot:activator="{ on }">
                    <v-text-field
                      :value="filterConfig.startdate"
                      clearable
                      label="Start Date"
                      readonly
                      v-on="on"
                      required
                    ></v-text-field>
                  </template>
                  <v-date-picker
                    v-model="filterConfig.startdate"
                    scrollable
                    @change="startDateMenu = false"
                  ></v-date-picker>
                </v-menu>
              </v-col>
              <v-col cols="12" sm="6" md="6">
                <v-menu
                  ref="startTime"
                  v-model="startTimeMenu"
                  :close-on-content-click="false"
                  :nudge-right="40"
                  :return-value.sync="starttime"
                  transition="scale-transition"
                  offset-y
                  max-width="290px"
                  min-width="290px"
                >
                  <template v-slot:activator="{ on }">
                    <v-text-field
                      v-model="filterConfig.starttime"
                      label="Start Time"
                      readonly
                      v-on="on"
                      required
                    ></v-text-field>
                  </template>
                  <v-time-picker
                    v-if="startTimeMenu"
                    v-model="filterConfig.starttime"
                    full-width
                    scrollable
                    @click:minute="$refs.startTime.save(starttime)"
                  ></v-time-picker>
                </v-menu>
              </v-col>
              <!-- stop here -->
              <v-col cols="12" sm="6" md="6">
                <v-menu v-model="endDateMenu" :close-on-content-click="false" max-width="290">
                  <template v-slot:activator="{ on }">
                    <v-text-field
                      :value="filterConfig.enddate"
                      clearable
                      label="End Date"
                      readonly
                      v-on="on"
                      required
                    ></v-text-field>
                  </template>
                  <v-date-picker
                    v-model="filterConfig.enddate"
                    scrollable
                    @change="endDateMenu = false"
                  ></v-date-picker>
                </v-menu>
              </v-col>
              <v-col cols="12" sm="6" md="6">
                <v-menu
                  ref="endTime"
                  v-model="endTimeMenu"
                  :close-on-content-click="false"
                  :nudge-right="40"
                  :return-value.sync="endtime"
                  transition="scale-transition"
                  offset-y
                  max-width="290px"
                  min-width="290px"
                >
                  <template v-slot:activator="{ on }">
                    <v-text-field
                      v-model="filterConfig.endtime"
                      label="End Time"
                      readonly
                      v-on="on"
                      required
                    ></v-text-field>
                  </template>
                  <v-time-picker
                    v-if="endTimeMenu"
                    v-model="filterConfig.endtime"
                    full-width
                    scrollable
                    @click:minute="$refs.endTime.save(endtime)"
                  ></v-time-picker>
                </v-menu>
              </v-col>
              <v-col col="12" sm="12" md="12">
                <v-text-field v-model="filterConfig.price" label="Price"></v-text-field>
              </v-col>
            </v-list-item-content>
          </v-list-item>
          <v-card-actions>
            <div class="flex-grow-1"></div>
            <v-btn color="blue darken-1" text @click="filterCarparks">Filter</v-btn>
            <v-btn color="blue darken-1" text @click="clearFilter">Clear Filter</v-btn>
          </v-card-actions>
        </v-card>
      </v-menu>
    </v-card-text>
  </v-flex>
</template>
<script>
import { filter } from "minimatch";
export default {
  data() {
    return {
      isFiltered: false,
      filterOpen: false,
      filterConfig: this.$store.getters.FILTER,
      vehType: ["Car", "Motorcycle", "Heavy Vehicle", "All"],
      agencyType: ["HDB", "URA", "LTA", "All"],
      startDateMenu: false,
      startTimeMenu: false,
      endDateMenu: false,
      endTimeMenu: false,
      starttime: null,
      endtime: null
    };
  },
  methods: {
    openFilter: function() {
      //Get the updated filterConfig settings
      this.filterConfig = this.$store.getters.FILTER;
    },
    filterCarparks: function() {
      //filter carparks here.
      this.$emit("clicked", this.filterConfig);
    },
    clearFilter: function(event) {
      event.stopPropagation();
      this.filterConfig = {
        //Set back default values
        IsAscending: false,
        IsMinPrice: false,
        PageSize: 20,
        PageNumber: 1
      };
    }
  }
};
</script>
<style>
.filterbox {
  margin-top: -15px !important;
  padding: 15px !important;
}

.filterheader {
  color: black;
  font-weight: 300;
  font-size: 18px;
}
</style>