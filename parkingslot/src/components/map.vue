<template>
  <gmap-map id="google-map" :center="center" ref="mapRef" :zoom="zoom" :options="mapStyle">
    <gmap-marker :position="center" :icon="userMarkerOptions" @click="selectUserLocation()">
      <gmap-info-window :position="center" @closeclick="currentinfo=false" :opened="currentinfo">
        <h2>Current location</h2>
        <v-container class>
          <v-row>
            <span class="body-1">Select a carpark to get more information and directions</span>
          </v-row>
        </v-container>
      </gmap-info-window>
    </gmap-marker>
    <CarparkFilter class="filterBtn" id="filter" @clicked="onFilter"></CarparkFilter>
    <v-toolbar id="filterBar" dense floating>
      <v-text-field hide-details prepend-icon="mdi-magnify" single-line></v-text-field>
      <v-btn icon>
        <v-icon color="blue">mdi-crosshairs-gps</v-icon>
      </v-btn>
      <v-btn icon>
        <v-icon>mdi-dots-horizontal</v-icon>
      </v-btn>
    </v-toolbar>
    <v-btn id="gpsBtn" @click="geolocation" class="ma-2 gmcontrol1">
      <v-icon size="28" color="blue darken-1">mdi-crosshairs-gps</v-icon>
    </v-btn>
  </gmap-map>
</template>

<script>
/*global google*/
import mapStyles from "@/assets/mapStyle";
import * as MarkerClusterer from "marker-clusterer-plus";
import CarparkFilter from "./utils/filter";
const mapMarker = require("../assets/mapmarker.png");
const carMarker = require("../assets/usermarker.png");
var gmap;
export default {
  components: {
    CarparkFilter
  },
  data() {
    return {
      center: {
        lat: 1.3521,
        lng: 103.8198
      },
      zoom: 14,
      markers: [],
      markerItem: {},
      carparkItem: {},
      filterConfig: this.$store.getters.FILTER,
      mapStyle: {
        styles: mapStyles,
        disableDefaultUi: false,
        zoomControl: false,
        mapTypeControl: false,
        scaleControl: true,
        streetViewControl: false,
        rotateControl: true,
        fullscreenControl: false
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
      infoWindow: {
        open: true
      },
      currentinfo: true
    };
  },
  mounted: function() {
    /* Do not remove */
    /* Need to add mapref on gmaps */
    /* https://github.com/xkjyeah/vue-google-maps/issues/403 */
    this.$refs.mapRef.$mapPromise.then(map => {
      //google api global object - important
      gmap = map;
      this.initGmaps();
      this.geolocation();
      this.fetchCaparks();
    });
  },
  methods: {
    onFilter(filterConfig) {
      //Pass the updated filter config
      //Filter by the markers
      this.filterConfig = filterConfig;
    },
    initGmaps: function() {
      var filterBar = document.getElementById("filter");
      var gpsBtn = document.getElementById("gpsBtn");
      this.mapObject = this.$refs.mapRef.$mapObject;
      this.mapObject.controls[google.maps.ControlPosition.TOP_RIGHT].push(
        filterBar
      );
      this.mapObject.controls[google.maps.ControlPosition.RIGHT_BOTTOM].push(
        gpsBtn
      );
    },
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
        .get("https://parkingslotapi.azurewebsites.net/api/carpark/all")
        .then(function(response) {
          var markers = [];
          for (var i = 0; i < response.data.length; i++) {
            //Remove zero coordinates marker
            if (
              response.data[i].xCoord != "0" &&
              response.data[i].yCoord != "0"
            ) {
              var userloc = {
                position: {
                  id: response.data[i].id,
                  carparkId: response.data[i].carparkId,
                  lat: parseFloat(response.data[i].xCoord),
                  lng: parseFloat(response.data[i].yCoord)
                }
              };
              var marker = new google.maps.Marker({
                position: userloc.position,
                map: gmap,
                icon: cur.markerOptions
              });
              markers.push(marker);
            }
          }
          var markerCluster = new MarkerClusterer(gmap, markers);
        });
    },
    selectUserLocation: function() {
      this.currentinfo = !this.currentinfo;
      this.$refs.mapRef.panTo(this.center);
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

#google-map {
  width: 100%;
  height: 100%;
}

#filterBar {
  width: 300px;
  margin-top: 10px !important;
  margin: 0 auto;
}

.filterBtn .filter input{
  color: white !important;
}
</style>