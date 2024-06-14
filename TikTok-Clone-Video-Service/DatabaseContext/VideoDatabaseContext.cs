using Microsoft.EntityFrameworkCore;
using TikTok_Clone_Video_Service.Models;

namespace TikTok_Clone_Video_Service.DatabaseContext
{
    public class VideoDatabaseContext : DbContext
    {
        public DbSet<Video> Videos { get; set; }
        public DbSet<Comment> Comments { get; set; }

        public DbSet<UserLikedVideos> UserLikedVideos { get; set; }

        public VideoDatabaseContext(DbContextOptions<VideoDatabaseContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Video>()
                .HasMany(v => v.Comments)

                .WithOne(c => c.Video)
                .HasForeignKey(c => c.VideoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Video>().HasMany(v => v.UserLikedVideos)
            .WithOne(e => e.Video)
            .HasForeignKey(e => e.VideoID)
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
