using Backend.Models;
using Backend.Services.Interfaces;
using Backend.ViewModels;
using Microsoft.EntityFrameworkCore;
using static Backend.Services.CursorService;

namespace Backend.Services
{
    public class SearchService : ISearchService
    {
        private readonly MiniSocialContext _context;
        private readonly ICursorService _cursorService;
        public SearchService(MiniSocialContext context, ICursorService cursorService)
        {
            _context = context;
            _cursorService = cursorService;
        }

        public async Task<ApiResponse<List<PostResponse>>> SearchPostsAsync(string q, string? cursor, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(q))
                return new ApiResponse<List<PostResponse>> { Data = null, Message = "Query string 'q' is required.", StatusCode = HttpStatusCode.BadRequest }; ;

            var query = _context.Posts.Where(p => EF.Functions.Like(p.Content, $"%{q}%"));

            if (cursor != null)
            {
                var payload = _cursorService.DecodeCursor(cursor);
                if (payload == null || payload.Type != CursorType.Post)
                {
                    return new ApiResponse<List<PostResponse>> { Message = "Invalid cursor.", StatusCode = HttpStatusCode.BadRequest };
                }
                query = query.Where(m => m.CreatedAt < payload.CreatedAt || (m.CreatedAt == payload.CreatedAt && m.Id < payload.Id));
            }

            var posts = await (from p in query
                               join u in _context.Users on p.UserId equals u.Id
                               orderby p.Id descending
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
                Message = "Search Posts successfully.",
                StatusCode = HttpStatusCode.OK
            };
        }

        public async Task<ApiResponse<List<UserResponse>>> SearchUsersAsync(string q, string? cursor, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(q))
                return new ApiResponse<List<UserResponse>> { Data = null, Message = "Query string 'q' is required.", StatusCode = HttpStatusCode.BadRequest }; ;

            var query = _context.Users.Where(p => EF.Functions.Like(p.Username, $"%{q}%"));

            if (cursor != null)
            {
                var payload = _cursorService.DecodeCursor(cursor);
                if (payload == null || payload.Type != CursorType.User)
                {
                    return new ApiResponse<List<UserResponse>> { Message = "Invalid cursor.", StatusCode = HttpStatusCode.BadRequest };
                }
                query = query.Where(m => m.CreatedAt < payload.CreatedAt || (m.CreatedAt == payload.CreatedAt && m.Id < payload.Id));
            }

            var users = await (from u in query
                               orderby u.Id descending
                               select new
                               {
                                   Id = u.Id,
                                   Username = u.Username,
                                   CreatedAt = u.CreatedAt
                               })
                               .Take(pageSize + 1)
                               .ToListAsync();

            var data = (from u in users
                        select new UserResponse
                        {
                            Id = u.Id,
                            Username = u.Username,
                            CreatedAt = u.CreatedAt
                        })
                        .Take(pageSize)
                        .ToList();

            bool hasNextPage = users.Count() > pageSize;

            return new PagedResponse<UserResponse>
            {
                Data = data,
                LastCursor = data.Any() ? _cursorService.EncodeCursor(new CursorPayload { Type = CursorType.Post, Id = data.Last().Id, CreatedAt = data.Last().CreatedAt }) : null,
                HasNextPage = hasNextPage,
                Message = "Search Users successfully.",
                StatusCode = HttpStatusCode.OK
            };
        }
    }
}