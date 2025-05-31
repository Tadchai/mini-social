using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.ViewModels
{
    public enum HttpStatusCode
    {
        OK = 200,
        Created = 201,
        BadRequest = 400,
        Unauthorized = 401,
        Forbidden = 403,
        NotFound = 404,
        Conflict = 409,
        InternalServerError = 500
    }
    public class ApiResponse
    {
        public string Message { get; set; } = string.Empty;
        public HttpStatusCode StatusCode { get; set; }
    }
    public class ApiWithDataResponse<T> : ApiResponse
    {
        public T? Data { get; set; }
    }
    public class ApiWithIdResponse : ApiResponse
    {
        public int Id { get; set; }
    }
    public class ApiWithTokenResponse : ApiResponse
    {
        public string Token { get; set; } = string.Empty;
    }
    public class ApiWithPagedResponse<T> : ApiResponse
    {
        public List<T>? Data { get; set; }

        public LastCursor? LastCursor { get; set; }
        public bool HasNextPage { get; set; }
    }
    public class LastCursor
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}