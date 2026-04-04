using System.ComponentModel.DataAnnotations;

namespace auth_api.Models
{
    public class Guess
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PuzzleId { get; set; }

        [Required]
        public string GuessText { get; set; } = null!;

        [Required]
        public bool Correct { get; set; }
    }
}