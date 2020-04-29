import Vue from 'vue'
import App from './App.vue'
import router from './router'
import axios from 'axios'
import QuestionHub from './question-hub'
import BootstrapVue from 'bootstrap-vue'
import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap-vue/dist/bootstrap-vue.css'
import '@fortawesome/fontawesome-free/css/all.css'

Vue.config.productionTip = false

Vue.prototype.$http = axios

Vue.use(BootstrapVue)
Vue.use(QuestionHub)

new Vue({
  router,
  render: h => h(App)
}).$mount('#app')
