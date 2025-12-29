<template>
  <v-app class="app-root">
      <router-view />
      <Footer />
  </v-app>
</template>

<script lang="ts">
import Footer from "@/components/Footer.vue";
export default { components: { Footer } };
</script>

<style>

</style>
