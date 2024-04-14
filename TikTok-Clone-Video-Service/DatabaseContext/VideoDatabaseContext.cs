using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TikTok_Clone_Video_Service.Models;


namespace TikTok_Clone_Video_Service.DatabaseContext
{
    public class VideoDatabaseContext : DbContext
    {
        public DbSet<Video> Videos { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //connection string 

            //optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=SchoolDb;Trusted_Connection=True;");
           
            optionsBuilder.UseSqlite("Filename=videoDatabase.db");
        }

       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //define the DB tables relationships

            /*  modelBuilder.Entity<Comment>()
              .HasOne(c => c.Video) // Each comment has one video

              .WithMany(v => v.Comments) // Each video has many comments
              .HasForeignKey(c => c.Id); // The foreign key in the Comment table is VideoId



              modelBuilder.Entity<Comment>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd();
  */
           /* modelBuilder.Entity<Comment>()
                  .HasOne(c => c.Video)
                  .WithMany(v => v.Comments)
                  .HasForeignKey(c => c.VideoId);*/

            modelBuilder.Entity<Video>()
                 .HasMany(v => v.Comments)
                 .WithOne(c => c.Video)
                 .HasForeignKey(c => c.VideoId)
                 .OnDelete(DeleteBehavior.Cascade);
                 


        }
    }


 
}
