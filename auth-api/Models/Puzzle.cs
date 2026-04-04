using System.ComponentModel.DataAnnotations;

namespace auth_api.Models
{
    public class Puzzle
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PackId { get; set; }

        [Required]
        public string ImageUrl { get; set; } = string.Empty;

        [Required]
        public string Answer { get; set; } = string.Empty;
    }
}