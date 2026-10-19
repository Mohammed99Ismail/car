class TokenService {
  getLocalRefreshToken(): string | null {
    try {
      const raw = localStorage.getItem('account')
      if (!raw) return null
      const account = JSON.parse(raw)
      return account.user?.refreshToken ?? null
    } catch {
      return null
    }
  }

  getLocalAccessToken(): string | null {
    try {
      const raw = localStorage.getItem('account')
      if (!raw) return null
      const account = JSON.parse(raw)
      return account.user?.token ?? null
    } catch {
      return null
    }
  }

  updateLocalAccessToken(token: string) {
    try {
      const raw = localStorage.getItem('account')
      const account = raw ? JSON.parse(raw) : {}
      if (account.user) {
        account.user.token = token
      } else {
        account.user = { token }
      }
      localStorage.setItem('account', JSON.stringify(account))
    } catch (e) {
      console.error('Failed to update access token', e)
    }
  }

  updateTokens(token: string, refreshToken: string) {
    try {
      const raw = localStorage.getItem('account')
      const account = raw ? JSON.parse(raw) : {}
      if (account.user) {
        account.user.token = token
        account.user.refreshToken = refreshToken
      } else {
        account.user = { token, refreshToken }
      }
      localStorage.setItem('account', JSON.stringify(account))
    } catch (e) {
      console.error('Failed to update tokens', e)
    }
  }

  getUser() {
    try {
      const raw = localStorage.getItem('account')
      if (!raw) return null
      const account = JSON.parse(raw)
      return account.user ?? null
    } catch {
      return null
    }
  }

  setUser(user: any) {
    try {
      const raw = localStorage.getItem('account')
      const account = raw ? JSON.parse(raw) : {}
      account.user = user
      localStorage.setItem('account', JSON.stringify(account))
    } catch (e) {
      console.error('Failed to set user', e)
    }
  }

  removeUser() {
    try {
      const raw = localStorage.getItem('account')
      const account = raw ? JSON.parse(raw) : {}
      account.user = null
      localStorage.setItem('account', JSON.stringify(account))
    } catch {
      localStorage.removeItem('account')
    }
  }
}

export default new TokenService()
