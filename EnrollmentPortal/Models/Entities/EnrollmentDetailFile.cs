using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EnrollmentPortal.Models.Entities
{
    public class EnrollmentDetailFile
    {
        [Key]
        public int Id { get; set; }  // Primary key for the table

        // Foreign key to EnrollmentHeaderFile
        [Required]
        public int EnrollmentHeaderFileId { get; set; }  // Reference to EnrollmentHeaderFile
        public EnrollmentHeaderFile? EnrollmentHeaderFile { get; set; }  // Navigation property to EnrollmentHeaderFile

        // Foreign key to SubjectFile
        [Required]
        public int SubjectFileId { get; set; }  // Reference to SubjectFile
        public SubjectFile? SubjectFile { get; set; }  // Navigation property to SubjectFile

        // Foreign key to SubjectSchedFile
        [Required]
        public int SubjectSchedFileId { get; set; }  // Reference to SubjectSchedFile
        public SubjectSchedFile? SubjectSchedFile { get; set; }  // Navigation property to SubjectSchedFile

        // Status of the enrollment detail (e.g., active, dropped)
        [MaxLength(20)]
        public string? ENRDFSTUDSTATUS { get; set; }  // Enrollment detail status
    }
}
