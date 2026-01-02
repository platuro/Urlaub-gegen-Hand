<template>
  <PublicNav />
  <div class="login-page">
    <div class="loginmain-bg">
      <div class="loginmain-form">
        <!-- Left Side - Information Panel -->
        <div class="login-left">
          <div class="login-left-content">
            <div class="login-main-text">
              <h2><span class="text-primary-blue">Willkommen bei Urlaub gegen Hand</span></h2>

              <p>Durch Ihre Registrierung öffnen Sie die Tür zu einzigartigen Reiseerfahrungen und kulturellem Austausch. Bevor Sie Teil unserer Community werden, benötigen wir Ihre Zustimmung:</p>
              
              <ul style="display: inline-block; text-align: left;">
                <li>Wir erfassen einige persönliche Daten, um Ihr Konto einzurichten und Ihnen die besten Möglichkeiten für Ihren Urlaub gegen Hand zu bieten.</li>
                <li>Ihre Daten werden vertraulich behandelt und nur für plattformbezogene Zwecke verwendet.</li>
                <li>Mit der Registrierung stimmen Sie unseren Nutzungsbedingungen und der Datenschutzerklärung zu.</li>
                <li>Sie erklären sich bereit, respektvoll mit Gastgebern und anderen Reisenden zu interagieren.</li>
              </ul>
              
              <p>Durch Klicken auf "Registrieren" bestätigen Sie, dass Sie diese Bedingungen akzeptieren.</p>
            </div>
            
            <div class="login-bottom-text">
              Entdecken Sie die Welt, teilen Sie Fähigkeiten, schaffen Sie unvergessliche Erlebnisse!
            </div>
          </div>
        </div>

        <!-- Right Side - Registration Form -->
        <div class="login-right">
          <div class="login-right-content-heading form-act">
            <div class="register-wrapper">
              <div class="login-form-section auth-card auth-register" id="login-content">
                <div class="login-heading">
                  <h2>Registrierung</h2>
                </div>

                <form class="form-border" @submit.prevent="register">
                  <div class="register-form-fields" :class="{ 'register-form-fields-mobile': isMobile }">
                    
                    <!-- Personal Information Section -->
                    <div class="form-section-divider">
                      <h5 class="heading5">Persönliche Daten</h5>
                    </div>

                    <!-- First Name -->
                    <div class="custom-form" :class="{ 'has-error': !firstName && showError }">
                      <label for="firstName">Vorname <span class="text-error-red">*</span></label>
                      <input 
                        type="text" 
                        class="form-control"
                        placeholder="Geben Sie Ihren Vornamen ein" 
                        id="firstName"
                        v-model="firstName" 
                      />
                      <span v-if="!firstName && showError" class="error-message">
                        Vorname ist erforderlich
                      </span>
                    </div>

                    <!-- Last Name -->
                    <div class="custom-form" :class="{ 'has-error': !lastName && showError }">
                      <label for="lastName">Nachname <span class="text-error-red">*</span></label>
                      <input 
                        type="text" 
                        class="form-control"
                        placeholder="Geben Sie Ihren Nachnamen ein" 
                        id="lastName" 
                        v-model="lastName" 
                      />
                      <span v-if="!lastName && showError" class="error-message">
                        Nachname ist erforderlich
                      </span>
                    </div>

                    <!-- Email -->
                    <div class="custom-form" :class="{ 'has-error': !email_Address && showError }">
                      <label for="email_Address">E-Mail-Adresse <span class="text-error-red">*</span></label>
                      <input 
                        type="email" 
                        class="form-control"
                        placeholder="beispiel@email.de" 
                        id="email"
                        v-model="email_Address" 
                      />
                      <span v-if="!email_Address && showError" class="error-message">
                        E-Mail-Adresse ist erforderlich
                      </span>
                    </div>

                    <!-- Gender -->
                    <div class="custom-form" :class="{ 'has-error': !gender && showError }">
                      <label for="gender">Geschlecht <span class="text-error-red">*</span></label>
                      <select class="form-control" id="gender" v-model="gender">
                        <option disabled value="">Bitte wählen Sie Ihr Geschlecht</option>
                        <option value="Male">Männlich</option>
                        <option value="Female">Weiblich</option>
                        <option value="Other">Möchte ich nicht bekannt geben</option>
                      </select>
                      <span v-if="!gender && showError" class="error-message">
                        Geschlecht ist erforderlich
                      </span>
                    </div>

                    <!-- Date of Birth -->
                    <div class="custom-form" :class="{ 'has-error': !dateOfBirth && showError }">
                      <label for="dateOfBirth">Geburtsdatum <span class="text-error-red">*</span></label>
                      <Datepicker 
                        date 
                        v-model="dateOfBirth" 
                        :enable-time-picker="false"
                        :auto-apply="true" 
                        placeholder="dd.mm.yyyy" 
                        :typeable="true" 
                        input-class-name="datepicker-input" 
                        startingView='year'  
                        inputFormat="dd.MM.yyyy"
                      />
                      <span v-if="!isValidDateOfBirth && showError" class="error-message">
                        Alter muss zwischen 14 und 120 Jahren liegen
                      </span>
                    </div>

                    <!-- Address Section -->
                    <div class="form-section-divider full-width">
                      <h5 class="heading5">Adresse</h5>
                    </div>

                    <!-- Address Map Picker -->
                    <div class="custom-form address-map-section full-width" :class="{ 'has-error': !selectedAddress && showError }">
                      <label>Adresse auswählen <span class="text-error-red">*</span></label>
                      <AddressMapPicker 
                        @address-selected="onAddressSelected"
                        :required="true"
                      />
                      <span v-if="!selectedAddress && showError" class="error-message">
                        Bitte wählen Sie eine Adresse aus
                      </span>
                    </div>

                    <!-- Address Details Display (Read-only) -->
                    <div v-if="selectedAddress" class="address-details-container full-width">
                      <div class="address-details-grid">
                        <!-- Street -->
                        <div class="custom-form">
                          <label>Straße</label>
                          <input 
                            type="text" 
                            class="form-control" 
                            :value="selectedAddress.road || 'Nicht verfügbar'" 
                            readonly
                          />
                        </div>

                        <!-- House Number -->
                        <div class="custom-form" :class="{ 'has-error': !houseNumber && showError }">
                          <label for="houseNumber">Hausnummer <span class="text-error-red">*</span></label>
                          <input 
                            type="text" 
                            class="form-control"
                            id="houseNumber"
                            placeholder="z.B. 42 oder 42a"
                            v-model="houseNumber"
                          />
                          <span v-if="!houseNumber && showError" class="error-message">
                            Hausnummer ist erforderlich
                          </span>
                        </div>

                        <!-- Postcode -->
                        <div class="custom-form">
                          <label>Postleitzahl</label>
                          <input 
                            type="text" 
                            class="form-control" 
                            :value="selectedAddress.postcode || 'Nicht verfügbar'" 
                            readonly
                          />
                        </div>

                        <!-- City -->
                        <div class="custom-form">
                          <label>Stadt</label>
                          <input 
                            type="text" 
                            class="form-control" 
                            :value="selectedAddress.city || 'Nicht verfügbar'" 
                            readonly
                          />
                        </div>

                        <!-- Country -->
                        <div class="custom-form">
                          <label>Land</label>
                          <input 
                            type="text" 
                            class="form-control" 
                            :value="selectedAddress.country || 'Nicht verfügbar'" 
                            readonly
                          />
                        </div>
                      </div>
                    </div>

                    <!-- Social Media Section -->
                    <div class="form-section-divider full-width">
                      <h5 class="heading5">Soziale Medien (Optional)</h5>
                    </div>

                    <!-- Facebook Link -->
                    <div class="custom-form full-width" :class="{ 'has-error': !isValidFacebookLink && showError }">
                      <label for="facebook_link">
                        <i class="ri-facebook-circle-fill" style="color: #1877f2;"></i> Facebook-Profillink
                      </label>
                      <input 
                        type="text" 
                        class="form-control"
                        placeholder="https://www.facebook.com/ihr.profil" 
                        id="facebook_link"
                        v-model="facebook_link"
                      >
                      <span v-if="!isFacebookLinkValid && showError" class="error-message">
                        Ungültiger Facebook-Link. Muss mit https://www.facebook.com/ beginnen
                      </span>
                    </div>

                    <!-- Security Section -->
                    <div class="form-section-divider full-width">
                      <h5 class="heading5">Sicherheit</h5>
                    </div>

                    <!-- Password -->
                    <div class="custom-form" :class="{ 'has-error': (!password && showError) || !isPasswordValid }">
                      <label for="password">Passwort <span class="text-error-red">*</span></label>
                      <div class="password-container" style="position: relative;">
                        <input 
                          :type="showPassword ? 'text' : 'password'" 
                          class="form-control"
                          placeholder="Geben Sie Ihr Passwort ein"
                          id="password" 
                          v-model="password" 
                          @input="validatePassword"
                        >
                        <i 
                          @click="togglePasswordVisibility" 
                          :class="showPassword ? 'ri-eye-off-fill' : 'ri-eye-fill'"
                          style="position: absolute; right: 10px; top: 10px; cursor: pointer; color: var(--text-light);"
                        ></i>
                      </div>
                      <span v-if="(!password && showError) || !isPasswordValid" class="error-message">
                        Passwort muss mindestens einen Großbuchstaben, eine Zahl, ein
                        Sonderzeichen <strong>!@#$%^&*</strong> enthalten und zwischen 8 und 20 Zeichen lang sein.
                      </span>
                    </div>

                    <!-- Confirm Password -->
                    <div class="custom-form" :class="{ 'has-error': !confirmPassword && showError }">
                      <label for="confirmPassword">Passwort bestätigen <span class="text-error-red">*</span></label>
                      <div class="password-container" style="position: relative;">
                        <input 
                          :type="showConfirmPassword ? 'text' : 'password'" 
                          class="form-control"
                          placeholder="Bestätigen Sie Ihr Passwort"
                          id="confirmPassword" 
                          v-model="confirmPassword"
                        >
                        <i 
                          @click="toggleConfirmPasswordVisibility"
                          :class="showConfirmPassword ? 'ri-eye-off-fill' : 'ri-eye-fill'"
                          style="position: absolute; right: 10px; top: 10px; cursor: pointer; color: var(--text-light);"
                        ></i>
                      </div>
                      <span v-if="(!confirmPassword && showError) || (password !== confirmPassword && showError)" class="error-message">
                        Passwortbestätigung ist erforderlich und muss mit dem Passwort übereinstimmen.
                      </span>
                    </div>

                  </div>

                  <!-- Submit Button -->
                  <div class="login-buttons">
                    <button type="submit" class="btn btn-primary-ugh">
                      <i class="ri-user-add-line"></i> Registrieren
                    </button>
                  </div>

                  <!-- Error Message -->
                  <div v-if="errorMessage" class="error-message" style="text-align: center; margin-top: 15px;">
                    {{ errorMessage }}
                  </div>

                  <!-- Back to Login -->
                  <div class="back-login">
                    Haben Sie bereits ein Konto? 
                    <router-link to="/login" class="action-link">Jetzt anmelden</router-link>
                  </div>
                </form>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import router from '@/router';
