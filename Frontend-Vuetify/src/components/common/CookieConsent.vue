<template>
  <div v-if="showBanner" class="cookie-consent-overlay">
    <div class="cookie-consent-banner">
      <div class="cookie-consent-content">
        <h3>🍪 Cookie-Einstellungen</h3>
        <p>
          Wir verwenden Cookies und ähnliche Technologien, um Ihnen ein optimales Nutzungserlebnis zu bieten. 
          Einige Cookies sind für den Betrieb der Website notwendig, während andere uns helfen, 
          die Website zu verbessern und Ihnen personalisierte Inhalte anzuzeigen.
        </p>
        <p>
          Weitere Informationen finden Sie in unserer 
          <router-link to="/datenschutzerklaerung" @click="closeBannerTemporarily">Datenschutzerklärung</router-link>.
        </p>
        
        <div class="cookie-options">
          <div class="cookie-option">
            <label class="cookie-checkbox">
              <input type="checkbox" v-model="essentialCookies" disabled checked>
              <span class="checkmark"></span>
              <span class="cookie-label">
                <strong>Notwendige Cookies</strong>
                <small>Diese Cookies sind für den Betrieb der Website erforderlich und können nicht deaktiviert werden.</small>
              </span>
            </label>
          </div>
          
          <div class="cookie-option">
            <label class="cookie-checkbox">
              <input type="checkbox" v-model="analyticsCookies">
              <span class="checkmark"></span>
              <span class="cookie-label">
                <strong>Analyse-Cookies</strong>
                <small>Helfen uns zu verstehen, wie Besucher mit der Website interagieren.</small>
              </span>
            </label>
          </div>
          
          <div class="cookie-option">
            <label class="cookie-checkbox">
              <input type="checkbox" v-model="marketingCookies">
              <span class="checkmark"></span>
              <span class="cookie-label">
                <strong>Marketing-Cookies</strong>
                <small>Werden verwendet, um Besuchern relevante Werbung anzuzeigen.</small>
              </span>
            </label>
          </div>
        </div>
        
        <div class="cookie-buttons">
          <button class="btn-cookie-reject" @click="rejectAll">
            Nur notwendige
          </button>
          <button class="btn-cookie-settings" @click="saveSettings">
            Auswahl speichern
          </button>
          <button class="btn-cookie-accept" @click="acceptAll">
            Alle akzeptieren
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';

const showBanner = ref(false);
const essentialCookies = ref(true);
const analyticsCookies = ref(false);
const marketingCookies = ref(false);

const COOKIE_CONSENT_KEY = 'ugh_cookie_consent';
const COOKIE_SETTINGS_KEY = 'ugh_cookie_settings';

onMounted(() => {
  const consent = localStorage.getItem(COOKIE_CONSENT_KEY);
  if (!consent) {
    showBanner.value = true;
  } else {
    // Lade gespeicherte Einstellungen
    const settings = localStorage.getItem(COOKIE_SETTINGS_KEY);
    if (settings) {
      const parsed = JSON.parse(settings);
      analyticsCookies.value = parsed.analytics || false;
      marketingCookies.value = parsed.marketing || false;
    }
  }
});

const saveConsent = () => {
  const settings = {
    essential: true,
    analytics: analyticsCookies.value,
    marketing: marketingCookies.value,
    timestamp: new Date().toISOString()
  };
  
  localStorage.setItem(COOKIE_CONSENT_KEY, 'true');
  localStorage.setItem(COOKIE_SETTINGS_KEY, JSON.stringify(settings));
  showBanner.value = false;
  
  // Hier können später Analytics etc. aktiviert werden
  if (analyticsCookies.value) {
    console.log('Analytics Cookies aktiviert');
  }
  
  if (marketingCookies.value) {
    console.log('Marketing Cookies aktiviert');
  }
};

const acceptAll = () => {
  analyticsCookies.value = true;
  marketingCookies.value = true;
  saveConsent();
};

const rejectAll = () => {
  analyticsCookies.value = false;
  marketingCookies.value = false;
  saveConsent();
};

