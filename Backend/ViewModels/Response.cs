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
    public class ApiResponse<T> : ApiResponse
    {
        public T? Data { get; set; }
    }
    public class IdResponse : ApiResponse<int> { }
    public class TokenResponse : ApiResponse<string> { }
    public class PagedResponse<T> : ApiResponse<List<T>>
    {
        public string? LastCursor { get; set; }
        public bool HasNextPage { get; set; }
    }
}