import Swal from 'sweetalert2';
import AES from 'crypto-js/aes';
import Datepicker from 'vue3-datepicker';
import axiosInstance from '@/interceptor/interceptor';
import toast from '../toaster/toast';
import AddressMapPicker from '@/components/common/AddressMapPicker.vue';
import PublicNav from '@/components/navbar/PublicNav.vue';

export default {
  components: {
    Datepicker,
    PublicNav,
    AddressMapPicker
  },
  data() {
    return {
      showPassword: false,
      showConfirmPassword: false,
      email_Address: '',
      password: '',
      firstName: '',
      lastName: '',
      gender: '',
      dateOfBirth: null,
      selectedAddress: null,
      houseNumber: '', // New field
      facebook_link: '',
      confirmPassword: '',
      errorMessage: '',
      link_RS: '',
      link_VS: '',
      showError: false,
      isPasswordValid: true,
      isFacebookLinkValid: true,
    };
  },

  computed: {
    isMobile() {
      return window.innerWidth <= 768;
    },
    isValidFacebookLink() {
      if (!this.facebook_link) {
        return true;
      }
      return this.facebook_link.startsWith('https://www.facebook.com/') || 
             this.facebook_link.startsWith('https://facebook.com/') ||
             this.facebook_link.startsWith('http://www.facebook.com/') ||
             this.facebook_link.startsWith('http://facebook.com/');
    },
    isValidDateOfBirth() {
      if (!this.dateOfBirth) {
        return false;
      }

      const birthDate = new Date(this.dateOfBirth);
      const today = new Date();
      let age = today.getFullYear() - birthDate.getFullYear();
      const monthDifference = today.getMonth() - birthDate.getMonth();
      const dayDifference = today.getDate() - birthDate.getDate();

      if (monthDifference < 0 || (monthDifference === 0 && dayDifference < 0)) {
        age--;
      }

      return age >= 14 && age <= 120;
    }
  },

  methods: {
    onAddressSelected(address) {
      this.selectedAddress = address;
      // Pre-fill house number if available
      if (address && address.houseNumber) {
        this.houseNumber = address.houseNumber;
      }
      if (address) {
        this.showError = false;
      }
    },
    
    togglePasswordVisibility() {
      this.showPassword = !this.showPassword;
    },

    toggleConfirmPasswordVisibility() {
      this.showConfirmPassword = !this.showConfirmPassword;
    },

    validatePassword() {
      const passwordRegex = /^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*])[A-Za-z\d!@#$%^&*]{8,20}$/;
      this.isPasswordValid = passwordRegex.test(this.password);
    },

    // Method to handle the registration process
    register() {
      this.showError = true;

      // Validate all required fields
      if (!this.firstName || !this.lastName || !this.email_Address || 
          !this.gender || !this.dateOfBirth || !this.password || 
          !this.confirmPassword || !this.selectedAddress || !this.houseNumber) {
        this.errorMessage = "Bitte füllen Sie alle Pflichtfelder aus.";
        toast.warning('Fehlende Informationen', 'Bitte füllen Sie alle Pflichtfelder aus.');
        return;
      }

      if (!this.isPasswordValid) {
        this.errorMessage = "Passwort erfüllt nicht die Anforderungen.";
        return;
      }

      // Check if passwords match
      if (this.password !== this.confirmPassword) {
        this.errorMessage = "Passwörter stimmen nicht überein.";
        return;
      }

      // Check Facebook link if provided
      if (this.facebook_link !== '') {
        if (!this.isValidFacebookLink) {
          this.errorMessage = "Ungültiger Facebook-Link. Bitte geben Sie einen gültigen Link ein.";
          return;
        }
      }

      // Check if date of birth is valid
      if (!this.isValidDateOfBirth) {
        this.errorMessage = "Das Geburtsdatum ist ungültig. Das Mindestalter beträgt 14 Jahre.";
        return;
      }

      // Show loader
      Swal.fire({
        title: 'Registrieren...',
        text: 'Bitte warten',
        allowOutsideClick: false,
        showConfirmButton: false,
        willOpen: () => {
          Swal.showLoading();
        }
      });

     // Creating registration data object with new address structure
      // Build displayName with house number included
      const displayNameWithHouseNumber = this.houseNumber 
        ? `${this.selectedAddress.road || ''} ${this.houseNumber}, ${this.selectedAddress.city || ''}, ${this.selectedAddress.postcode || ''}, ${this.selectedAddress.country || ''}`
        : this.selectedAddress.displayName;

      const registrationData = {
        email_Address: this.email_Address,
        password: this.password,
        firstName: this.firstName,
        lastName: this.lastName,
        gender: this.gender,
        dateOfBirth: this.dateOfBirth,
        // Address data on root level
        latitude: this.selectedAddress.latitude,
        longitude: this.selectedAddress.longitude,
        displayName: displayNameWithHouseNumber.trim(),
        houseNumber: this.houseNumber, // Using the manual input
        road: this.selectedAddress.road,
        city: this.selectedAddress.city,
        postcode: this.selectedAddress.postcode,
        country: this.selectedAddress.country,
        countryCode: this.selectedAddress.countryCode,
        facebook_link: this.facebook_link,
        link_RS: this.link_RS,
        link_VS: this.link_VS,
      };

      // Sending a POST request to the registration endpoint with the registration data
      axiosInstance.post(`authenticate/register`, registrationData)
        .then(response => {
          Swal.close();
          toast.success('Registrierung erfolgreich!', 'Überprüfen Sie Ihre E-Mail und klicken Sie auf „Bestätigen".');

          const reg_Id = response.data.value.userId;
          const encryptedLogId = this.encryptItem(reg_Id);
          sessionStorage.setItem('logId', encryptedLogId);
          router.push('/upload-id');
        })
        .catch(error => {
          Swal.close();
          if (error.response && error.response.data.error.message === "E-Mail Adresse existiert bereits") {
            toast.warning('Benutzer mit dieser E-Mail existiert bereits!', 'Versuchen Sie es mit einer anderen E-Mail-Adresse oder versuchen Sie sich anzumelden');
          } else {
            console.error('Registrierungsfehler:', error);
            this.errorMessage = 'Registrierung fehlgeschlagen. Bitte versuchen Sie es erneut.';
            toast.error('Registrierung fehlgeschlagen!', 'Es ist ein Fehler aufgetreten!');
          }
        });
    },

    // Method to encrypt a given item using AES encryption
    encryptItem(item) {
      return AES.encrypt(item, import.meta.env.VITE_SECRET_KEY || 'thisismytestsecretkey').toString();
    }
  },

  mounted() {
    Swal.fire({
      title: 'Zustimmung erforderlich',
      html: `
        <p>Ihre Privatsphäre ist uns wichtig. Um Ihr Konto zu erstellen, müssen wir einige Ihrer Daten erfassen. Mit dem Fortfahren stimmen Sie zu:</p>
        <ul style="display: inline-block; text-align: left; padding-left: 30px; margin: 15px 0;">
          <li>Der Erfassung und Nutzung Ihrer Daten für die Kontoerstellung</li>
          <li>Den wesentlichen Funktionen unserer Plattform</li>
          <li>Unseren Nutzungsbedingungen und <a href="https://alreco.de/datenschutzerklaerung" target="_blank">Datenschutzrichtlinien</a></li>
        </ul>
        <div style="margin-top: 20px; text-align: center;">
          <input type="checkbox" id="gdpr-consent-checkbox" style="margin-right: 8px;">
          <label for="gdpr-consent-checkbox" style="font-weight: 600;">Ich akzeptiere die Bedingungen</label>
        </div>
      `,
      showCancelButton: true,
      confirmButtonText: 'Akzeptieren',
      cancelButtonText: 'Abbrechen',
      allowOutsideClick: false,
      customClass: {
        popup: 'custom-consent-modal',
        confirmButton: 'btn-primary-ugh',
        cancelButton: 'btn-cancel'
      },
      didOpen: () => {
        const confirmButton = Swal.getConfirmButton();
        confirmButton.disabled = true;
        confirmButton.classList.add('disabled-confirm-button');
        const checkbox = document.getElementById('gdpr-consent-checkbox');
        checkbox.addEventListener('change', (event) => {
          confirmButton.disabled = !event.target.checked;
          if (confirmButton.disabled) {
            confirmButton.classList.add('disabled-confirm-button');
          } else {
            confirmButton.classList.remove('disabled-confirm-button');
          }
        });
      }
    }).then((result) => {
      if (!result.isConfirmed) {
        router.push({ name: 'Login' });
      }
    });
  }
}
</script>

