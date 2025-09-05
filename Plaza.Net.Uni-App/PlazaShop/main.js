import App from './App'

// #ifndef VUE3
import Vue from 'vue'
import './uni.promisify.adaptor'
import uView from '@/uni_modules/uview-ui'

Vue.use(uView)

Vue.config.productionTip = false
App.mpType = 'app'
const app = new Vue({
  ...App
})
app.$mount()
// #endif

// #ifdef VUE3
import { createSSRApp } from 'vue'
import uView from '@/uni_modules/uview-ui'
export function createApp() {
  const app = createSSRApp(App)
  Vue.use(uView)
  return {
    app
  }
}
// #endif