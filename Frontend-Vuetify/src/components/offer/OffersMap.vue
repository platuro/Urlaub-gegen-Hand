<template>
  <div class="map-wrapper">
    <div id="offers-map" class="offers-map-container"></div>
    <div class="map-legend">
      <span class="legend-item"><span class="dot offer"></span> Angebot</span>
      <span class="legend-item"><span class="dot request"></span> Gesuch</span>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, onUnmounted, watch } from 'vue';
import L from 'leaflet';
import 'leaflet/dist/leaflet.css';
import router from '@/router';

const props = defineProps({
  offers: {
    type: Array,
    default: () => []
  }
});

let map = null;
let markersLayer = null;

// Hilfsfunktion: Sucht Koordinaten auch in verschachtelten Objekten (address)
const getLat = (obj) => {
  if (!obj) return null;
  if (obj.address && obj.address.latitude) return obj.address.latitude; // Hier liegen deine Daten
  if (obj.latitude !== undefined) return obj.latitude;
  if (obj.Latitude !== undefined) return obj.Latitude;
  return null;
};

const getLng = (obj) => {
  if (!obj) return null;
  if (obj.address && obj.address.longitude) return obj.address.longitude; // Hier liegen deine Daten
  if (obj.longitude !== undefined) return obj.longitude;
  if (obj.Longitude !== undefined) return obj.Longitude;
  return null;
};

const initMap = () => {
  // Standard: Deutschland Mitte
  let center = [51.1657, 10.4515]; 
  let zoom = 6;

  const container = document.getElementById('offers-map');
  if (!container) return;

  if (map) { map.remove(); map = null; }

  map = L.map('offers-map').setView(center, zoom);

  L.tileLayer('https://{s}.basemaps.cartocdn.com/rastertiles/voyager/{z}/{x}/{y}{r}.png', {
    attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors &copy; <a href="https://carto.com/attributions">CARTO</a>',
    subdomains: 'abcd',
    maxZoom: 19
  }).addTo(map);

  markersLayer = L.featureGroup().addTo(map);
  
  updateMarkers();
};

const updateMarkers = () => {
  if (!map || !markersLayer) return;

  markersLayer.clearLayers();
  let validCount = 0;

  props.offers.forEach(offer => {
    const rawLat = getLat(offer);
    const rawLng = getLng(offer);

    // Robustes Parsing (Komma zu Punkt)
    let lat = parseFloat(String(rawLat).replace(',', '.'));
    let lng = parseFloat(String(rawLng).replace(',', '.'));

    if (!isNaN(lat) && !isNaN(lng) && lat !== 0 && lng !== 0) {
      const isGesuch = offer.listingType === 1;
      const color = isGesuch ? '#f97316' : '#0891b2'; 

      const marker = L.circleMarker([lat, lng], {
        radius: 8,
        fillColor: color,
        color: "#fff",
        weight: 2,
        opacity: 1,
        fillOpacity: 0.9
      });

      const popupContent = document.createElement('div');
      popupContent.innerHTML = `
        <div style="text-align:center; min-width: 150px; font-family: sans-serif;">
          <strong style="display:block; margin-bottom:4px; color:#333; font-size:14px;">${offer.title}</strong>
          <span style="background:${color}; color:#fff; padding:2px 8px; border-radius:10px; font-size:11px; font-weight: bold;">
            ${isGesuch ? 'GESUCH' : 'ANGEBOT'}
          </span>
          <div style="margin-top:8px; color:#0891b2; font-size:12px; cursor:pointer;">
            Klicken für Details &rarr;
          </div>
        </div>
      `;
      
      popupContent.onclick = () => {
        router.push({ name: 'OfferDetail', params: { id: offer.id } });
      };

      marker.bindPopup(popupContent);
      markersLayer.addLayer(marker);
      validCount++;
    }
  });

  if (validCount > 0) {
    map.fitBounds(markersLayer.getBounds(), { padding: [50, 50] });
  }
};

watch(() => props.offers, () => {
  updateMarkers();
}, { deep: true });

onMounted(() => {
  setTimeout(initMap, 500);
});

onUnmounted(() => {
  if (map) map.remove();
});
</script>

<style scoped>
.map-wrapper { position: relative; margin-bottom: 30px; }
.offers-map-container {
  width: 100%;
  height: 550px;
  border-radius: 12px;
  box-shadow: 0 4px 12px rgba(0,0,0,0.1);
  background: #eee;
  z-index: 1;
}
.map-legend {
  position: absolute;
  bottom: 20px;
  right: 20px;
  background: white;
  padding: 10px 15px;
  border-radius: 8px;
  box-shadow: 0 2px 10px rgba(0,0,0,0.2);
  z-index: 1000; 
  display: flex;
  gap: 15px;
  font-size: 14px;
  font-weight: bold;
}
.legend-item { display: flex; align-items: center; gap: 6px; }
.dot { width: 12px; height: 12px; border-radius: 50%; display: inline-block; }
.dot.offer { background-color: #0891b2; border: 2px solid white; box-shadow: 0 0 2px #000;}
.dot.request { background-color: #f97316; border: 2px solid white; box-shadow: 0 0 2px #000;}
</style>
