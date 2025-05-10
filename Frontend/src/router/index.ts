import { createRouter, createWebHistory } from 'vue-router'

import MainLayout from '../components/MainLayout.vue'
import AuthLayout from '../components/AuthLayout.vue'
import Login from '../views/Login.vue'
import Home from '../views/Home.vue'
import About from '../views/About.vue'
import ChatRoom from '../views/ChatRoom.vue'
import Follow from '../views/Follow.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      component: MainLayout,
      meta: { requiresAuth: true },
      children: [
        { path: '', component: Home },
        { path: 'about', component: About },
        { path: 'chat', component: ChatRoom },
        { path: 'follow', component: Follow },
      ],
    },
    {
      path: '/login',
      component: AuthLayout,
      children: [{ path: '', component: Login }],
    },
  ],
})

router.beforeEach((to, from, next) => {
  const isLoggedIn = !!localStorage.getItem('token')
  if (to.meta.requiresAuth && !isLoggedIn) {
    next('/login')
  } else if (to.path === '/login' && isLoggedIn) {
    next('/')
  } else {
    next()
  }
})

export default router
