using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthorizationWithCustomClaim.Entities
{
    public class User : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [NotMapped]
        public string Password { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        public string RoleName { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class RegisterUserModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [NotMapped]
        public string Password { get; set; }

        public string RoleName { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
