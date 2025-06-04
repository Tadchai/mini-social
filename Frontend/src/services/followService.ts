import type { Follow } from "@/types/Follow";
import type { ApiResponse } from "@/types/Response";

const API_URL = import.meta.env.VITE_API_URL;

export async function GetFollow(): Promise<ApiResponse<Follow[]>> {
  const token = localStorage.getItem("token");
  if (!token) throw new Error("Authentication token not found.");

  const url = new URL(`${API_URL}/Follow/Get`);

  const controller = new AbortController();
  const timeout = setTimeout(() => controller.abort(), 10000);

  try {
    const response = await fetch(url.toString(), {
      method: "GET",
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json"
      },
      signal: controller.signal
    });

    clearTimeout(timeout);

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(`HTTP error ${response.status}: ${errorText}`);
    }

    return await response.json();
  } catch (error: any) {
    if (error.name === "AbortError") {
      throw new Error("Request timeout.");
    }
    throw error;
  }
}
