using Backend.Models;
using Backend.Services.Interfaces;
using Backend.ViewModels;
using Microsoft.EntityFrameworkCore;
using static Backend.Services.CursorService;

namespace Backend.Services
{
    public class PostService : IPostService
    {
        private readonly MiniSocialContext _context;
        private readonly IConfiguration _configuration;
        private readonly ICursorService _cursorService;
        public PostService(MiniSocialContext context, IConfiguration configuration, ICursorService cursorService)
        {
            _context = context;
            _configuration = configuration;
            _cursorService = cursorService;
        }
        public async Task<ApiResponse> CreatePostAsync(CreatePostRequest request, int userId, HttpRequest httpRequest)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            foreach (var image in request.Image)
            {
                if (!allowedExtensions.Contains(Path.GetExtension(image.FileName).ToLower()))
                    return new ApiResponse { Message = "Unsupported file type.", StatusCode = HttpStatusCode.BadRequest };
            }

            var uploadsFolder = Path.Combine(_configuration["UploadSettings:ImageUploadPath"], "uploads");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var postModel = new Post
                {
                    UserId = userId,
                    Content = request.Content,
                    CreatedAt = DateTime.Now
                };
                await _context.Posts.AddAsync(postModel);
                await _context.SaveChangesAsync();

                foreach (var image in request.Image)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);
                    using var stream = new FileStream(filePath, FileMode.Create);
                    await image.CopyToAsync(stream);

                    var fileUrl = $"{httpRequest.Scheme}://{httpRequest.Host}/uploads/{fileName}";
                    await _context.PostImages.AddAsync(new PostImage
                    {
                        PostId = postModel.Id,
                        ImageUrl = fileUrl,
                        CreatedAt = DateTime.Now
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ApiResponse { Message = "Post Created successfully.", StatusCode = HttpStatusCode.Created };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ApiResponse { Message = $"An error occurred: {ex.Message}", StatusCode = HttpStatusCode.InternalServerError };
            }
        }

        public async Task<ApiResponse> UpdatePostAsync(UpdatePostRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var post = await _context.Posts.SingleAsync(p => p.Id == request.PostId);
                post.Content = request.Content;
                post.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new ApiResponse { Message = "Post updated successfully.", StatusCode = HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ApiResponse { Message = $"An error occurred: {ex.Message}", StatusCode = HttpStatusCode.InternalServerError };
            }
        }

        public async Task<ApiResponse> DeletePostAsync(DeletePostRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var images = await _context.PostImages.Where(pi => pi.PostId == request.PostId).ToListAsync();
                _context.PostImages.RemoveRange(images);
                await _context.SaveChangesAsync();

                var post = await _context.Posts.SingleAsync(p => p.Id == request.PostId);
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return new ApiResponse { Message = "Post deleted successfully.", StatusCode = HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ApiResponse { Message = $"An error occurred: {ex.Message}", StatusCode = HttpStatusCode.InternalServerError };
            }
        }

