import type { Post } from "@/types/Post"
import type { ApiResponseBase, PagedResponse } from "@/types/Response"

const API_URL = import.meta.env.VITE_API_URL

export async function createPost(content: null | string, selectedImages: File[]): Promise<ApiResponseBase> {
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

export async function fetchFollowPosts(pageSize: number, lastCursor?: string): Promise<PagedResponse<Post[]>> {
  const token = localStorage.getItem('token')
  let url = `${API_URL}/Post/Follow?pageSize=${pageSize}`

  if (lastCursor) {
    url += `&lastCursor=${lastCursor}`
  }

  const response = await fetch(url, {
    headers: { Authorization: `Bearer ${token}` }
  })
  return await response.json()
}

export async function fetchMyPosts(pageSize: number, lastCursor?: string): Promise<PagedResponse<Post[]>> {
  const token = localStorage.getItem('token')
  let url = `${API_URL}/Post/My?pageSize=${pageSize}`

  if (lastCursor) {
    url += `&lastCursor=${lastCursor}`
  }

  const response = await fetch(url, {
    headers: { Authorization: `Bearer ${token}` }
  })
  return await response.json()
}