<style scoped>
.v-container {
  display: none !important;
}

/* Form Section Dividers */
.form-section-divider {
  width: 100%;
  margin: 25px 0 15px 0;
  padding-bottom: 10px;
  border-bottom: 2px solid var(--primary-blue);
}

.form-section-divider h5 {
  margin: 0;
  color: var(--primary-blue);
  font-weight: 600;
}

/* Full Width Form Elements */
.full-width {
  width: 100% !important;
  grid-column: 1 / -1;
}

/* Address Details Container */
.address-details-container {
  margin: 20px 0;
  padding: 20px;
  background: var(--bg-light);
  border-radius: 10px;
  border: 1px solid #e0e0e0;
}

.address-details-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 15px;
}

/* Error Message Styling */
.error-message {
  color: var(--error-red);
  font-size: 12px;
  margin-top: 5px;
  display: block;
}

.text-error-red {
  color: var(--error-red);
}

/* Custom Form Styling */
.custom-form {
  margin-bottom: 20px;
}

.address-details-container .custom-form {
  width: 100%;
}

.custom-form label {
  display: block;
  margin-bottom: 6px;
  font-weight: 600;
  color: var(--text-dark);
  font-size: 14px;
}

.custom-form input,
.custom-form select {
  width: 100%;
}

.custom-form.has-error input,
.custom-form.has-error select {
  border-color: var(--error-red);
  background-color: #fff5f5;
}

