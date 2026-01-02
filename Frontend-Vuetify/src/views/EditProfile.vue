<template>
  <Navbar />
  <div class="inner_banner_layout">
    <div class="container">
      <div class="row">
        <div class="col-sm-12">
          <div class="inner_banner">
            <h2>Profil bearbeiten</h2>
          </div>
        </div>
      </div>
    </div>
  </div>
  <div class="container mt-4">
    <div class="col-md-7 mx-auto">
      <div class="card">
        <form @submit.prevent="saveProfile" class="profile-form">
          <div class="row">            
            <div class="col-sm-12">
              <div class="form-group">
                <label>Fertigkeiten </label>
                <HierarchicalSkillSelect
                  v-model="profile.skills"
                  :skills="skillOptions"
                  label="Fertigkeiten"
                  placeholder="Fertigkeiten auswählen..."
                />
                <small v-if="errors.skills" style="color: red;">{{ errors.skills }}</small>
              </div>
            </div>
            <input type="hidden" v-model="profile.displayName" />
            <input type="hidden" v-model="profile.latitude" />
            <input type="hidden" v-model="profile.longitude" />
            <input type="hidden" v-model="profile.id" />
            <div class="col-sm-12">
              <div class="form-group">
                <label>Hobbys</label>
                <div class="hobbies_inputBox input-group mb-3">
                  <input type="text" v-model="newHobby" class="form-control" placeholder="Hobby eingeben"
                    @keyup.enter="addHobby">
                  <div class="input-group-append">
                    <button class="btn btn-outline-secondary" type="button" @click="addHobby">Hobby hinzufügen</button>
                  </div>
                </div>
                <div v-if="profile.hobbies.length > 0" class="mt-2">
                  <span v-for="(hobby, index) in profile.hobbies" :key="index" class="badge badge-primary mr-2 mb-2">
                    {{ hobby }}
                    <button type="button" class="close ml-1" @click="removeHobby(index)">&times;</button>
                  </span>
                </div>
                <span v-if="errors.hobbies" class="text-danger">{{ errors.hobbies }}</span>
              </div>
            </div>
          </div>

          <div class="row">
            <div class="col-sm-12">
              <div class="profile_btn">
                <!-- Submit Button -->
                <button type="button" @click="back()" class="btn btn-back rounded">Zurück</button>
                <button type="submit" class="btn-primary-ugh">Profil speichern</button>
              </div>
            </div>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'

import Navbar from '@/components/navbar/Navbar.vue'
import Securitybot from '@/services/SecurityBot'
import axiosInstance from '@/interceptor/interceptor'
import HierarchicalSkillSelect from '@/components/form/HierarchicalSkillSelect.vue'
import toast from '@/components/toaster/toast'

const router = useRouter()

const profile = reactive({
  skills: [],
  hobbies: [],
})

const skills = ref([])
const skillOptions = ref([])
const newHobby = ref('')
const errors = reactive({})
const formIsValid = ref(true)

function back() {
  globalThis.history.back()
}

function addHobby() {
  const v = newHobby.value.trim()
  if (!v) return
  if (profile.hobbies.some(h => h.toLowerCase() === v.toLowerCase())) {
    return
  }
  profile.hobbies.push(v)
  newHobby.value = ''
}


function removeHobby(index) {
  profile.hobbies.splice(index, 1)
}

async function fetchSkills() {
  try {
    const response = await axiosInstance.get(`skills/get-all-skills`)
    skills.value = response.data
    skillOptions.value = skills.value.map(skill => ({
      id: skill.skill_ID,
      name: skill.skillDescrition,
      parentId: skill.parentSkill_ID
    }))
  } catch (error) {
    console.error('Fehler beim Laden der Skills:', error)
  }
}

async function fetchUserProfile() {
  try {
    const response = await axiosInstance.get(`profile/get-user-profile`)
    if (response.data.profile) {
      // WICHTIG: reactive object nicht ersetzen, sondern patchen
      Object.assign(profile, response.data.profile)

      profile.skills = Array.isArray(profile.skills)
        ? profile.skills.filter(skill => skill && skill.id && skill.name)
        : []

      if (typeof profile.hobbies === 'string') {
        profile.hobbies = profile.hobbies.split(', ')
      } else if (!Array.isArray(profile.hobbies)) {
        profile.hobbies = []
      }

      await fetchSkills()
    } else {
      toast.info("Benutzerprofildaten nicht gefunden")
    }
  } catch (error) {
    console.error('Fehler beim Laden des Benutzerprofils:', error)
    toast.info("Fehler beim Laden des Benutzerprofils!")
  }
}

