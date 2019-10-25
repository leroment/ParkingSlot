<template>
  <v-content style="margin-bottom:55px;">
    <v-layout>
      <v-btn class="ma-2 gmcontrol1">
        <v-icon size="28" @click="geolocation" color="#333333">mdi-crosshairs-gps</v-icon>
      </v-btn>
    </v-layout>
    <gmap-map :center="center" ref="mapRef" :zoom="14" :options="mapStyle">
      <gmap-marker :position="center" :icon="userMarkerOptions" @click="currentinfo = !currentinfo">
        <gmap-info-window :position="center" @closeclick="currentinfo=false" :opened="currentinfo">
          <h2>Current location</h2>
          <v-container class>
            <v-row>
              <span class="body-1">Select a carpark to get more information and directions</span>
            </v-row>
          </v-container>
        </gmap-info-window>
      </gmap-marker>
      <gmap-cluster>
        <gmap-marker
          :key="index"
          v-for="(m, index) in markers"
          :position="m.position"
          :clickable="true"
          :draggable="false"
          @click="showMarkerInfo(m)"
          :icon="markerOptions"
        ></gmap-marker>
        <gmap-info-window
          @closeclick="window_open=false"
          :opened="window_open"
          :position="infowindow"
          :options="{
              pixelOffset: {
                width: 0,
                height: -35
              }
            }"
        >
          <h2>
            {{ markerItem.carparkName }}
            <v-chip
              v-if="markerItem.agencyType != '-1'"
              class="ma-2"
              color="primary"
              outlined
            >{{ markerItem.agencyType }}</v-chip>
          </h2>
          <v-container class>
            <v-row v-if="markerItem.address != '-1'">
              <span class="body-1">Address: {{ markerItem.address }}</span>
            </v-row>
            <v-row v-if="markerItem.totalAvailableLots != '-1'">
              <span class="body-1">Total Available Lots: {{ markerItem.totalAvailableLots }}</span>
            </v-row>
            <v-row v-if="markerItem.totalLots != '-1'">
              <span class="body-1">Total Lots: {{ markerItem.totalLots }}</span>
            </v-row>
            <v-row v-if="markerItem.carAvailability != '-1'">
              <span class="body-1">Car Availability: {{ markerItem.carAvailability }}</span>
            </v-row>
            <v-row v-if="markerItem.mAvailability != '-1'">
              <span class="body-1">Motorcycle Availability: {{ markerItem.mAvailability }}</span>
            </v-row>
            <v-row v-if="markerItem.hvAvailability != '-1'">
              <span class="body-1">Heavy Vehicle Availability: {{ markerItem.hvAvailability }}</span>
            </v-row>
            <v-row v-if="markerItem.carCapacity != '-1'">
              <span class="body-1">Car Capacity: {{ markerItem.carCapacity }}</span>
            </v-row>
            <v-row v-if="markerItem.mCapacity != '-1'">
              <span class="body-1">Motorcycle Capacity: {{ markerItem.mCapacity }}</span>
            </v-row>
            <v-row v-if="markerItem.hvCapacity != '-1'">
              <span class="body-1">Heavy Vehicle Capacity: {{ markerItem.hvCapacity }}</span>
            </v-row>
          </v-container>
          <v-btn
            class="mt-5"
            color="info"
            @click="getDirection(carparkItem)"
            type="submit"
          >Directions</v-btn>
        </gmap-info-window>
      </gmap-cluster>
    </gmap-map>
          <table class="searchbox">
        <tr>
          <td>
            <input type="text"  class="textbox centering" placeholder="ここで検索" />
          </td>
          <td>
            <button class="button centering" >検索</button>
          </td>
        </tr>
      </table>
  </v-content>
</template>

