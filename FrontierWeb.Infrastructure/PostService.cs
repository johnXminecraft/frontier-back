using FrontierWeb.Application.DTO;
using FrontierWeb.Application.Interfaces;
using FrontierWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FrontierWeb.Infrastructure
{
    public class PostService : IPostService
    {
        private readonly BlogDbContext _db;

        public PostService(BlogDbContext db) => _db = db;

        public async Task<Paged<Post>> ListAsync(string? q, int page, int pageSize, bool? published, CancellationToken ct = default)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _db.Posts.AsNoTracking().AsQueryable();

            if (published is not null) query = query.Where(p => p.Published == published);
            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(p => p.Title.Contains(q) || p.Content.Contains(q) || p.Slug.Contains(q));

            var total = await query.CountAsync(ct);
            var items = await query.OrderByDescending(p => p.CreatedAtUtc)
                                   .Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync(ct);

            return new Paged<Post>(items, total, page, pageSize);
        }

        public async Task<Post?> GetByIdOrSlugAsync(string idOrSlug, CancellationToken ct = default)
        {
            if (int.TryParse(idOrSlug, out var id))
                return await _db.Posts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, ct);

            return await _db.Posts.AsNoTracking().FirstOrDefaultAsync(p => p.Slug == idOrSlug, ct);
        }

        public async Task<(bool ok, string? error, Post? post)> CreateAsync(PostCreateRequest req, CancellationToken ct = default)
        {
            if (await _db.Posts.AnyAsync(p => p.Slug == req.Slug, ct))
                return (false, "Slug must be unique.", null);

            var post = new Post
            {
                Title = req.Title.Trim(),
                Slug = req.Slug.Trim(),
                Content = req.Content,
                Published = req.Published,
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };

            _db.Posts.Add(post);
            await _db.SaveChangesAsync(ct);
            return (true, null, post);
        }

        public async Task<(bool ok, string? error, Post? post)> UpdateAsync(int id, PostUpdateRequest req, CancellationToken ct = default)
        {
            var post = await _db.Posts.FirstOrDefaultAsync(p => p.Id == id, ct);
            if (post is null) return (false, null, null);

            if (await _db.Posts.AnyAsync(p => p.Id != id && p.Slug == req.Slug, ct))
                return (false, "Slug must be unique.", null);

            post.Title = req.Title.Trim();
            post.Slug = req.Slug.Trim();
            post.Content = req.Content;
            post.Published = req.Published;
            post.UpdatedAtUtc = DateTime.UtcNow;

            await _db.SaveChangesAsync(ct);
            return (true, null, post);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var post = await _db.Posts.FirstOrDefaultAsync(p => p.Id == id, ct);
            if (post is null) return false;
            _db.Posts.Remove(post);
            await _db.SaveChangesAsync(ct);
            return true;
        }
    }
}
