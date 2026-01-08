<template>  
<Navbar />
<PublicNav />
  <section class="offer-detail-container offer_detail_layout section_space" v-if="!loading">
    <div class="container">
      <div class="row">
        <div class="col-xs-12 col-sm-6">
          <!-- Galerie-Start -->
          <div v-if="offer.images && offer.images.length > 0" class="gallery-container">
            <div class="main-image" @click="openLightbox = true">
              <img :src="offer.images[activeImage].src" alt="Angebotsbild" style="width:100%; max-height:350px; object-fit:contain; border-radius:8px; cursor: pointer;" />
            </div>
            <div v-if="offer.images.length > 1" class="thumbnails d-flex mt-2 gap-2">
              <img v-for="(img, idx) in offer.images" :key="idx" :src="img.src" @click="activeImage = idx" :class="['thumbnail', {active: idx === activeImage}]" style="width:60px; height:60px; object-fit:cover; border-radius:6px; border:2px solid #eee; cursor:pointer;" />
            </div>
            <!-- Lightbox -->
            <div v-if="openLightbox" class="lightbox" @click.self="openLightbox = false">
              <span class="close-btn" @click="openLightbox = false">&times;</span>
              <img :src="offer.images[activeImage].src" class="lightbox-img" />
              <div class="lightbox-thumbs">
                <img v-for="(img, idx) in offer.images" :key="idx" :src="img.src" @click.stop="activeImage = idx" :class="['lightbox-thumb', {active: idx === activeImage}]" />
              </div>
              <button v-if="activeImage > 0" class="lightbox-prev" @click.stop="activeImage--">&#8592;</button>
              <button v-if="activeImage < offer.images.length-1" class="lightbox-next" @click.stop="activeImage++">&#8594;</button>
            </div>
          </div>
          <div v-else>
            <img src="/defaultprofile.jpg" alt="Kein Bild verfügbar" style="width:100%; max-height:350px; object-fit:contain; border-radius:8px;" />
          </div>
          <!-- Galerie-Ende -->
        </div>
        <div class="col-xs-12 col-sm-6">
          <div class="offer-content">
            <h1 class="offer-title">{{ offer.title }}</h1>
            <table class="item_description">
              <!-- GEÄNDERT: Gastgeber nur für eingeloggte Benutzer anzeigen -->
              <tbody v-if="offer.hostId != null && isLoggedIn">
                <tr>
                  <td>
                    <div style="display: flex; align-items: center;"> <a class="b">Gastgeber:&nbsp;</a>
                      <UserLink :hostPic=offer.hostPicture :hostId=offer.hostId :hostName=offer.hostName />
                  </div> </td>
                </tr>
              </tbody>
              <tbody>
                <tr><a class="b">Gesuchte Fähigkeiten:</a> {{ offer.skills }}</tr>
                <tr><a class="b">Unterbringung:</a> {{ offer.accomodation || 'Nicht angegeben' }}</tr>
                <tr><a class="b">Geeignet für:</a> {{ offer.accomodationsuitable || 'Nicht angegeben' }}</tr>
                <tr v-if="isActiveMember"><a class="b">Ort/Region:</a> {{ offer.location || 'Nicht angegeben' }}</tr>
                <tr><a class="b">Angebotszeitraum:</a> {{offer.fromDate}} - {{offer.toDate}}</tr>
               </tbody>
              </table>
            
            <!-- GEÄNDERT: Apply und Kontakt nur für aktive Mitglieder -->
            <div v-if="isActiveMember">
              <Apply :offer=offer :isActiveMember=isActiveMember :logId=logId :userRole=userRole />
              <!-- Kontakt aufnehmen Button -->
              <div v-if="offer.hostId && logId !== offer.hostId" class="mt-3">
                <button class="btn-primary-ugh" @click="openContactModal">
                  <i class="ri-mail-send-line me-2"></i>Kontakt aufnehmen
                </button>
              </div>
            </div>
            
            <!-- NEU: Hinweis für Nicht-Mitglieder -->
            <div v-else class="membership-required-box mt-3 p-4">
              <div class="text-center">
                <i class="ri-lock-line" style="font-size: 48px; color: #00bdd6; margin-bottom: 16px;"></i>
                <h3 class="mb-3">Möchtest du Kontakt aufnehmen?</h3>
                <p class="mb-4">Werde Mitglied unserer Community und tausche Urlaub gegen Handarbeit!</p>
                <div class="d-flex gap-2 justify-content-center">
                  <button class="btn-primary-ugh" @click="goToLogin">
                    <i class="ri-login-box-line me-2"></i>Anmelden
                  </button>
                  <button class="btn-secondary-ugh" @click="goToRegister">
                    <i class="ri-user-add-line me-2"></i>Registrieren
                  </button>
                </div>
              </div>
            </div>
            
            <div class="offer_btn">
              <button @click="backtooffers()" class="btn-arrow-ugh"><i class="ri-arrow-go-back-line" aria-hidden="true"></i></button>
              <span class="ms-2"><span class="text-primary-blue">Zurück</span></span>
            </div>
          </div>
        </div>
      </div>
      <div class="row">
        <div class="col-sm-12">
          <div class="offer_decription mt-4">
            <VueMarkdown :source="offer.description" class="offer-description mb-0" />
          </div>
        </div>
      </div>
      
      <!-- Buttons nur für Angebots-Besitzer -->
      <div v-if="logId == offer.hostId && isActiveMember">
        <div v-if="offer.status == '0'">
          <button class="btn btn-danger" @click="closeOffer(offer)">
            Angebot schließen
          </button>
          <a v-if="!offer.applicationsExist">
           <button class="btn-primary-ugh ms-2" @click="modifyOffer()">
            Angebot modifizieren
           </button>
          </a>
          <a v-else>
            <button class="btn btn-blocked">
              Angebot schreibgeschützt.
             </button>
          </a>
        </div>
        <div v-else>
          <button class="btn btn-blocked">Angebot geschlossen</button>
          <button
            class="btn btn-success ms-2"
            @click="reactivateOffer(offer.id)"
            :disabled="!offer.canReactivate"
            :title="!offer.canReactivate ? 'Angebotszeitraum abgelaufen. Reaktivierung nicht möglich.' : 'Angebot reaktivieren'"
          >
            <i class="ri-refresh-line"></i> Reaktivieren
          </button>
        </div>        
      </div>
    </div>
  </section>
  
  <!-- Contact Modal - nur für eingeloggte Benutzer -->
  <ContactModal
    v-if="showContactModal && offer && isLoggedIn"
    v-model:show="showContactModal"
    :receiver-id="offer.hostId"
    :receiver-name="offer.hostName"
    :receiver-avatar="offer.hostPicture"
    :offer-id="offer.id"
    @sent="onMessageSent"
  />
  
  <div class="loading" v-if="loading">
    Wird geladen...
  </div>