<script>
/*global google*/
import mapStyles from "@/assets/mapStyle";
const mapMarker = require("../assets/mapmarker.png");
const carMarker = require("../assets/usermarker.png");
export default {
  props: {
    allmarkers: Array
  },
  data() {
    return {
      center: {
        lat: +0,
        lng: +0,
      },
      markers: [],
      markerItem: {},
      carparkItem: {},
      mapStyle: {
        styles: mapStyles,
        zoomControl: true,
        mapTypeControl: false,
        scaleControl: true,
        streetViewControl: false,
        rotateControl: true,
        fullscreenControl: true,
        disableDefaultUi: false
      },
      markerOptions: {
        url: mapMarker,
        size: { width: 30, height: 30 },
        scaledSize: { width: 30, height: 30 }
      },
      userMarkerOptions: {
        url: carMarker,
        size: { width: 40, height: 40 },
        scaledSize: { width: 40, height: 40 }
      },
      info_marker: null,
      infowindow: { lat: 10.0, lng: 10.0 },
      window_open: false,
      currentinfo: true
    };
  },
  created: function() {
    this.geolocation();
    this.fetchCaparks();
    //"ReferenceError: google is not defined"
    //https://github.com/xkjyeah/vue-google-maps/wiki/vue-google-maps-FAQ
    this.$gmapApiPromiseLazy().then(() => {
      this.mapStyle.zoomControlOptions = {
        style: google.maps.ZoomControlStyle.SMALL,
        position: google.maps.ControlPosition.TOP_RIGHT
      };
    });
  },
  mounted: function() {
    /* Do not remove */
    /* Need to add mapref on gmaps */
    /* https://github.com/xkjyeah/vue-google-maps/issues/403 */
    this.$refs.mapRef.$mapPromise.then(map => {
      this.map = map;
    });
  },
  methods: {
    geolocation: function() {
      navigator.geolocation.getCurrentPosition(position => {
        this.center = {
          lat: parseFloat(position.coords.latitude),
          lng: parseFloat(position.coords.longitude)
        };
        this.$refs.mapRef.panTo(this.center);
      });
    },
    fetchCaparks: function() {
      let cur = this;
      this.axios
        .get("https://localhost:44392/api/carpark/all")
        .then(function(response) {
          for (var i = 0; i < response.data.length; i++) {
            var userloc = {
              position: {
                id: response.data[i].id,
                carparkId: response.data[i].carparkId,
                lat: parseFloat(response.data[i].xCoord),
                lng: parseFloat(response.data[i].yCoord)
              }
            };
            cur.markers.push(userloc);
          }
        });
    },
    showMarkerInfo: function(markerInfo) {
      //Display markerinfo except for current position
      //Change content if the user click on center
      this.carparkItem = {};
      this.infowindow.lat = parseFloat(markerInfo.position.lat);
      this.infowindow.lng = parseFloat(markerInfo.position.lng);
      //Move to center
      this.$refs.mapRef.panTo(this.infowindow);
      let cur = this;
      this.carparkItem = markerInfo.position;
      //fetch individual carpark position
      this.axios
        .get(
          "https://parkingslotapi.azurewebsites.net/api/carpark/" +
            markerInfo.position.id
        )
        .then(function(response) {
          //Pass in all the values
          cur.markerItem = response.data;
          delete markerInfo.xCoord;
          delete markerInfo.yCoord;
        });
      this.window_open = true;
    },
    getDirection: function(markerInfo) {
      //Close the infowindow
      this.window_open = false;
      var destination = new Object();
      destination.lat = parseFloat(markerInfo.lat);
      destination.lng = parseFloat(markerInfo.lng);

      var directionsService = new google.maps.DirectionsService();
      //To change the polyline color
      //polylineOptions:{strokeColor:"#4a4a4a",strokeWeight:5},
      var directionsDisplay = new google.maps.DirectionsRenderer({
        suppressMarkers: true,
        polylineOptions: { strokeColor: "green", strokeWeight: 5 }
      });
      //Remove previous routing
      directionsDisplay.setMap(this.$refs.mapRef.$mapObject);
      directionsService.route(
        {
          origin: this.center,
          destination: destination,
          travelMode: "DRIVING"
          /* More configurations
          transitOptions: TransitOptions,
          drivingOptions: DrivingOptions,
          unitSystem: UnitSystem,
          waypoints[]: DirectionsWaypoint,
          optimizeWaypoints: Boolean,
          provideRouteAlternatives: Boolean,
          avoidFerries: Boolean,
          avoidHighways: Boolean,
          avoidTolls: Boolean,
          */
        },
        function(response, status) {
          if (status === "OK") {
            directionsDisplay.setDirections(response);
          } else {
            console.error("Directions request failed due to " + status);
          }
        }
      );
      console.log("Finished getting the route!");
    }
  }
};
</script>
<style>
/* Infowindow styling */
.gm-style-iw {
  width: 350px !important;
  background-color: #fff !important;
  box-shadow: 0 1px 6px rgba(178, 178, 178, 0.6);
}

.gmcontrol1 {
  position: absolute;
  background-color: white;
  top: 90%;
  right: 0;
  bottom: 0;
  z-index: 10;
  width: 40px !important;
  height: 40px !important;
}
</style>