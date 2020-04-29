import Vue from 'vue'
import Router from 'vue-router'
import Home from './views/Home.vue'
import QuestionPage from './views/question.vue'

Vue.use(Router)

export default new Router({
  routes: [
    {
      path: '/',
      name: 'home',
      component: Home
    },
    {
      path: '/question/:id',
      name: 'Question',
      component: QuestionPage
    }
  ]
})
