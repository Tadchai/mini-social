import { createRouter, createWebHistory } from 'vue-router'
import { isTokenValid } from '@/services/authService'
import AuthLayout from '@/components/layouts/AuthLayout.vue'
import LoginPage from '@/views/LoginPage.vue'
import HomePage from '@/views/HomePage.vue'
import AboutPage from '@/views/AboutPage.vue'
import ChatRoomPage from '@/views/ChatRoomPage.vue'
import FriendPage from '@/views/FriendPage.vue'
import MyPostPage from '@/views/MyPostPage.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    { path: '/', component: HomePage },
    { path: '/about', component: AboutPage },
    { path: '/chat/:id', component: ChatRoomPage },
    { path: '/friends', component: FriendPage },
    { path: '/myposts', component: MyPostPage},
    {
      path: '/login',
      component: AuthLayout,
      children: [{ path: '', component: LoginPage }],
    }
  ]
})

router.beforeEach((to, from, next) => {
  const token = localStorage.getItem('token')
  const isLoggedIn = isTokenValid(token)

  if (to.meta.requiresAuth && !isLoggedIn) {
    next('/login')
  } else if (to.path === '/login' && isLoggedIn) {
    next('/')
  } else {
    next()
  }
})

export default router
