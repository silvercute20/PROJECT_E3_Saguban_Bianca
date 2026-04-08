using auth_api.Models;
using Microsoft.EntityFrameworkCore;

namespace auth_api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Puzzle> Puzzles { get; set; } = null!;
        public DbSet<UserPuzzleProgress> UserPuzzleProgresses { get; set; } = null!;
        public DbSet<GameSubmission> GameSubmissions { get; set; } = null!;
        public DbSet<Guess> Guesses { get; set; } = null!;
        public DbSet<Image> Images { get; set; } = null!;
        public DbSet<Tag> Tags { get; set; } = null!;
        public DbSet<ImageTag> ImageTags { get; set; } = null!;
        public DbSet<Pack> Packs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ImageTag>()
                .HasKey(it => new { it.ImageId, it.TagId });

            modelBuilder.Entity<ImageTag>()
                .HasOne(it => it.Image)
                .WithMany(i => i.ImageTags)
                .HasForeignKey(it => it.ImageId);

            modelBuilder.Entity<ImageTag>()
                .HasOne(it => it.Tag)
                .WithMany(t => t.ImageTags)
                .HasForeignKey(it => it.TagId);

            base.OnModelCreating(modelBuilder);
        }
    }
}