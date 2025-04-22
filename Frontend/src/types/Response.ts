export interface ApiResponse<T> {
  data? :T
  statusCode: number
  message: string
}

export interface ApiLoginResponse {
  token: string;
}

export interface ApiWithIdResponse<T> extends ApiResponse<T> {
  id: number;
}
