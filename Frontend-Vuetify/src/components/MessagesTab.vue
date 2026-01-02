<template>
  <v-card>
    <v-tabs v-model="tab" background-color="primary" dark>
      <v-tab>
        <v-icon left>mdi-inbox</v-icon>
        Posteingang
        <v-chip v-if="unreadCount > 0" small class="ml-2" color="red">
          {{ unreadCount }}
        </v-chip>
      </v-tab>
      <v-tab>
        <v-icon left>mdi-send</v-icon>
        Gesendet
      </v-tab>
    </v-tabs>

    <v-tabs-items v-model="tab">
      <!-- Posteingang -->
      <v-tab-item>
        <v-card flat>
          <v-card-text>
            <div v-if="loading" class="text-center py-8">
              <v-progress-circular indeterminate color="primary"></v-progress-circular>
              <div class="mt-2 grey--text">Lade Nachrichten...</div>
            </div>

            <div v-else-if="inbox.length === 0" class="text-center py-8 grey--text">
              <v-icon size="64" color="grey lighten-2">mdi-inbox</v-icon>
              <div class="mt-4">Keine Nachrichten vorhanden</div>
            </div>

            <v-list v-else two-line>
              <v-list-item
                v-for="msg in inbox"
                :key="msg.id"
                @click="openMessage(msg)"
                :class="{ 'blue lighten-5': !msg.isRead }"
                class="message-item"
              >
                <v-list-item-avatar>
                  <v-img :src="msg.senderAvatar || '/default-avatar.png'"></v-img>
                </v-list-item-avatar>

                <v-list-item-content>
                  <v-list-item-title class="d-flex align-center">
                    <span :class="{ 'font-weight-bold': !msg.isRead }">
                      {{ msg.senderName }}
                    </span>
                    <v-chip v-if="!msg.isRead" x-small class="ml-2" color="primary">
                      Neu
                    </v-chip>
                  </v-list-item-title>
                  <v-list-item-subtitle>
                    {{ msg.subject }}
                  </v-list-item-subtitle>
                  <v-list-item-subtitle class="text-caption grey--text">
                    {{ formatDate(msg.sentAt) }}
                  </v-list-item-subtitle>
                </v-list-item-content>

                <v-list-item-action v-if="msg.relatedOfferId">
                  <v-chip small outlined color="primary">
                    <v-icon small left>mdi-home</v-icon>
                    Angebot #{{ msg.relatedOfferId }}
                  </v-chip>
                </v-list-item-action>
              </v-list-item>
            </v-list>

            <!-- Pagination -->
            <v-pagination
              v-if="inboxPages > 1"
              v-model="inboxPage"
              :length="inboxPages"
              @input="loadInbox"
              class="mt-4"
            ></v-pagination>
          </v-card-text>
        </v-card>
      </v-tab-item>

      <!-- Gesendet -->
      <v-tab-item>
        <v-card flat>
          <v-card-text>
            <div v-if="loading" class="text-center py-8">
              <v-progress-circular indeterminate color="primary"></v-progress-circular>
              <div class="mt-2 grey--text">Lade Nachrichten...</div>
            </div>

            <div v-else-if="sent.length === 0" class="text-center py-8 grey--text">
              <v-icon size="64" color="grey lighten-2">mdi-send</v-icon>
              <div class="mt-4">Keine gesendeten Nachrichten</div>
            </div>

            <v-list v-else two-line>
              <v-list-item
                v-for="msg in sent"
                :key="msg.id"
                @click="openMessage(msg)"
                class="message-item"
              >
                <v-list-item-avatar>
                  <v-img :src="msg.receiverAvatar || '/default-avatar.png'"></v-img>
                </v-list-item-avatar>

                <v-list-item-content>
                  <v-list-item-title>
                    An: {{ msg.receiverName }}
                  </v-list-item-title>
                  <v-list-item-subtitle>
                    {{ msg.subject }}
                  </v-list-item-subtitle>
                  <v-list-item-subtitle class="text-caption grey--text">
                    {{ formatDate(msg.sentAt) }}
                    <v-icon v-if="msg.isRead" small color="success" class="ml-1">
                      mdi-check-all
                    </v-icon>
                    <span v-if="msg.isRead" class="green--text text--darken-1 ml-1">
                      Gelesen
                    </span>
                  </v-list-item-subtitle>
                </v-list-item-content>

                <v-list-item-action v-if="msg.relatedOfferId">
                  <v-chip small outlined color="primary">
                    <v-icon small left>mdi-home</v-icon>
                    Angebot #{{ msg.relatedOfferId }}
                  </v-chip>
                </v-list-item-action>
              </v-list-item>
            </v-list>

            <!-- Pagination -->
            <v-pagination
              v-if="sentPages > 1"
              v-model="sentPage"
              :length="sentPages"
              @input="loadSent"
              class="mt-4"
            ></v-pagination>
          </v-card-text>
        </v-card>
      </v-tab-item>
    </v-tabs-items>

    <!-- Message Detail Dialog -->
    <v-dialog v-model="showDetail" max-width="800">
      <v-card v-if="selectedMessage">
        <v-card-title class="headline blue lighten-5">
          {{ selectedMessage.subject }}
          <v-spacer></v-spacer>
          <v-btn icon @click="showDetail = false">
            <v-icon>mdi-close</v-icon>
          </v-btn>
        </v-card-title>

        <v-card-text class="pt-4">
          <!-- Header -->
          <div class="message-header mb-4">
            <div class="d-flex align-center mb-2">
              <v-avatar size="40" class="mr-3">
                <v-img :src="getSenderAvatar(selectedMessage)"></v-img>
              </v-avatar>
              <div>
                <div class="font-weight-bold">{{ getSenderName(selectedMessage) }}</div>
                <div class="text-caption grey--text">
                  {{ formatDate(selectedMessage.sentAt) }}
                </div>
              </div>
            </div>
          </div>

          <v-divider class="mb-4"></v-divider>

          <!-- Content -->
          <div class="message-content" style="white-space: pre-wrap; line-height: 1.6;">
            {{ selectedMessage.content }}
          </div>

          <!-- Related Offer -->
          <v-chip
            v-if="selectedMessage.relatedOfferId"
            class="mt-4"
            color="primary"
            outlined
            @click="goToOffer(selectedMessage.relatedOfferId)"
          >
            <v-icon left>mdi-home</v-icon>
            Zum Angebot gehen
          </v-chip>
        </v-card-text>

        <v-card-actions class="px-6 pb-4">
          <v-btn
            v-if="tab === 0"
            color="primary"
            @click="reply"
          >
            <v-icon left>mdi-reply</v-icon>
            Antworten
          </v-btn>
          <v-spacer></v-spacer>
          <v-btn text color="error" @click="deleteMessage(selectedMessage.id)">
            <v-icon left>mdi-delete</v-icon>
            Löschen
          </v-btn>
          <v-btn text @click="showDetail = false">
            Schließen
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-card>
</template>