/* Password Container */
.password-container {
  position: relative;
}

/* Login Buttons */
.login-buttons {
  margin-top: 30px;
  text-align: center;
}

.login-buttons .btn {
  width: 100%;
  max-width: 100%;
  padding: 14px 30px;
  font-size: 16px;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
}

.login-buttons .btn i {
  font-size: 20px;
}

/* Disabled Button State */
.disabled-confirm-button {
  opacity: 0.5;
  cursor: not-allowed !important;
}

/* Back to Login Link */
.back-login {
  text-align: center;
  margin-top: 20px;
  font-size: 14px;
}

/* Responsive Adjustments */
@media only screen and (max-width: 991px) {
  .register-form-fields {
    gap: 15px;
  }
  
  .address-details-grid {
    grid-template-columns: 1fr;
  }
}

@media only screen and (max-width: 575px) {
  .register-form-fields .custom-form {
    width: 100% !important;
  }
  
  .form-section-divider {
    margin: 20px 0 10px 0;
  }
  
  .address-details-container {
    padding: 15px;
  }
}

/* Login Heading Styling */
.login-heading h2 {
  color: var(--primary-blue);
  font-size: 28px;
  font-weight: 600;
  margin-bottom: 20px;
  text-align: center;
}

/* Social Media Icons */
.ri-facebook-circle-fill {
  font-size: 18px;
  vertical-align: middle;
}
</style>

<!-- Global styles for datepicker (not scoped) -->
<style>
/* Fix for vue3-datepicker overflow issues */
.auth-card,
.auth-register,
.login-form-section,
.register-wrapper,
.form-border {
  overflow: visible !important;
}

/* Ensure datepicker calendar appears above other elements */
.v3dp__datepicker {
  position: relative;
  z-index: 1000;
}

.v3dp__popout {
  z-index: 1001 !important;
  position: absolute !important;
}

/* Additional datepicker styling */
.datepicker-input {
  width: 100%;
  padding: 6px 12px;
  border: 1px solid #c4cbdd;
  border-radius: 4px;
  background: #f3f4f6;
  height: 36px;
  font-size: 14px;
  color: rgb(23, 26, 31);
}

.datepicker-input:focus {
  outline: none;
  border-color: var(--primary-blue);
  background: #fff;
}

/* Make sure the form doesn't clip the datepicker */
.login-right-content-heading {
  overflow: visible !important;
}

.loginmain-bg .login-right {
  overflow: visible !important;
}

/* Datepicker calendar styling */
.v3dp__popout {
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.2) !important;
  border-radius: 8px !important;
  border: 1px solid #e0e0e0 !important;
}

</style>