</template>

<script setup lang="ts">
import {
  ref,
  computed,
  onMounted,
  watch
} from 'vue';
import Navbar from '@/components/navbar/Navbar.vue';
import PublicNav from '@/components/navbar/PublicNav.vue';
import axiosInstance from '@/interceptor/interceptor';
import router from '@/router';
import {isActiveMembership, GetUserRole} from '@/services/GetUserPrivileges';
import Apply from '@/components/Apply.vue';
import getLoggedUserId from '@/services/LoggedInUserId';
import UserLink from '@/components/offer/UserLink.vue';
import VueMarkdown from "vue-markdown-render";
import ContactModal from "@/components/ContactModal.vue";
import {useRoute} from 'vue-router';
import { useToast } from 'vue-toastification';

const toast = useToast();
const {params} = useRoute();

const pictureLink = `offer/get-preview-picture/${params.id}`;
const offer = ref(null);
const loading = ref(true);
const defaultProfileImgSrc = '/defaultprofile.jpg';
const activeImage = ref(0);
const openLightbox = ref(false);
const showContactModal = ref(false);

// NEU: Prüfe ob Benutzer eingeloggt ist
const isLoggedIn = computed(() => {
  return sessionStorage.getItem('token') !== null;
});

const redirectToProfile = (userId) => {
  sessionStorage.setItem("UserId", userId);
  router.push("/account");
};

const openContactModal = () => {
  console.log('Opening contact modal');
  showContactModal.value = true;
};

const onMessageSent = () => {
  toast.success("Nachricht erfolgreich gesendet!");
  showContactModal.value = false;
};

// NEU: Navigation zu Login/Register
const goToLogin = () => {
  router.push('/login');
};

const goToRegister = () => {
  router.push('/register');
};

const fetchOfferDetail = async () => {
  try {
    const response = await axiosInstance.get(`offer/get-offer-by-id/${params.id}`);
    offer.value = response.data;
  } catch (error) {
    console.error('Fehler beim Laden der Angebotsdetails:', error);
  } finally {
    loading.value = false;
  }
};

const formatDate = (dateString) => {
  const options: Intl.DateTimeFormatOptions = {
    year: 'numeric',
    month: 'long',
    day: '2-digit',
  };
  return new Date(dateString).toLocaleDateString(undefined, options);
};

