<template>
  <Navbar />
  <PublicNav />
  
  <section class="messages-container section_space">
    <div class="container">
      <div class="row">
        <div class="col-12">
          <h1 class="page-title">
            <i class="ri-mail-line me-2"></i>Nachrichten
            <span v-if="unreadCount > 0" class="badge bg-primary ms-2">{{ unreadCount }}</span>
          </h1>
          
          <!-- Tabs -->
          <div class="tabs mb-4">
            <button 
              :class="['tab-btn', { active: activeTab === 'inbox' }]"
              @click="activeTab = 'inbox'; loadMessages()"
            >
              <i class="ri-inbox-line me-1"></i>
              Posteingang
              <span v-if="unreadCount > 0" class="badge bg-danger ms-1">{{ unreadCount }}</span>
            </button>
            <button 
              :class="['tab-btn', { active: activeTab === 'sent' }]"
              @click="activeTab = 'sent'; loadMessages()"
            >
              <i class="ri-send-plane-line me-1"></i>
              Gesendet
            </button>
          </div>
          
          <!-- Loading -->
          <div v-if="loading" class="text-center py-5">
            <div class="spinner-border text-primary" role="status">
              <span class="visually-hidden">Lädt...</span>
            </div>
          </div>
          
          <!-- Keine Nachrichten -->
          <div v-else-if="messages.length === 0" class="no-messages text-center py-5">
            <i class="ri-mail-line" style="font-size: 4rem; color: #ccc;"></i>
            <p class="text-muted mt-3">
              {{ activeTab === 'inbox' ? 'Keine empfangenen Nachrichten' : 'Keine gesendeten Nachrichten' }}
            </p>
          </div>
          
          <!-- Nachrichten-Liste -->
          <div v-else class="messages-list">
            <div 
              v-for="message in messages" 
              :key="message.id"
              :class="['message-card', { unread: !message.isRead && activeTab === 'inbox' }]"
              @click="openMessage(message)"
            >
              <div class="message-avatar">
                <img 
                  v-if="getMessageAvatar(message)" 
                  :src="getMessageAvatar(message)" 
                  alt="Avatar"
                />
                <div v-else class="default-avatar">
                  <i class="ri-user-line"></i>
                </div>
              </div>
              
              <div class="message-content">
                <div class="message-header">
                  <strong>{{ getMessageName(message) }}</strong>
                  <span class="message-date">{{ formatDate(message.sentAt) }}</span>
                </div>
                <div class="message-subject">{{ message.subject }}</div>
                <div class="message-preview">{{ message.content }}</div>
                <div v-if="message.relatedOfferId" class="message-offer-link">
                  <i class="ri-link me-1"></i>Bezieht sich auf Angebot #{{ message.relatedOfferId }}
                </div>
              </div>
              
              <div class="message-actions">
                <button 
                  class="btn-icon" 
                  @click.stop="deleteMessage(message.id)"
                  title="Löschen"
                >
                  <i class="ri-delete-bin-line"></i>
                </button>
              </div>
            </div>
          </div>
          
          <!-- Pagination -->
          <div v-if="pageCount > 1" class="pagination-wrapper mt-4">
            <button 
              class="btn btn-outline-primary" 
              :disabled="currentPage === 1"
              @click="changePage(currentPage - 1)"
            >
              <i class="ri-arrow-left-line"></i> Zurück
            </button>
            <span class="mx-3">Seite {{ currentPage }} von {{ pageCount }}</span>
            <button 
              class="btn btn-outline-primary" 
              :disabled="currentPage === pageCount"
              @click="changePage(currentPage + 1)"
            >
              Weiter <i class="ri-arrow-right-line"></i>
            </button>
          </div>
        </div>
      </div>
    </div>
  </section>
  
  <!-- Message Detail Modal -->
  <div v-if="selectedMessage" class="modal-overlay" @click="closeMessageModal">
    <div class="modal-content message-modal" @click.stop>
      <div class="modal-header">
        <h3>{{ selectedMessage.subject }}</h3>
        <button class="close-btn" @click="closeMessageModal">
          <i class="ri-close-line"></i>
        </button>
      </div>
      
      <div class="modal-body">
        <div class="message-detail-header">
          <div class="d-flex align-items-center mb-3">
            <div class="message-avatar me-3">
              <img 
                v-if="getMessageAvatar(selectedMessage)" 
                :src="getMessageAvatar(selectedMessage)" 
                alt="Avatar"
              />
              <div v-else class="default-avatar">
                <i class="ri-user-line"></i>
              </div>
            </div>
            <div>
              <strong>{{ getMessageName(selectedMessage) }}</strong>
              <div class="text-muted small">{{ formatDate(selectedMessage.sentAt) }}</div>
            </div>
          </div>
        </div>
        
        <div class="message-detail-content">
          {{ selectedMessage.content }}
        </div>
        
        <div v-if="selectedMessage.relatedOfferId" class="mt-3">
          <router-link 
            :to="`/offer/${selectedMessage.relatedOfferId}`"
            class="btn btn-sm btn-outline-primary"
          >
            <i class="ri-external-link-line me-1"></i>
            Angebot ansehen
          </router-link>
        </div>
      </div>
      
      <div class="modal-footer">
        <button 
          v-if="activeTab === 'inbox' && selectedMessage" 
          class="btn btn-primary btn-reply" 
          @click="openReplyModal"
        >
          <i class="ri-reply-line me-1"></i>Antworten
        </button>
        <button class="btn btn-secondary" @click="closeMessageModal">
          Schließen
        </button>
      </div>
    </div>
  </div>
  
  <!-- Reply Modal -->
  <div v-if="showReplyModal" class="modal-overlay" @click="closeReplyModal">
    <div class="modal-content reply-modal" @click.stop>
      <div class="modal-header">
        <h3>Antworten</h3>
        <button class="close-btn" @click="closeReplyModal">
          <i class="ri-close-line"></i>
        </button>
      </div>
      
      <div class="modal-body">
        <div class="reply-to-info mb-3">
          <strong>An:</strong> {{ getMessageName(selectedMessage) }}
        </div>
        
        <div class="form-group">
          <label>Betreff</label>
          <input 
            v-model="replySubject" 
            type="text" 
            class="form-control" 
            placeholder="Betreff"
          />
        </div>
        
        <div class="form-group">
          <label>Nachricht *</label>
          <textarea 
            v-model="replyContent" 
            class="form-control" 
            rows="8" 
            placeholder="Schreibe deine Antwort..."
            maxlength="2000"
          ></textarea>
          <small class="text-muted">{{ replyContent.length }} / 2000 Zeichen</small>
        </div>
      </div>
      
      <div class="modal-footer">
        <button class="btn btn-secondary" @click="closeReplyModal">
          Abbrechen
        </button>
        <button 
          class="btn btn-primary" 
          @click="sendReply" 
          :disabled="!replyContent.trim()"
        >
          <i class="ri-send-plane-line me-1"></i>Senden
        </button>
      </div>
    </div>
  </div>
