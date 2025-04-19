using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Backend.Models;
using Backend.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize]
    public class PostController : ControllerBase
    {
        private readonly MiniSocialContext _context;
        private readonly IConfiguration _configuration;
        public PostController(MiniSocialContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int userId)
        {
            try
            {
                var queyPost = from u in _context.Users
                               join p in _context.Posts on u.Id equals p.UserId
                               where p.UserId == userId
                               select new 
                               {
                                   Id = p.Id,
                                   Content = p.Content,
                               };

                var post = await queyPost.ToListAsync();
                var postDic = post.ToDictionary(x => x.Id, x => new List<ImageResponse>());

                var queyPostImages = from p in _context.Posts
                                     join pi in _context.PostImages on p.Id equals pi.PostId
                                     where post.Select(x => x.Id).Contains(pi.Id)
                                     select new 
                                     {
                                         PostId = pi.PostId,
                                         Id = pi.Id,
                                         ImageUrl = pi.ImageUrl
                                     };
                foreach (var image in queyPostImages)
                {
                    postDic[image.PostId].Add(new ImageResponse
                    {
                        Id = image.Id,
                        ImageUrl = image.ImageUrl
                    });
                }
                var data = post.Select(p => new GetByIdResponse<ImageResponse>
                {
                    Id = p.Id,
                    Content = p.Content,
                    ImageUrl = postDic[p.Id].Select(p => new ImageResponse
                    {
                        Id = p.Id,
                        ImageUrl = p.ImageUrl
                    }).ToList()
                }).ToList();

                return new JsonResult(new ApiResponse<List<GetByIdResponse<ImageResponse>>> { Data = data, Message = "Posts retrieved successfully.", StatusCode = HttpStatusCode.OK });
            }
            catch (Exception ex)
            {

                return new JsonResult(new ApiResponse<object> { Message = $"An error occurred: {ex.Message}", StatusCode = HttpStatusCode.InternalServerError });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePostRequest request)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

            foreach (var image in request.Image)
            {
                var extension = Path.GetExtension(image.FileName).ToLower();
                if (!allowedExtensions.Contains(extension))
                {
                    return BadRequest("Unsupported file type.");
                }
            }
            var uploadsFolder = Path.Combine(_configuration["UploadSettings:ImageUploadPath"], "uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    var postModel = new Post
                    {
                        UserId = int.Parse(userId),
                        Content = request.Content,
                        CreateAt = DateTime.Now,
                    };
                    await _context.Posts.AddAsync(postModel);
                    await _context.SaveChangesAsync();

                    foreach (var image in request.Image)
                    {
                        var extension = Path.GetExtension(image.FileName).ToLower();
                        if (!allowedExtensions.Contains(extension))
                            continue;

                        var fileName = Guid.NewGuid().ToString() + extension;
                        var filePath = Path.Combine(uploadsFolder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";
                        var postImageModel = new PostImage
                        {
                            PostId = postModel.Id,
                            ImageUrl = fileUrl,
                            CreatedAt = DateTime.Now,
                        };
                        await _context.PostImages.AddAsync(postImageModel);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new JsonResult(new ApiResponse<object> { Message = "Post Created successfully.", StatusCode = HttpStatusCode.Created });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new JsonResult(new ApiResponse<object> { Message = $"An error occurred: {ex.Message}", StatusCode = HttpStatusCode.InternalServerError });
                }
            }
        }

    }
}