export interface ApiResponseBase {
  statusCode: number
  message: string
}

export interface ApiResponse<T> extends ApiResponseBase {
  data: T | null
}

export interface TokenResponse extends ApiResponse<string> {}

export interface IdResponse extends ApiResponse<number> {}

export interface PagedResponse<T> extends ApiResponse<T> {
  lastCursor: string
  hasNextPage: boolean
}
