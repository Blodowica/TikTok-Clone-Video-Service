using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TikTok_Clone_Video_Service.Models;


namespace TikTok_Clone_Video_Service.DatabaseContext
{
    public class VideoDatabaseContext : DbContext
    {
        public DbSet<Video> Vdeos { get; set; }
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

            modelBuilder.Entity<Comment>()
                .HasOne<Video>(c => c.Video)
                .WithMany(v => v.Comments)
                .HasForeignKey(c => c.Id)
                .OnDelete(DeleteBehavior.Cascade);
             
            
        }
    }


 
}
