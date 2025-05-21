export interface LastCursor{
  createdAt: string
  id: number
}

export interface ApiResponse {
  statusCode: number
  message: string
}

export interface ApiWithDataResponse<T> extends ApiResponse{
  data :T
}

export interface ApiWithTokenResponse extends ApiResponse {
  token: string;
}

export interface ApiWithIdResponse extends ApiResponse {
  id: number;
}

export interface ApiWithPagedResponse<T> extends ApiWithDataResponse<T> {
  lastCursor: LastCursor;
  hasNextPage: boolean;
}