</template>

<script>
import Navbar from '@/components/navbar/Navbar.vue';
import PublicNav from '@/components/navbar/PublicNav.vue';
import axiosInstance from '@/interceptor/interceptor';
import { useToast } from 'vue-toastification';

export default {
  name: 'Messages',
  components: {
    Navbar,
    PublicNav
  },
  
  data() {
    return {
      activeTab: 'inbox',
      messages: [],
      unreadCount: 0,
      loading: false,
      currentPage: 1,
      pageCount: 1,
      totalCount: 0,
      selectedMessage: null,
      showReplyModal: false,
      replySubject: '',
      replyContent: ''
    };
  },
  
  mounted() {
    this.loadMessages();
    this.loadUnreadCount();
  },
  
  methods: {
    openReplyModal() {
      if (!this.selectedMessage) return;
      
      const originalSubject = this.selectedMessage.subject || 'Neue Nachricht';
      this.replySubject = originalSubject.startsWith('Re:') 
        ? originalSubject 
        : 'Re: ' + originalSubject;
      
      this.replyContent = '';
      this.showReplyModal = true;
    },
    
    closeReplyModal() {
      this.showReplyModal = false;
      this.replySubject = '';
      this.replyContent = '';
    },
    
    async sendReply() {
      if (!this.replyContent.trim() || !this.selectedMessage) return;
      
      try {
        await axiosInstance.post('/contact/send', {
          receiverId: this.selectedMessage.senderId,
          subject: this.replySubject,
          content: this.replyContent,
          relatedOfferId: this.selectedMessage.relatedOfferId
        });
        
        useToast().success('Antwort gesendet!');
        this.closeReplyModal();
        this.closeMessageModal();
        this.loadMessages();
      } catch (error) {
        console.error('Fehler beim Senden:', error);
        useToast().error('Antwort konnte nicht gesendet werden');
      }
    },

    openReplyModal() {
      if (!this.selectedMessage) return;
      
      const originalSubject = this.selectedMessage.subject || 'Neue Nachricht';
      this.replySubject = originalSubject.startsWith('Re:') 
        ? originalSubject 
        : 'Re: ' + originalSubject;
      
      this.replyContent = '';
      this.showReplyModal = true;
    },
    
    closeReplyModal() {
      this.showReplyModal = false;
      this.replySubject = '';
      this.replyContent = '';
    },
    
    async sendReply() {
      if (!this.replyContent.trim() || !this.selectedMessage) return;
      
      try {
        await axiosInstance.post('/contact/send', {
          receiverId: this.selectedMessage.senderId,
          subject: this.replySubject,
          content: this.replyContent,
          relatedOfferId: this.selectedMessage.relatedOfferId
        });
        
        useToast().success('Antwort gesendet!');
        this.closeReplyModal();
        this.closeMessageModal();
        this.loadMessages();
      } catch (error) {
        console.error('Fehler beim Senden:', error);
        useToast().error('Antwort konnte nicht gesendet werden');
      }
    },

    async loadMessages() {
      this.loading = true;
      try {
        const endpoint = this.activeTab === 'inbox' ? 'inbox' : 'sent';
        const response = await axiosInstance.get(`/contact/${endpoint}`, {
          params: {
            page: this.currentPage,
            pageSize: 20
          }
        });
        
        this.messages = response.data.messages;
        this.totalCount = response.data.totalCount;
        this.pageCount = response.data.pageCount;
      } catch (error) {
        console.error('Fehler beim Laden der Nachrichten:', error);
        useToast().error('Nachrichten konnten nicht geladen werden');
      } finally {
        this.loading = false;
      }
    },
    
    async loadUnreadCount() {
      try {
        const response = await axiosInstance.get('/contact/unread-count');
        this.unreadCount = response.data.count;
      } catch (error) {
        console.error('Fehler beim Laden der ungelesenen Nachrichten:', error);
      }
    },
    
    async openMessage(message) {
      this.selectedMessage = message;
      
      // Mark as read if in inbox and unread
      if (this.activeTab === 'inbox' && !message.isRead) {
        try {
          await axiosInstance.put(`/contact/${message.id}/mark-read`);
          message.isRead = true;
          this.unreadCount = Math.max(0, this.unreadCount - 1);
        } catch (error) {
          console.error('Fehler beim Markieren als gelesen:', error);
        }
      }
    },
    
    closeMessageModal() {
      this.selectedMessage = null;
    },
    
    async deleteMessage(messageId) {
      if (!confirm('Möchten Sie diese Nachricht wirklich löschen?')) {
        return;
      }
      
      try {
        await axiosInstance.delete(`/contact/${messageId}`);
        useToast().success('Nachricht gelöscht');
        this.loadMessages();
        this.loadUnreadCount();
      } catch (error) {
        console.error('Fehler beim Löschen:', error);
        useToast().error('Nachricht konnte nicht gelöscht werden');
      }
    },
    
    changePage(page) {
      this.currentPage = page;
      this.loadMessages();
    },
    
    getMessageName(message) {
      if (this.activeTab === 'inbox') {
        return message.senderName || 'Unbekannt';
      } else {
        return message.receiverName || 'Unbekannt';
      }
    },
    
    getMessageAvatar(message) {
      const avatar = this.activeTab === 'inbox' ? message.senderAvatar : message.receiverAvatar;
      return avatar ? `data:image/jpeg;base64,${avatar}` : null;
    },
    
    formatDate(dateString) {
      const date = new Date(dateString);
      const now = new Date();
      const diffMs = now - date;
      const diffMins = Math.floor(diffMs / 60000);
      const diffHours = Math.floor(diffMs / 3600000);
      const diffDays = Math.floor(diffMs / 86400000);
      
      if (diffMins < 1) return 'Gerade eben';
      if (diffMins < 60) return `Vor ${diffMins} Min.`;
      if (diffHours < 24) return `Vor ${diffHours} Std.`;
      if (diffDays < 7) return `Vor ${diffDays} Tag${diffDays > 1 ? 'en' : ''}`;
      
      return date.toLocaleDateString('de-DE', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric'
      });
    }
  }
};
</script>

