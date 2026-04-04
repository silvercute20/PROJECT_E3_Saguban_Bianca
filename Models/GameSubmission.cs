using System.ComponentModel.DataAnnotations;

namespace auth_api.Models
{
    public class GameSubmission
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int PackId { get; set; }
        public List<Guess> Guesses { get; set; } = new List<Guess>();

        [Required]
        public int Score { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        
    }
}