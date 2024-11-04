using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EnrollmentPortal.Models.Entities
{
    public class EnrollmentHeaderFile
    {
        [Key]
        public int Id { get; set; }  // Primary key for the table

        [Required(ErrorMessage = "Enroll date is required.")]
        public DateTime ENRHFSTUDDATEENROLL { get; set; }  // Date of enrollment

        [Required(ErrorMessage = "School year is required.")]
        [MaxLength(10)]
        public string? ENRHFSTUDSCHLYR { get; set; }  // School year (e.g., 2023-2024)

        [Required(ErrorMessage = "Semester is required.")]
        [MaxLength(10)]
        public string? ENRHFSTUDSEM { get; set; }  // Semester (e.g., 1st, 2nd)

        [MaxLength(50)]
        public string? ENRHFSTUDENCODER { get; set; }  // Person who encoded the enrollment

        public double ENRHFSTUDTOTALUNITS { get; set; }  // Total number of units enrolled

        [Required(ErrorMessage = "Status is required.")]
        [MaxLength(20)]
        public string? ENRHFSTUDSTATUS { get; set; }  // Enrollment status

        // Foreign key to StudentFile
        [Required(ErrorMessage = "Student ID is required.")]
        public long StudentFileId { get; set; }  // Reference to StudentFile
        public StudentFile? StudentFile { get; set; }  // Navigation property to StudentFile

        // One-to-many relationship: each EnrollmentHeaderFile can have multiple enrollment details
        public ICollection<EnrollmentDetailFile>? EnrollmentDetailFiles { get; set; }  // Navigation property to EnrollmentDetailFile
    }

}