<style scoped lang="scss">
.messages-container {
  min-height: 80vh;
  padding: 2rem 0;
}

.page-title {
  font-size: 2rem;
  font-weight: 600;
  margin-bottom: 2rem;
  display: flex;
  align-items: center;
  
  .badge {
    font-size: 0.875rem;
    padding: 0.25rem 0.5rem;
  }
}

.tabs {
  display: flex;
  gap: 0.5rem;
  border-bottom: 2px solid #e0e0e0;
  
  .tab-btn {
    padding: 0.75rem 1.5rem;
    border: none;
    background: transparent;
    cursor: pointer;
    font-weight: 500;
    color: #666;
    border-bottom: 3px solid transparent;
    transition: all 0.3s;
    
    &:hover {
      color: #007bff;
    }
    
    &.active {
      color: #007bff;
      border-bottom-color: #007bff;
    }
    
    .badge {
      font-size: 0.75rem;
      padding: 0.15rem 0.4rem;
    }
  }
}

.messages-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.message-card {
  display: flex;
  align-items: flex-start;
  gap: 1rem;
  padding: 1.5rem;
  background: white;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.2s;
  
  &:hover {
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    border-color: #007bff;
  }
  
  &.unread {
    background: #f0f7ff;
    border-left: 4px solid #007bff;
    
    .message-subject {
      font-weight: 700;
    }
  }
}

