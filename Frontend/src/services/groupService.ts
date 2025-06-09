import type { MessagesResponse } from "@/types/Group";
import type { IdResponse, PagedResponse } from "@/types/Response";

const API_URL = import.meta.env.VITE_API_URL;

export async function GroupMessages(conversationId: number): Promise<PagedResponse<MessagesResponse[]>> {
  const token = localStorage.getItem('token');

  if (!token) {
    throw new Error('Authentication token not found.');
  }

  const url = new URL(`${API_URL}/Group/Messages`);
  url.searchParams.append('groupId', conversationId.toString());
  url.searchParams.append('pageSize', '20');

  const response = await fetch(url.toString(), {
    headers: {
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json',
      Accept: 'application/json',
    }
  });

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(`HTTP error ${response.status}: ${errorText}`);
  }

  return await response.json();
}

export async function CreatePrivate(targetUserId: number): Promise<IdResponse> {
  const token = localStorage.getItem('token');

  if (!token) {
    throw new Error('Authentication token not found.');
  }

  const url = new URL(`${API_URL}/Group/CreatePrivate`);

  const response = await fetch(url.toString(), {
    method: "POST",
    headers: {
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json',
      Accept: 'application/json',
    },
      body: JSON.stringify({ targetUserId }),
  });

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(`HTTP error ${response.status}: ${errorText}`);
  }

  return await response.json();
}