<script>
import axios from 'axios';

export default {
  name: 'MessagesTab',

  data() {
    return {
      tab: 0,
      inbox: [],
      sent: [],
      unreadCount: 0,
      inboxPage: 1,
      sentPage: 1,
      pageSize: 20,
      inboxPages: 1,
      sentPages: 1,
      loading: false,
      showDetail: false,
      selectedMessage: null
    };
  },

  mounted() {
    this.loadInbox();
    this.loadSent();
    this.loadUnreadCount();
  },

  watch: {
    tab(val) {
      if (val === 0) {
        this.loadInbox();
        this.loadUnreadCount();
      } else {
        this.loadSent();
      }
    }
  },

  methods: {
    async loadInbox() {
      this.loading = true;
      try {
        const response = await axios.get('/api/contact/inbox', {
          params: {
            page: this.inboxPage,
            pageSize: this.pageSize
          }
        });
        
        this.inbox = response.data.messages || [];
        this.inboxPages = response.data.pageCount || 1;
      } catch (error) {
        console.error('Fehler beim Laden des Posteingangs:', error);
        if (this.$toast) {
          this.$toast.error('Posteingang konnte nicht geladen werden');
        }
      } finally {
        this.loading = false;
      }
    },

    async loadSent() {
      this.loading = true;
      try {
        const response = await axios.get('/api/contact/sent', {
          params: {
            page: this.sentPage,
            pageSize: this.pageSize
          }
        });
        
        this.sent = response.data.messages || [];
        this.sentPages = response.data.pageCount || 1;
      } catch (error) {
        console.error('Fehler beim Laden gesendeter Nachrichten:', error);
        if (this.$toast) {
          this.$toast.error('Gesendete Nachrichten konnten nicht geladen werden');
        }
      } finally {
        this.loading = false;
      }
    },

    async loadUnreadCount() {
      try {
        const response = await axios.get('/api/contact/unread-count');
        this.unreadCount = response.data.count || 0;
      } catch (error) {
        console.error('Fehler beim Laden der ungelesenen Nachrichten:', error);
      }
    },

    async openMessage(msg) {
      this.selectedMessage = msg;
      this.showDetail = true;

      // Als gelesen markieren wenn Posteingang
      if (!msg.isRead && this.tab === 0) {
        try {
          await axios.put(`/api/contact/${msg.id}/mark-read`);
          msg.isRead = true;
          msg.readAt = new Date().toISOString();
          this.unreadCount = Math.max(0, this.unreadCount - 1);
        } catch (error) {
          console.error('Fehler beim Markieren als gelesen:', error);
        }
      }
    },

    reply() {
      if (!this.selectedMessage) return;
      
      // Emit Event für Parent-Komponente um ContactModal zu öffnen
      this.$emit('reply', {
        receiverId: this.selectedMessage.senderId,
        receiverName: this.selectedMessage.senderName,
        receiverAvatar: this.selectedMessage.senderAvatar
      });
      
      this.showDetail = false;
    },

    async deleteMessage(messageId) {
      if (!confirm('Möchten Sie diese Nachricht wirklich löschen?')) {
        return;
      }

      try {
        await axios.delete(`/api/contact/${messageId}`);
        
        // Entferne aus Liste
        this.inbox = this.inbox.filter(m => m.id !== messageId);
        this.sent = this.sent.filter(m => m.id !== messageId);
        
        this.showDetail = false;
        
        if (this.$toast) {
          this.$toast.success('Nachricht gelöscht');
        }
      } catch (error) {
        console.error('Fehler beim Löschen:', error);
        if (this.$toast) {
          this.$toast.error('Nachricht konnte nicht gelöscht werden');
        }
      }
    },

    goToOffer(offerId) {
      this.$router.push({ name: 'OfferDetail', params: { id: offerId } });
    },

    getSenderName(msg) {
      return this.tab === 0 ? msg.senderName : msg.receiverName;
    },

    getSenderAvatar(msg) {
      return this.tab === 0 ? 
        (msg.senderAvatar || '/default-avatar.png') : 
        (msg.receiverAvatar || '/default-avatar.png');
    },

    formatDate(dateString) {
      const date = new Date(dateString);
      const now = new Date();
      const diff = now - date;
      const hours = Math.floor(diff / (1000 * 60 * 60));
      const days = Math.floor(diff / (1000 * 60 * 60 * 24));

      if (hours < 1) return 'Gerade eben';
      if (hours < 24) return `vor ${hours} Stunde${hours > 1 ? 'n' : ''}`;
      if (days < 7) return `vor ${days} Tag${days > 1 ? 'en' : ''}`;
      
      return date.toLocaleDateString('de-DE', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
      });
    }
  }
};
</script>

<style scoped>
.message-item {
  cursor: pointer;
  transition: background-color 0.2s;
}

.message-item:hover {
  background-color: #f5f5f5;
}
</style>
