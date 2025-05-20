export interface ApiResponse<T> {
  data? :T
  statusCode: number
  message: string
}

export interface ApiLoginResponse {
  token: string;
  statusCode: number
  message: string
}

export interface ApiWithIdResponse<T> extends ApiResponse<T> {
  id: number;
}
