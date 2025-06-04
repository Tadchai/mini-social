import { jwtDecode } from 'jwt-decode'
import type { ApiResponseBase, TokenResponse } from '../types/Response'

const API_URL = import.meta.env.VITE_API_URL

export function isTokenValid(token: string | null): boolean {
  if (!token) return false

  try {
    const decoded = jwtDecode<{ exp: number }>(token)
    return decoded.exp * 1000 > Date.now()
  } catch {
    return false
  }
}

export async function loginUser(username: string, password: string): Promise<TokenResponse> {
  const response = await fetch(`${API_URL}/Auth/Login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ username, password }),
  })
  return await response.json()
}

export async function registerUser(username: string, email: string, password: string, confirmPassword: string): Promise<ApiResponseBase> {
  const response = await fetch(`${API_URL}/Auth/Register`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ username, email, password, confirmPassword }),
  })
  return await response.json()
}
