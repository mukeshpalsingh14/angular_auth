using System.ComponentModel.DataAnnotations;

namespace AngularAuthApi.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string  FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
    }
}

