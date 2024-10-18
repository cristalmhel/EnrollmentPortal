using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EnrollmentPortal.Models.Entities
{
    public class EnrollmentHeaderFile
    {
        [Key]
        public int Id { get; set; }  // Primary key for the table

        [Required]
        public DateTime ENRHFSTUDDATEENROLL { get; set; }  // Date of enrollment

        [Required]
        [MaxLength(10)]
        public string? ENRHFSTUDSCHLYR { get; set; }  // School year (e.g., 2023-2024)

        [Required]
        [MaxLength(10)]
        public string? ENRHFSTUDSEM { get; set; }  // Semester (e.g., 1st, 2nd)

        [MaxLength(50)]
        public string? ENRHFSTUDENCODER { get; set; }  // Person who encoded the enrollment

        public double ENRHFSTUDTOTALUNITS { get; set; }  // Total number of units enrolled

        [MaxLength(20)]
        public string? ENRHFSTUDSTATUS { get; set; }  // Enrollment status (e.g., active, pending)

        // Foreign key to StudentFile
        [Required]
        public long StudentFileId { get; set; }  // Reference to StudentFile
        public StudentFile? StudentFile { get; set; }  // Navigation property to StudentFile

        // One-to-many relationship: each EnrollmentHeaderFile can have multiple enrollment details
        public ICollection<EnrollmentDetailFile>? EnrollmentDetailFiles { get; set; }  // Navigation property to EnrollmentDetailFile
    }

}
