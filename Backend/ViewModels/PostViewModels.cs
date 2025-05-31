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
        public string Content { get; set; } = string.Empty;
    }
    public class DeletePostRequest
    {
        public int PostId { get; set; }
    }
    public class PostResponse
    {
        public int Id { get; set; }
        public string? Content { get; set; }
        public List<ImageResponse>? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class ImageResponse
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}