const backtooffers = () => {
  window.history.back();
};

const closeOffer = async (offer) => {
  try {
    await axiosInstance.put(`/offer/close-offer/${offer.id}`);
    toast.success('Angebot wurde geschlossen.');
    await fetchOfferDetail();
  } catch (error) {
    toast.error('Fehler beim Schließen des Angebots.');
  }
};

const modifyOffer = () => {
  router.push({
    name: 'ModifyOffer', params: {id: params.id}
  });
};

const reactivateOffer = async (offerId) => {
  try {
    await axiosInstance.put(`/offer/reactivate/${offerId}`);
    toast.success('Angebot wurde reaktiviert.');
    window.location.reload();
  } catch (error) {
    toast.error((error && typeof error === 'object' && 'response' in error && (error as any).response?.data) ? (error as any).response.data : 'Fehler beim Reaktivieren des Angebots.');
  }
};

watch(() => offer.value?.images, (imgs) => { 
  if (imgs && imgs.length > 0) activeImage.value = 0; 
});

onMounted(fetchOfferDetail);
</script>

<script lang="ts">    
export default {
    data() {
        return{
            isActiveMember: isActiveMembership(),
            logId: getLoggedUserId(),
            userRole: GetUserRole(),
        };
}
}
</script>

<style scoped lang="scss">
.loading {
  font-size: 1.5rem;
  color: #888;
  text-align: center;
  padding: 40px;
}

/* NEU: Styling für Mitgliedschafts-Box */
.membership-required-box {
  background: linear-gradient(135deg, #f5f7fa 0%, #e8f0fe 100%);
  border: 2px solid #00bdd6;
  border-radius: 12px;
  box-shadow: 0 4px 6px rgba(0, 189, 214, 0.1);
}

.membership-required-box h3 {
  color: #2c3e50;
  font-weight: 600;
}

.membership-required-box p {
  color: #5a6c7d;
  font-size: 16px;
}

.btn-secondary-ugh {
  background-color: #6c757d;
  color: white;
  border: none;
  padding: 10px 24px;
  border-radius: 6px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s ease;
  display: inline-flex;
  align-items: center;
  text-decoration: none;
}

.btn-secondary-ugh:hover {
  background-color: #5a6268;
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(108, 117, 125, 0.3);
}

.modal {
  display: block;
  position: fixed;
  z-index: 1;
  left: 0;
  top: 0;
  width: 100%;
  height: 100%;
  background-color: rgb(0, 0, 0);
  background-color: rgba(0, 0, 0, 0.4);
}

.modal-content {
  background-color: #fefefe;
  margin: 15% auto;
  padding: 20px;
  border: 1px solid #888;
  width: 80%;
  max-width: 400px;
  text-align: center;
}

.close {
  color: #aaa;
  float: right;
  font-size: 28px;
  font-weight: bold;
  cursor: pointer;
}

.close:hover,
.close:focus {
  color: black;
  text-decoration: none;
}

.lightbox {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0,0,0,0.9);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 9999;
}

.lightbox-img {
  max-width: 90%;
  max-height: 80%;
  object-fit: contain;
  border-radius: 8px;
}

.close-btn {
  position: absolute;
  top: 20px;
  right: 40px;
  color: #fff;
  font-size: 40px;
  font-weight: bold;
  cursor: pointer;
}

.close-btn:hover {
  color: #ccc;
}

.lightbox-thumbs {
  position: absolute;
  bottom: 20px;
  left: 50%;
  transform: translateX(-50%);
  display: flex;
  gap: 10px;
}

.lightbox-thumb {
  width: 60px;
  height: 60px;
  object-fit: cover;
  border-radius: 6px;
  border: 2px solid #eee;
  cursor: pointer;
  opacity: 0.6;
  transition: opacity 0.3s, border-color 0.3s;
}

.lightbox-thumb.active,
.lightbox-thumb:hover {
  opacity: 1;
  border-color: #00bdd6;
}

.lightbox-prev,
.lightbox-next {
  position: absolute;
  top: 50%;
  transform: translateY(-50%);
  background: rgba(255,255,255,0.8);
  border: none;
  font-size: 30px;
  padding: 10px 20px;
  cursor: pointer;
  border-radius: 4px;
  transition: background 0.3s;
}

.lightbox-prev:hover,
.lightbox-next:hover {
  background: rgba(255,255,255,1);
}

.lightbox-prev {
  left: 20px;
}

.lightbox-next {
  right: 20px;
}
</style>
