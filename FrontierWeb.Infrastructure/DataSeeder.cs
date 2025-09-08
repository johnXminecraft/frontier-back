using FrontierWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


//TESTING ONLY
//TODO: REMOVE

namespace FrontierWeb.Infrastructure
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(IServiceProvider sp)
        {
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BlogDbContext>();
            await db.Database.EnsureCreatedAsync();

            if (!await db.Users.AnyAsync())
            {
                db.Users.Add(new User
                {
                    Username = "admin",
                    PasswordHash = PasswordHash.Create("admin123"),
                    Role = "Admin"
                });
            }

            if (!await db.Posts.AnyAsync())
            {
                db.Posts.Add(new Post
                {
                    Title = "Hello, world!",
                    Slug = "hello-world",
                    Content = "This is your first post.",
                    Published = true,
                    CreatedAtUtc = DateTime.UtcNow,
                    UpdatedAtUtc = DateTime.UtcNow
                });
            }

            await db.SaveChangesAsync();
        }
    }
}
