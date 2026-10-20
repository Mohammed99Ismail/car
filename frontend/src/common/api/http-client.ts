import type { AxiosInstance, AxiosRequestConfig } from 'axios'
import axios from 'axios'
import tokenService from './token.service'

let isRefreshing = false
let failedQueue: Array<{
  resolve: (value?: unknown) => void
  reject: (reason?: unknown) => void
}> = []

const processQueue = (error: unknown, token: string | null = null) => {
  failedQueue.forEach((prom) => {
    if (error) {
      prom.reject(error)
    } else {
      prom.resolve(token)
    }
  })
  failedQueue = []
}

function httpClient(baseApi: string | null = null): AxiosInstance {
  const baseURL = baseApi ?? (import.meta.env.VITE_API_URL || '/api')

  const client = axios.create({
    baseURL,
    headers: {
      'Content-Type': 'application/json',
    },
  })

  client.interceptors.request.use(
    (config: any) => {
      const token = tokenService.getLocalAccessToken()
      if (token && config.headers) {
        config.headers.Authorization = `Bearer ${token}`
      }
      try {
        const layoutStored = localStorage.getItem('layout')
        const activeLang = layoutStored ? JSON.parse(layoutStored)?.activeLanguage : null
        if (config.headers) {
          config.headers['Accept-Language'] = activeLang || 'ar'
        }
      } catch {
        if (config.headers) {
          config.headers['Accept-Language'] = 'ar'
        }
      }
      return config
    },
    (error) => Promise.reject(error),
  )

  client.interceptors.response.use(
    (response) => response,
    async (error) => {
      const originalRequest = error.config

      if (error.response?.status === 401 && !originalRequest._retry) {
        const refreshToken = tokenService.getLocalRefreshToken()
        const accessToken = tokenService.getLocalAccessToken()

        // If no refresh token exists or the request was already to auth endpoint, reject
        if (!refreshToken || originalRequest.url?.includes('auth/')) {
          tokenService.removeUser()
          return Promise.reject(error)
        }

        if (isRefreshing) {
          return new Promise((resolve, reject) => {
            failedQueue.push({ resolve, reject })
          })
            .then((token) => {
              originalRequest.headers.Authorization = `Bearer ${token}`
              return client(originalRequest)
            })
            .catch((err) => Promise.reject(err))
        }

        originalRequest._retry = true
        isRefreshing = true

        try {
          const cleanBase = (baseURL || '').replace(/\/+$/, '')
          const res = await axios.post(`${cleanBase}/auth/refresh-token`, {
            accessToken,
            refreshToken,
          })

          if (res.data?.data?.accessToken) {
            const newAccessToken = res.data.data.accessToken
            const newRefreshToken = res.data.data.refreshToken
            tokenService.updateTokens(newAccessToken, newRefreshToken)

            processQueue(null, newAccessToken)
            originalRequest.headers.Authorization = `Bearer ${newAccessToken}`
            return client(originalRequest)
          } else {
            processQueue(new Error('Refresh failed'), null)
            tokenService.removeUser()
            window.location.href = '/account/login'
            return Promise.reject(error)
          }
        } catch (refreshErr) {
          processQueue(refreshErr, null)
          tokenService.removeUser()
          window.location.href = '/account/login'
          return Promise.reject(refreshErr)
        } finally {
          isRefreshing = false
        }
      }

      // Display meaningful API errors
      const errMessage =
        error.response?.data?.message ||
        error.response?.data?.errors?.[0] ||
        error.message

      if (errMessage && (error.response?.status !== 401 || originalRequest?.url?.includes('auth/'))) {
        try {
          useNotifyStore().error(errMessage)
        } catch {
          // Store might not be mounted yet
        }
      }

      return Promise.reject(error)
    },
  )

  return client
}

export default httpClient
