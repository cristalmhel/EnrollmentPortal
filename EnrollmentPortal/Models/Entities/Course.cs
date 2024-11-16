using System.ComponentModel.DataAnnotations;

namespace EnrollmentPortal.Models.Entities
{
    public class Course
    {
        [Key]
        public int Id { get; set; }   // Primary Key for Course

        [Required]
        [MaxLength(10)]
        public string? Code { get; set; }  // Course Code (e.g., "CS101")

        [Required]
        [MaxLength(100)]
        public string? Name { get; set; }  // Course Name (e.g., "Computer Science")

        public string? Description { get; set; }  // Course Description

        public string? Status { get; set; }

        // One-to-many relationship with StudentFile
        public ICollection<StudentFile>? StudentFiles { get; set; }

        // One-to-many relationship with SubjectFile
        public ICollection<SubjectFile>? SubjectFiles { get; set; }
    }
}
