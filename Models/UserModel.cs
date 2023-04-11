using System.ComponentModel.DataAnnotations;
namespace BBQLover.webapp.Models
{
    public class UserModel
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
