<template>
  <v-content style="margin-bottom:55px;">
    <v-layout>
      <v-btn class="ma-2 gmcontrol1">
        <v-icon size="28" @click="geolocation" color="#333333">mdi-crosshairs-gps</v-icon>
      </v-btn>
    </v-layout>
    <gmap-map :center="center" ref="mapRef" :zoom="14" :options="mapStyle">
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
            {{ carparkItem.name }}
            <v-chip
              v-if="carparkItem.agency != undefined"
              class="ma-2"
              color="primary"
              outlined
            >{{ carparkItem.agency }}</v-chip>
          </h2>
          <v-container class>
            <v-row v-if="carparkItem.type != undefined">
              <span class="body-1">Type: {{ carparkItem.type }}</span>
            </v-row>
            <v-row v-if="carparkItem.address != undefined">
              <span class="body-1">Address: {{ carparkItem.address }}</span>
            </v-row>
            <v-row v-if="carparkItem.desc != undefined">
              <span class="body-1">{{ carparkItem.desc }}</span>
            </v-row>
          </v-container>
          <v-layout v-if="carparkItem.desc == undefined">
            <v-divider></v-divider>
            <v-btn
              class="mt-5"
              color="info"
              @click="getDirection(carparkItem)"
              type="submit"
            >Directions</v-btn>
          </v-layout>
        </gmap-info-window>
      </gmap-cluster>
    </gmap-map>
  </v-content>
</template>

<script>
/*global google*/
import mapStyles from "@/assets/mapStyle";
const mapMarker = require("../assets/mapmarker.png");
export default {
  props: {
    allmarkers: Array
  },
  data() {
    return {
      currentLocation: { lat: 0, lng: 0 },
      center: {},
      markers: [],
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
      info_marker: null,
      infowindow: { lat: 10.0, lng: 10.0 },
      window_open: false,
      carparkItem: {}
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
        this.currentLocation = {
          lat: parseFloat(position.coords.latitude),
          lng: parseFloat(position.coords.longitude)
        };
        var userloc = {
          position: {
            lat: parseFloat(this.currentLocation.lat),
            lng: parseFloat(this.currentLocation.lng)
          }
        };
        console.log(this.markers);
        //if object is empty (first init), pass in the lat and lng
        if (
          Object.entries(this.center).length === 0 &&
          this.center.constructor === Object
        ) {
          this.center = {
            lat: parseFloat(this.currentLocation.lat),
            lng: parseFloat(this.currentLocation.lng)
          };
          this.markers.push(userloc);
        } else {
          //Check if the old center is the same as the new one
          //If it is not the same, change it
          if (
            userloc.position.lat != this.center.lat ||
            userloc.position.lng != this.center.lng
          ) {
            for (var i = 0; i < this.markers.length; i++) {
              if (
                this.markers[i].position.lat == this.center.lat &&
                this.markers[i].position.lng == this.center.lng
              ) {
                this.markers[i].position.lat = userloc.lat;
                this.markers[i].position.lng = userloc.lng;
                break;
              }
            }
          }
        }
        this.$refs.mapRef.panTo(this.center);
        this.showMarkerInfo(userloc);
      });
    },
    fetchCaparks: function() {
      let cur = this;
      this.axios
        .get("https://parkingslotapi.azurewebsites.net/api/carpark/all")
        .then(function(response) {
          console.log(response);
          for (var i = 0; i < response.data.length; i++) {
            var userloc = {
              position: {
                id: response.data[i].carparkId,
                name: response.data[i].carparkName,
                type: response.data[i].lottype,
                agency: response.data[i].agencyType,
                address: response.data[i].address,
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
      this.infowindow.lat = markerInfo.position.lat;
      this.infowindow.lng = markerInfo.position.lng;
      //Move to center
      this.$refs.mapRef.panTo(this.infowindow);

      if (
        markerInfo.position.lat != this.center.lat &&
        markerInfo.position.lng != this.center.lng
      ) {
        this.carparkItem = markerInfo.position;
        this.window_open = true;
      } else {
        this.carparkItem.name = "Current location";
        this.carparkItem.desc =
          "Select a carpark to get more information and directions";
        this.window_open = true;
      }
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
        polylineOptions:{strokeColor:"green",strokeWeight:5}
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