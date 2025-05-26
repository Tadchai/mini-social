import type { Post } from "@/types/Post"
import type { ApiResponse, ApiWithPagedResponse, LastCursor } from "@/types/Response"

const API_URL = import.meta.env.VITE_API_URL

export async function createPost(content: null | string, selectedImages: File[]): Promise<ApiResponse> {
  const token = localStorage.getItem('token')
  const formData = new FormData()
    if (content !== null) {
      formData.append('content', content)
    }
    selectedImages.forEach((file) => {
      formData.append('Image', file)
    })

  const response = await fetch(`${API_URL}/Post/Create`, {
    method: 'POST',
    headers: { Authorization: `Bearer ${token}` },
      body: formData,
  })
  return await response.json()
}

export async function fetchFollowPosts( pageSize: number, lastCursor?: LastCursor): Promise<ApiWithPagedResponse<Post[]>> {
  const token = localStorage.getItem('token')
  let url = `${API_URL}/Post/GetByFollow?pageSize=${pageSize}`

  if (lastCursor) {
    url += `&lastCreatedAt=${encodeURIComponent(lastCursor.createdAt)}&lastId=${lastCursor.id}`
  }

  const response = await fetch(url, {
    headers: { Authorization: `Bearer ${token}` }
  })
  return await response.json()
}

export async function fetchMyPosts( pageSize: number, lastCursor?: LastCursor): Promise<ApiWithPagedResponse<Post[]>> {
  const token = localStorage.getItem('token')
  let url = `${API_URL}/Post/GetById?pageSize=${pageSize}`

  if (lastCursor) {
    url += `&lastCreatedAt=${encodeURIComponent(lastCursor.createdAt)}&lastId=${lastCursor.id}`
  }

  const response = await fetch(url, {
    headers: { Authorization: `Bearer ${token}` }
  })
  return await response.json()
}
