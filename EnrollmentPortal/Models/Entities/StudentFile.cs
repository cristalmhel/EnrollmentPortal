using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EnrollmentPortal.Models.Entities
{
    public class StudentFile
    {
        [Key]
        public long StudId { get; set; }  // Primary key for the table

        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(50)]
        public string? STFSTUDLNAME { get; set; }  // Student last name

        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(50)]
        public string? STFSTUDFNAME { get; set; }  // Student first name

        [Required(ErrorMessage = "Middle name is required."), MaxLength(50)]
        public string? STFSTUDMNAME { get; set; }  // Student middle name (optional)

        [Required(ErrorMessage = "Year is required.")]
        public int STFSTUDYEAR { get; set; }  // Student year level

        [MaxLength(200)]
        public string? STFSTUDREMARKS { get; set; }  // Remarks about the student (optional)

        [Required(ErrorMessage = "Status is required.")]
        public string? STFSTUDSTATUS { get; set; }  // Status of the student (e.g., active, inactive)

        // Foreign key to Course
        [Required]
        public int CourseId { get; set; }  // Reference to Course
        public Course? Course { get; set; }  // Navigation property to Course

        // One-to-many relationship: each student can have multiple enrollment headers
        public ICollection<EnrollmentHeaderFile>? EnrollmentHeaderFiles { get; set; }  // Navigation property for EnrollmentHeaderFile
    }
}
