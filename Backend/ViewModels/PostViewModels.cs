using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.ViewModels
{
    public class CreatePostRequest
    {
        public List<IFormFile>? Image { get; set; }
        public string? Content { get; set; }
    }

    public class UpdatePostRequest
    {
        public int PostId { get; set; }
        public string Content { get; set; }
    }

    public class DeletePostRequest
    {
        public int PostId { get; set; }
    }

    public class GetByIdResponse<T>
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public List<T> ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ImageResponse
    {
        public int Id { get; set;}
        public string ImageUrl { get; set; }
    }

    public class GetByIdRequest
    {
        public int UserId { get; set; }
        public DateTime? LastCreatedAt { get; set; }
        public int? LastId { get; set; }
        public int PageSize { get; set; }
    }
}