import type { PagedResponse } from "@/types/Response"
import type { UserResponse } from "@/types/User"

const API_URL = import.meta.env.VITE_API_URL

export async function fetchSearch(search: string, cursor?: string): Promise<PagedResponse<UserResponse[]>> {
  const url = new URL(`${API_URL}/Search/Users`);
  url.searchParams.append('q', search);
  url.searchParams.append('pageSize', '8');
  if (cursor != null) {
    url.searchParams.append('cursor', cursor);
  }

  const response = await fetch(url.toString(), {
    headers: {
      'Content-Type': 'application/json',
    }
  });

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(`HTTP error ${response.status}: ${errorText}`);
  }

  return await response.json();
}