function validateForm() {
  // reactive errors leeren
  Object.keys(errors).forEach(k => delete errors[k])
  formIsValid.value = true

  const skillsCount = Array.isArray(profile.skills)
    ? profile.skills.filter(s => (typeof s === 'object' ? s?.id : String(s).trim())).length
    : 0

  const hobbiesCount = Array.isArray(profile.hobbies)
    ? profile.hobbies.map(h => String(h).trim()).filter(Boolean).length
    : 0

  if (skillsCount === 0 && hobbiesCount === 0) {
    errors.skills = "Bitte gib mindestens ein Skill oder ein Hobby an"
    errors.hobbies = "Bitte gib mindestens ein Skill oder ein Hobby an"
    formIsValid.value = false
  }

  return formIsValid.value
}

function saveProfile() {
  if (!validateForm()) return

  const updateRequest = {
    Skills: Array.isArray(profile.skills)
      ? profile.skills.map(skill => skill.id || skill).filter(s => s && s !== '').join(', ')
      : profile.skills,
    Hobbies: Array.isArray(profile.hobbies)
      ? profile.hobbies.join(', ')
      : profile.hobbies
  }

  updateProfileAPI(updateRequest)
}

async function updateProfileAPI(updatedProfile) {
  try {
    const response = await axiosInstance.put(`profile/update-profile`, updatedProfile)
    if (response.status === 200) {
      toast.success("Profil erfolgreich gespeichert!")

      if (response.data && response.data.profile) {
        const filteredSkills = Array.isArray(profile.skills)
          ? profile.skills.filter(skill => skill && skill.id && skill.name)
          : []

        Object.assign(profile, response.data.profile)
        profile.skills = filteredSkills
      }

      router.push('/profile')
    } else {
      toast.error("Fehler beim Speichern des Profils. Bitte versuchen Sie es erneut.")
    }
  } catch (error) {
    console.error('Fehler beim Speichern des Profils:', error)
    toast.error("Profil konnte nicht gespeichert werden. Bitte versuchen Sie es erneut.")
  }
}

onMounted(() => {
  Securitybot()
  fetchUserProfile()
})
</script>


<style scoped>
.v-container {
  display: flex;
  justify-content: center;
  align-items: center;
  padding: 20px;
}

.account-page {
  background-color: #f8f9fa;
  padding: 30px;
  border-radius: 8px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  max-width: 600px;
  width: 100%;
}

.card {
  background-color: #fff;
  border: none;
  border-radius: 8px;
  overflow: hidden;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
  width: 100%;
}

.card-body {
  padding: 20px;
}

.card-title {
  margin-bottom: 20px;
  font-size: 24px;
  font-weight: bold;
  color: #333;
  text-align: center;
}

.card-text-group {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.card-text {
  font-size: 16px;
  color: #555;
}

.card-text strong {
  color: #333;
}

.card-text a {
  color: #007bff;
  text-decoration: none;
}

.card-text a:hover {
  text-decoration: underline;
}

.btn-primary {
  color: #fff !important;
}

/* .btn-primary {
  margin-top: 20px;
  display: block;
  width: 100%;
  text-align: center;
} */

.profile-form-inner {
  display: flex;
  flex-wrap: wrap;
  gap: 1%;
}

.profile-form-inner .form-group {
  width: 19.2%;
}

.form-group {
  margin-bottom: 15px;
}

.text-danger {
  color: red;
}

.badge {
  font-size: 0.9em;
  padding: 0.5em 0.7em;
}

.badge .close {
  font-size: 1em;
  line-height: 0.8;
}

.btn.btn-back {
  background: #4e4e4e;
  background-color: #585757;
  color: #fff;
}

.btn.btn-back:hover {
  background: #4e4e4e;
  color: #fff;
}



.badge.badge-primary {
  background: #f0f6fd !important;
  font-size: 11px;
  font-weight: 400;
  color: #000;
  border: 1px solid rgb(0 0 0 / 12%) !important;
  padding: 5px 7px 5px 7px;
  border-radius: 5px;
}

.hobbies_inputBox .input-group-append .btn {
  font-size: 14px;
  border: 1px solid #c4cbdd;
  background: #d9d9d9;
  border-left: 2px solid #c4cbdd;
  color: #333;
  font-weight: 500;
}
</style>
