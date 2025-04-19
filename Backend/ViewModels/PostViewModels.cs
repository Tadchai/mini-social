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

    public class GetByIdResponse<T>
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public List<T> ImageUrl { get; set; }
    }

    public class ImageResponse
    {
        public int Id { get; set;}
        public string ImageUrl { get; set; }
    }
}