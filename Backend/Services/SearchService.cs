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

        public async Task<ApiResponse<List<PostResponse>>> SearchPostsAsync(string q, string? cursor)
        {
            if (string.IsNullOrWhiteSpace(q))
                return new ApiResponse<List<PostResponse>> { Data = null, Message = "Query string 'q' is required.", StatusCode = HttpStatusCode.BadRequest }; ;

            const int pageSize = 10;

            var query = _context.Posts.Where(p => EF.Functions.Like(p.Content, $"%{q}%"));

            if (cursor != null)
            {
                var payload = _cursorService.DecodeCursor(cursor);
                if (payload == null || payload.Type != CursorType.Post)
                {
                    return new ApiResponse<List<PostResponse>> { Message = "Invalid cursor.", StatusCode = HttpStatusCode.BadRequest };
                }
                query = query.Where(p => p.Id < payload.Id);
            }

            var posts = await (from p in query
                               orderby p.Id descending
                               select new
                               {
                                   Id = p.Id,
                                   Content = p.Content,
                                   CreatedAt = p.CreateAt
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
                            CreatedAt = p.CreatedAt
                        })
                        .Take(pageSize)
                        .ToList();

            bool hasNextPage = posts.Count() > pageSize;

            return new PagedResponse<PostResponse>
            {
                Data = data,
                LastCursor = data.Any() ? new LastCursor { CreatedAt = data.Last().CreatedAt, Id = data.Last().Id } : null,
                HasNextPage = hasNextPage,
                Message = "Search Posts successfully.",
                StatusCode = HttpStatusCode.OK
            };
        }
    }
}