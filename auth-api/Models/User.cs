using System.ComponentModel.DataAnnotations;
namespace auth_api.Models
{
    public class User
    {
       [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        public string Role { get; set; } = "User";
    }
}