        public async Task<PagedResponse<PostResponse>> GetByIdAsync(int userId, string? cursor, int pageSize)
        {
            var queryPost = from p in _context.Posts
                            where p.UserId == userId
                            select p;

            if (cursor != null)
            {
                var payload = _cursorService.DecodeCursor(cursor);
                if (payload == null || payload.Type != CursorType.Post)
                {
                    return new PagedResponse<PostResponse> { Message = "Invalid cursor.", StatusCode = HttpStatusCode.BadRequest };
                }
                queryPost = queryPost.Where(p => p.CreatedAt < payload.CreatedAt || (p.CreatedAt == payload.CreatedAt && p.Id < payload.Id));
            }

            var posts = await (from p in queryPost
                               join u in _context.Users on p.UserId equals u.Id
                               orderby p.CreatedAt descending, p.Id descending
                               select new
                               {
                                   Id = p.Id,
                                   Content = p.Content,
                                   UserId = u.Id,
                                   Username = u.Username,
                                   CreatedAt = p.CreatedAt
                               })
                               .Take(pageSize + 1)
                               .ToListAsync();

            var postImages = await (from pi in _context.PostImages
                                    where posts.Select(x => x.Id).Contains(pi.PostId)
                                    select new
                                    {
                                        PostId = pi.PostId,
                                        Id = pi.Id,
                                        ImageUrl = pi.ImageUrl
                                    })
                                    .ToListAsync();

            var postDic = posts.ToDictionary(x => x.Id, x => new List<ImageResponse>());
            foreach (var image in postImages)
            {
                postDic[image.PostId].Add(new ImageResponse
                {
                    Id = image.Id,
                    ImageUrl = image.ImageUrl
                });
            }

            var data = (from p in posts
                        select new PostResponse
                        {
                            Id = p.Id,
                            Content = p.Content,
                            ImageUrl = postDic[p.Id],
                            UserId = p.Id,
                            Username = p.Username,
                            CreatedAt = p.CreatedAt
                        })
                        .Take(pageSize)
                        .ToList();

            bool hasNextPage = posts.Count() > pageSize;

            return new PagedResponse<PostResponse>
            {
                Data = data,
                LastCursor = data.Any() ? _cursorService.EncodeCursor(new CursorPayload { Type = CursorType.Post, Id = data.Last().Id, CreatedAt = data.Last().CreatedAt }) : null,
                HasNextPage = hasNextPage,
                Message = "Posts retrieved successfully.",
                StatusCode = HttpStatusCode.OK
            };
        }

        public async Task<PagedResponse<PostResponse>> GetByFollowAsync(int userId, string? cursor, int pageSize)
        {
            var queryPost = from p in _context.Posts
                            where _context.Follows
                                .Where(f => f.FollowerId == userId)
                                .Select(f => f.FollowingId)
                                .Contains(p.UserId)
                            select p;

            if (cursor != null)
            {
                var payload = _cursorService.DecodeCursor(cursor);
                if (payload == null || payload.Type != CursorType.Post)
                {
                    return new PagedResponse<PostResponse> { Message = "Invalid cursor.", StatusCode = HttpStatusCode.BadRequest };
                }
                queryPost = queryPost.Where(p => p.CreatedAt < payload.CreatedAt || (p.CreatedAt == payload.CreatedAt && p.Id < payload.Id));
            }

            var posts = await (from p in queryPost
                               join u in _context.Users on p.UserId equals u.Id
                               orderby p.CreatedAt descending, p.Id descending
                               select new
                               {
                                   Id = p.Id,
                                   Content = p.Content,
                                   UserId = u.Id,
                                   Username = u.Username,
                                   CreatedAt = p.CreatedAt
                               })
                               .Take(pageSize + 1)
                               .ToListAsync();

            var postImages = await (from pi in _context.PostImages
                                    where posts.Select(x => x.Id).Contains(pi.PostId)
                                    select new
                                    {
                                        PostId = pi.PostId,
                                        Id = pi.Id,
                                        ImageUrl = pi.ImageUrl
                                    })
                                    .ToListAsync();

            var postDic = posts.ToDictionary(x => x.Id, x => new List<ImageResponse>());
            foreach (var image in postImages)
            {
                postDic[image.PostId].Add(new ImageResponse
                {
                    Id = image.Id,
                    ImageUrl = image.ImageUrl
                });
            }

            var data = (from p in posts
                        select new PostResponse
                        {
                            Id = p.Id,
                            Content = p.Content,
                            ImageUrl = postDic[p.Id],
                            UserId = p.Id,
                            Username = p.Username,
                            CreatedAt = p.CreatedAt
                        })
                        .Take(pageSize)
                        .ToList();

            bool hasNextPage = posts.Count > pageSize;

            return new PagedResponse<PostResponse>
            {
                Data = data,
                LastCursor = data.Any() ? _cursorService.EncodeCursor(new CursorPayload { Type = CursorType.Post, Id = data.Last().Id, CreatedAt = data.Last().CreatedAt }) : null,
                HasNextPage = hasNextPage,
                Message = "Posts retrieved successfully.",
                StatusCode = HttpStatusCode.OK
            };
        }
    }
}