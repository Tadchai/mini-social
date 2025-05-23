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

    public class LastCursor
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ApiWithDataResponse<T>
    {
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public HttpStatusCode StatusCode { get; set; }
    }
    public class ApiResponse
    {
        public string Message { get; set; } = string.Empty;
        public HttpStatusCode StatusCode { get; set; }
    }
    public class ApiWithIdResponse
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public HttpStatusCode StatusCode { get; set; }
    }
    public class ApiWithTokenResponse
    {
        public string Token { get; set; }
        public string Message { get; set; } = string.Empty;
        public HttpStatusCode StatusCode { get; set; }
    }
    public class ApiWithPagedResponse<T>
    {
        public List<T> Data { get; set; }

        public LastCursor? LastCursor { get; set; }
        public bool HasNextPage { get; set; }
        public string Message { get; set; } = string.Empty;
        public HttpStatusCode StatusCode { get; set; }
    }

    public class TokenResponse
    {
        public string Token { get; set; } = string.Empty;
    }


}