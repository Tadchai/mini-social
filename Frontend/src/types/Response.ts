export interface ApiResponse {
  statusCode: number
  message: string
}

export interface ApiLoginResponse extends ApiResponse {
  token: string
}

export interface ApiWithIdResponse extends ApiResponse {
  id: number
}
