using System.ComponentModel.DataAnnotations;

namespace EnrollmentPortal.Models.Entities
{
    public class User
    {
        [Key]
        [StringLength(9)]
        public string Id { get; set; }

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        public string MiddleInitial { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Phone { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        // Constructor to generate the unique ID when creating a new entity
        public User()
        {
            Id = GenerateCustomId();
        }

        // Method to generate a unique 9-character ID
        private string GenerateCustomId()
        {
            // Generate a 9-character numeric string. You can also make this alphanumeric.
            return Guid.NewGuid().ToString().Substring(0, 9);
        }
    }
}
