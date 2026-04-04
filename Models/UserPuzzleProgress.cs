using System.ComponentModel.DataAnnotations;

namespace auth_api.Models
{
    public class UserPuzzleProgress
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int PuzzleId { get; set; }

        public bool Solved { get; set; } = false;
    }
}