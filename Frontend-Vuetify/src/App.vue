<template>
  <v-app class="app-root">
    <div class="layout">
      <v-main class="content">
        <router-view />
      </v-main>
      <Footer />
    </div>
  </v-app>
</template>

<script lang="ts">
import Footer from "@/components/Footer.vue";
export default { components: { Footer } };
</script>

<style>
html, body, #app { height: 100%; margin: 0; }

.app-root {
  min-height: 100vh;
}

.layout {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.content {
  flex: 1;
  padding-bottom: 0 !important;
}
</style>