.message-avatar {
  flex-shrink: 0;
  width: 50px;
  height: 50px;
  border-radius: 50%;
  overflow: hidden;
  background: #e0e0e0;
  display: flex;
  align-items: center;
  justify-content: center;
  
  img {
    width: 100%;
    height: 100%;
    object-fit: cover;
  }
  
  .default-avatar {
    font-size: 1.5rem;
    color: #666;
  }
}

.message-content {
  flex: 1;
  min-width: 0;
}

.message-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.5rem;
  
  strong {
    font-size: 1.1rem;
  }
  
  .message-date {
    color: #666;
    font-size: 0.875rem;
  }
}

.message-subject {
  font-size: 1rem;
  font-weight: 600;
  margin-bottom: 0.25rem;
  color: #333;
}

.message-preview {
  color: #666;
  font-size: 0.9rem;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.message-offer-link {
  margin-top: 0.5rem;
  font-size: 0.875rem;
  color: #007bff;
}

.message-actions {
  display: flex;
  gap: 0.5rem;
  
  .btn-icon {
    padding: 0.5rem;
    border: none;
    background: transparent;
    cursor: pointer;
    color: #666;
    border-radius: 4px;
    transition: all 0.2s;
    
    &:hover {
      background: #f0f0f0;
      color: #dc3545;
    }
  }
}

.pagination-wrapper {
  display: flex;
  justify-content: center;
  align-items: center;
}

.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 9999;
}

.message-modal {
  background: white;
  border-radius: 8px;
  max-width: 600px;
  width: 90%;
  max-height: 80vh;
  overflow-y: auto;
  
  .modal-header {
    padding: 1.5rem;
    border-bottom: 1px solid #e0e0e0;
    display: flex;
    justify-content: space-between;
    align-items: center;
    
    h3 {
      margin: 0;
      font-size: 1.5rem;
    }
    
    .close-btn {
      border: none;
      background: transparent;
      font-size: 1.5rem;
      cursor: pointer;
      padding: 0.25rem;
      
      &:hover {
        color: #007bff;
      }
    }
  }
  
  .modal-body {
    padding: 1.5rem;
  }
  
  .message-detail-content {
    white-space: pre-wrap;
    line-height: 1.6;
    color: #333;
  }
  
  .modal-footer {
    padding: 1rem 1.5rem;
    border-top: 1px solid #e0e0e0;
    display: flex;
    justify-content: flex-end;
  }
}

.no-messages {
  padding: 4rem 2rem;
}
</style>
