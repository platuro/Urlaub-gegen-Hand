<template>
  <v-app class="app-root">
      <CookieConsent />
      <router-view />
      <Footer />
  </v-app>
</template>

<script lang="ts">
import CookieConsent from '@/components/common/CookieConsent.vue';
import Footer from "@/components/Footer.vue";

export default { 
  components: { 
    Footer, 
    CookieConsent 
  } 
};
</script>

<style>
/* Hier können globale Styles hin */
</style>
