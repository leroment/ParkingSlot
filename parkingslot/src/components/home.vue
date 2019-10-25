<template>
  <v-content>
    <v-card>
      <v-list two-line>
        <v-list-item-group v-model="selected" multiple active-class="blue--text">
          <template v-for="(item, index) in items">
            <v-list-item :key="item.carparkName">
              <template v-slot:default="{ active, toggle }">
                <v-list-item-content>
                  <v-list-item-title v-text="item.carparkName"></v-list-item-title>
                  <v-list-item-subtitle v-text="item.carparkLocation"></v-list-item-subtitle>
                </v-list-item-content>
                <v-list-item-action>
                  <v-list-item-action-text v-text="item.count"></v-list-item-action-text>
                  <v-icon v-if="!active" color="grey lighten-1">mdi-heart-outline</v-icon>
                  <v-icon v-else color="red">mdi-heart</v-icon>
                </v-list-item-action>
              </template>
            </v-list-item>
            <v-divider v-if="index + 1 < items.length" :key="index"></v-divider>
          </template>
        </v-list-item-group>
      </v-list>
    </v-card>
  </v-content>
</template>


<script>
export default {
  data: () => ({
    selected: [2],
    items: [
      {
        count: "739",
        carparkName: "Harbourfront Centre1",
        carparkLocation: "1 Martime Square"
      },
      {
        count: "2394",
        carparkName: "Resorts World Sentosa",
        carparkLocation: "8 Sentosa Gateway"
      },
      {
        count: "1498",
        carparkName: "Sentosa",
        carparkLocation: "Beach View Road"
      },
      {
        count: "767",
        carparkName: "VivoCity P2",
        carparkLocation: "1 Harbourfront Walk"
      },
      {
        count: "1219",
        carparkName: "VivoCity P3",
        carparkLocation: "1 Harbourfront Walk"
      },
      {
        count: "477",
        carparkName: "Marina Bay Sands",
        carparkLocation: "10 Bayfront Ave"
      },
      {
        count: "1857",
        carparkName: "Marina Square",
        carparkLocation: "6 Raffles Blvd"
      },
      {
        count: "1277",
        carparkName: "Millenia Singapore",
        carparkLocation: "9 Raffles Boulevard"
      },
      {
        count: "204",
        carparkName: "National Gallery",
        carparkLocation: "1 St Andrew's Rd"
      },
      {
        count: "803",
        carparkName: "Raffles City",
        carparkLocation: "252 North Bridge Rd"
      },
      {
        count: "0",
        carparkName: "Singapore Flyer",
        carparkLocation: "30 Raffles Ave"
      },
      {
        count: "2358",
        carparkName: "Suntec city",
        carparkLocation: "3 Temasek Blvd"
      },
      {
        count: "795",
        carparkName: "The Esplanade",
        carparkLocation: "1 Esplanade Dr"
      },
      {
        count: "195",
        carparkName: "313 @ Somerset",
        carparkLocation: "313 Orchard Rd"
      },
      {
        count: "343",
        carparkName: "Centrepoint",
        carparkLocation: "175 Orchard Road"
      }
    ]
  })
};
</script>