const saveSettings = () => {
  saveConsent();
};

const closeBannerTemporarily = () => {
  // Banner kurz schließen damit User Datenschutz lesen kann
};

const reopenBanner = () => {
  showBanner.value = true;
};

defineExpose({ reopenBanner });
</script>

<style scoped>
.cookie-consent-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.7);
  z-index: 99999;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 20px;
}

.cookie-consent-banner {
  background: #ffffff;
  border-radius: 16px;
  max-width: 600px;
  width: 100%;
  max-height: 90vh;
  overflow-y: auto;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.3);
}

.cookie-consent-content {
  padding: 30px;
}

.cookie-consent-content h3 {
  margin: 0 0 15px 0;
  color: #333;
  font-size: 1.5rem;
}

.cookie-consent-content p {
  color: #666;
  line-height: 1.6;
  margin-bottom: 15px;
  font-size: 0.95rem;
}

.cookie-consent-content a {
  color: #0891b2;
  text-decoration: underline;
}

.cookie-options {
  margin: 25px 0;
  border-top: 1px solid #eee;
  padding-top: 20px;
}

.cookie-option {
  margin-bottom: 15px;
}

.cookie-checkbox {
  display: flex;
  align-items: flex-start;
  cursor: pointer;
  position: relative;
  padding-left: 35px;
}

.cookie-checkbox input {
  position: absolute;
  opacity: 0;
  cursor: pointer;
  height: 0;
  width: 0;
}

.cookie-checkbox .checkmark {
  position: absolute;
  left: 0;
  top: 2px;
  height: 22px;
  width: 22px;
  background-color: #eee;
  border-radius: 4px;
  border: 2px solid #ddd;
}

.cookie-checkbox:hover input ~ .checkmark {
  background-color: #ddd;
}

.cookie-checkbox input:checked ~ .checkmark {
  background-color: #0891b2;
  border-color: #0891b2;
}

.cookie-checkbox input:disabled ~ .checkmark {
  background-color: #0891b2;
  border-color: #0891b2;
  opacity: 0.7;
}

.cookie-checkbox .checkmark:after {
  content: "";
  position: absolute;
  display: none;
}

.cookie-checkbox input:checked ~ .checkmark:after {
  display: block;
}

.cookie-checkbox .checkmark:after {
  left: 6px;
  top: 2px;
  width: 6px;
  height: 12px;
  border: solid white;
  border-width: 0 2px 2px 0;
  transform: rotate(45deg);
}

.cookie-label {
  display: flex;
  flex-direction: column;
}

.cookie-label strong {
  color: #333;
  font-size: 1rem;
  margin-bottom: 3px;
}

.cookie-label small {
  color: #888;
  font-size: 0.85rem;
  line-height: 1.4;
}

.cookie-buttons {
  display: flex;
  gap: 10px;
  flex-wrap: wrap;
  margin-top: 25px;
  padding-top: 20px;
  border-top: 1px solid #eee;
}

.cookie-buttons button {
  flex: 1;
  min-width: 140px;
  padding: 12px 20px;
  border-radius: 8px;
  font-weight: bold;
  font-size: 0.95rem;
  cursor: pointer;
  transition: all 0.2s ease;
  border: none;
}

.btn-cookie-reject {
  background-color: #f3f4f6;
  color: #374151;
}

.btn-cookie-reject:hover {
  background-color: #e5e7eb;
}

.btn-cookie-settings {
  background-color: #e0f2fe;
  color: #0369a1;
}

.btn-cookie-settings:hover {
  background-color: #bae6fd;
}

.btn-cookie-accept {
  background-color: #0891b2;
  color: white;
}

.btn-cookie-accept:hover {
  background-color: #0e7490;
}

/* Mobile Responsive */
@media (max-width: 480px) {
  .cookie-consent-content {
    padding: 20px;
  }
  
  .cookie-buttons {
    flex-direction: column;
  }
  
  .cookie-buttons button {
    width: 100%;
  }
}
</style>
