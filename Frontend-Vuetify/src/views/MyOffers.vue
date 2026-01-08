<template>
  <Navbar />
  <div class="inner_banner_layout">
    <div class="container">
      <div class="row">
        <div class="col-sm-12">
          <div class="inner_banner">
            <h2>Meine Inserate</h2>
          </div>
        </div>
      </div>
    </div>
  </div>
  <section class="section_space offers_list">
    <div class="container">
      <div class="row">
        <div class="col-sm-12">
          <div class="sort-new-button">
            
            <div class="add-new-offer d-flex flex-wrap gap-2 align-items-center mt-3">
              <router-link class="btn-primary-ugh" to="/add-offer">
                <i class="ri-add-circle-line"></i> Neues Angebot einstellen
              </router-link>
              
              <router-link class="btn-gesuch-ugh" to="/add-gesuch">
                <i class="ri-search-line"></i> Neues Gesuch einstellen
              </router-link>
            </div>

          </div>
           
          <div class="status-filter mt-3 mb-3">
            <div class="form-check">
              <input class="form-check-input" type="checkbox" v-model="showInactive" id="showInactiveCheck" @change="onShowInactiveChange">
              <label class="form-check-label" for="showInactiveCheck">
                Deaktivierte Angebote einblenden
              </label>
            </div>
          </div>
          <div v-if="offers && offers.length > 0" class="offers_group">
            <div v-if="loading" class="spinner-container text-center">
              <div class="spinner"></div>
            </div>
            <div v-else class="row">
              <div v-for="offer in filteredOffers" :key="offer.id" class="col-md-3 mb-4">
                <div class="all_items card-offer">
                  <div class="d-flex align-items-center mb-1">
                    <span class="fw-bold">{{ offer.title }}</span>
                  </div>
                  <OfferCard :offer="offer" :showStatus="true">
                    <template #actions>
                      <div class="d-flex flex-wrap gap-2 justify-content-center">
                        <button class="btn-primary-ugh btn-sm" @click.stop="shareOnFacebook(offer)">
                          <i class="ri-facebook-fill"></i> Teilen
                        </button>
                      </div>
                      
                      <div class="d-flex flex-wrap gap-2 justify-content-center mt-1">
                        <button v-if="offer.status === 0"
                          class="btn btn-outline-danger btn-sm"
                          @click.stop="deactivateOffer(offer.id)"
                          title="Angebot vorübergehend deaktivieren"
                        >
                          <i class="ri-pause-circle-line"></i> Deaktivieren
                        </button>

                        <button v-if="offer.status === 1"
                          class="btn btn-outline-success btn-sm"
                          @click.stop="reactivateOffer(offer.id)"
                          :disabled="!offer.canReactivate"
                          :title="!offer.canReactivate ? 'Zeitraum abgelaufen.' : 'Angebot wieder aktivieren'"
                        >
                          <i class="ri-play-circle-line"></i> Reaktivieren
                        </button>
                      </div>
                    </template>
                  </OfferCard>
                </div>
              </div>
            </div>
             <div class="pagination">
              <button class="btn-arrow-ugh me-2" @click="changePage(currentPage - 1)" :disabled="currentPage === 1"><i class="ri-arrow-left-s-line"></i></button>
              <span>Seite {{ Math.max(currentPage, 1) }} von {{ Math.max(totalPages, 1) }}</span>
              <button class="btn-arrow-ugh ms-2" @click="changePage(currentPage + 1)" :disabled="currentPage === totalPages"><i class="ri-arrow-right-s-line"></i></button>
            </div>
          </div>
          <div v-else-if="!loading">
            <h2 class="text-center">Keine Inserate gefunden!</h2>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>
<script setup lang="ts">
import OfferCard from '@/components/offer/OfferCard.vue';
import Swal from 'sweetalert2';
import Navbar from '@/components/navbar/Navbar.vue';
import toast from '@/components/toaster/toast';
import axiosInstance from "@/interceptor/interceptor"
import Securitybot from '@/services/SecurityBot';
import { ref, onMounted, computed } from 'vue';

