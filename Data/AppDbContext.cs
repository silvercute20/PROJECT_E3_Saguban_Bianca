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
    }
}