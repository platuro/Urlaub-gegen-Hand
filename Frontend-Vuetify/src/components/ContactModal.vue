<template>
  <v-dialog :model-value="show" @update:model-value="updateShow" max-width="600" persistent>
    <v-card>
      <v-card-title class="headline primary white--text">
        Nachricht senden
        <v-spacer></v-spacer>
        <v-btn icon dark @click="close">
          <v-icon>mdi-close</v-icon>
        </v-btn>
      </v-card-title>

      <v-card-text class="pt-4">
        <div class="d-flex align-center mb-4">
          <v-avatar size="50" class="mr-3">
            <v-img :src="receiverAvatar || '/defaultprofile.jpg'"></v-img>
          </v-avatar>
          <div>
            <div class="text-subtitle-1 font-weight-bold">{{ receiverName }}</div>
          </div>
        </div>

        <v-divider class="mb-4"></v-divider>

        <v-text-field
          v-model="subject"
          label="Betreff"
          outlined
          dense
        ></v-text-field>

        <v-textarea
          v-model="message"
          label="Ihre Nachricht"
          outlined
          rows="8"
          counter="2000"
          :rules="[rules.required, rules.maxLength]"
        ></v-textarea>

        <v-alert v-if="error" type="error" dense class="mt-2">
          {{ error }}
        </v-alert>

        <v-alert v-if="success" type="success" dense class="mt-2">
          {{ success }}
        </v-alert>
      </v-card-text>

      <v-card-actions class="px-6 pb-4">
        <v-spacer></v-spacer>
        <v-btn text @click="close" :disabled="sending">
          Abbrechen
        </v-btn>
        <v-btn
          color="primary"
          @click="sendMessage"
          :loading="sending"
          :disabled="!isValid || sending"
        >
          <v-icon left>mdi-send</v-icon>
          Senden
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script>
import axiosInstance from '@/interceptor/interceptor';

export default {
  name: 'ContactModal',
  
  props: {
    show: {
      type: Boolean,
      default: false
    },
    receiverId: {
      type: String,
      required: true
    },
    receiverName: {
      type: String,
      required: true
    },
    receiverAvatar: {
      type: String,
      default: null
    },
    offerId: {
      type: Number,
      default: null
    }
  },

  emits: ['update:show', 'sent'],

  data() {
    return {
      subject: '',
      message: '',
      sending: false,
      error: null,
      success: null,
      rules: {
        required: value => !!value || 'Dieses Feld ist erforderlich',
        maxLength: value => !value || value.length <= 2000 || 'Maximal 2000 Zeichen'
      }
    };
  },

  computed: {
    isValid() {
      return this.message && this.message.length > 0 && this.message.length <= 2000;
    }
  },

  watch: {
    show(val) {
      console.log('ContactModal show changed:', val);
      if (val) {
        this.resetForm();
        if (this.offerId) {
          this.subject = `Anfrage zu Angebot #${this.offerId}`;
        }
      }
    }
  },

  methods: {
    updateShow(value) {
      console.log('updateShow called:', value);
      this.$emit('update:show', value);
    },

    async sendMessage() {
      if (!this.isValid) return;

      this.sending = true;
      this.error = null;
      this.success = null;

      try {
        await axiosInstance.post('/contact/send', {
          receiverId: this.receiverId,
          subject: this.subject || 'Neue Nachricht',
          content: this.message,
          relatedOfferId: this.offerId || null
        });

        this.success = 'Nachricht erfolgreich gesendet!';
        
        setTimeout(() => {
          this.close();
          this.$emit('sent');
        }, 1500);
        
      } catch (error) {
        console.error('Fehler beim Senden:', error);
        
        if (error.response && error.response.data) {
          this.error = error.response.data.message || error.response.data;
        } else {
          this.error = 'Nachricht konnte nicht gesendet werden.';
        }
      } finally {
        this.sending = false;
      }
    },

    close() {
      console.log('Closing modal');
      this.$emit('update:show', false);
      setTimeout(() => {
        this.resetForm();
      }, 300);
    },

    resetForm() {
      this.subject = '';
      this.message = '';
      this.error = null;
      this.success = null;
    }
  }
};
</script>