declare global {
  interface Window {
    FontAwesomeConfig?: any;
  }
}

window.FontAwesomeConfig = {
  autoReplaceSvg: false
};

const loading = ref(true);
const offers = ref([]);
const searchTerm = ref('');
const searchTimeout = ref(null);
const currentIndex = ref(0);
const currentPage = ref(1);
const totalPages = ref(1);
const pageSize = ref(8);
const showInactive = ref(false);
const sortBy = ref('newest');

const filteredOffers = computed(() => {
  return offers.value;
});

const fetchOffers = async () => {
  try {
    const response = await axiosInstance.get(`offer/get-offer-by-user`, {
      params: {
        searchTerm: searchTerm.value,
        pageSize: pageSize.value,
        pageNumber: currentPage.value,
        sortBy: sortBy.value,
        includeInactive: showInactive.value ? "true" : "false"
      }
    });
    offers.value = response.data.items || response.data.Items || [];
    totalPages.value = Math.ceil((response.data.totalCount || response.data.TotalCount || 0) / pageSize.value);
  } catch (error) {
    console.error('Fehler beim Laden der eigenen Angebote:', error);
    toast.error('Eigene Einträge konnten nicht geladen werden.');
  } finally {
    loading.value = false;
  }
};

const changePage = (newPage: number) => {
  if (newPage >= 1 && newPage <= totalPages.value) {
    currentPage.value = newPage;
    fetchOffers();
  }
};

const searchOffers = () => {
  loading.value = true;
  currentPage.value = 1;
  fetchOffers();
};

const debouncedSearch = () => {
  if (searchTimeout.value) {
    clearTimeout(searchTimeout.value);
  }
  searchTimeout.value = setTimeout(searchOffers, 500);
};

const onShowInactiveChange = () => {
  loading.value = true;
  currentPage.value = 1;
  fetchOffers();
};

// Einzeln Reaktivieren
const reactivateOffer = async (offerId: number) => {
  try {
    await axiosInstance.put(`offer/reactivate/${offerId}`);
    toast.success('Inserat wieder aktiviert!');
    fetchOffers();
  } catch (error) {
    toast.error('Konnte nicht aktiviert werden.');
  }
};

// Einzeln Deaktivieren (mit korrektem Endpunkt)
const deactivateOffer = async (offerId: number) => {
  const result = await Swal.fire({
    title: 'Inserat pausieren?',
    text: 'Das Inserat wird ausgeblendet, bleibt aber erhalten.',
    icon: 'warning',
    showCancelButton: true,
    confirmButtonColor: '#f97316',
    cancelButtonColor: '#3085d6',
    confirmButtonText: 'Ja, deaktivieren',
    cancelButtonText: 'Abbrechen'
  });

  if (result.isConfirmed) {
    try {
      await axiosInstance.put(`offer/close-offer/${offerId}`);
      toast.success('Inserat deaktiviert.');
      fetchOffers();
    } catch (error) {
      console.error(error);
      toast.error('Fehler beim Deaktivieren.');
    }
  }
};

const shareOnFacebook = (offer: any) => {
  const url = encodeURIComponent(`${window.location.origin}/offer/${offer.id}`);
  const fbUrl = `https://www.facebook.com/sharer/sharer.php?u=${url}`;
  window.open(fbUrl, '_blank', 'width=600,height=400');
};

onMounted(() => {
  fetchOffers();
  Securitybot();
});
</script>

<style>
.offers_list_page {
  margin-top: 30px;
}

.btn-gesuch-ugh {
  background-color: #f97316 !important;
  color: #ffffff !important;
  padding: 12px 25px;
  border-radius: 8px;
  font-weight: bold;
  text-decoration: none;
  display: inline-flex;
  align-items: center;
  gap: 8px;
}
.btn-gesuch-ugh:hover {
  background-color: #ea580c !important;
}

/* Neue Button Styles für Actions */
.btn-outline-danger {
    color: #dc3545;
    border-color: #dc3545;
}
.btn-outline-danger:hover {
    color: #fff;
    background-color: #dc3545;
}
</style>
