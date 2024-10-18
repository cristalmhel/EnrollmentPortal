using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EnrollmentPortal.Models.Entities
{
    public class SubjectSchedFile
    {
        // Primary key for the table
        [Key]
        public int Id { get; set; }  // Auto-generated Primary Key

        [Required(ErrorMessage = "EDP Code is required.")]
        [MaxLength(20)]
        public string? SSFEDPCODE { get; set; }  // Schedule code

        // Foreign key to SubjectFile
        [Required(ErrorMessage = "Subject is required.")]
        public int SubjectFileId { get; set; }  // Reference to SubjectFile
        public SubjectFile? SubjectFile { get; set; }  // Navigation property to SubjectFile

        [Required(ErrorMessage = "Start time is required.")]
        public DateTime SSFSTARTTIME { get; set; }  // Class start time

        [Required(ErrorMessage = "End time is required.")]
        public DateTime SSFENDTIME { get; set; }  // Class end time

        [Required(ErrorMessage = "Days is required.")]
        [MaxLength(50)]
        public string? SSFDAYS { get; set; }  // Class days (e.g., MWF, TTh)

        [MaxLength(20)]
        public string? SSFROOM { get; set; }  // Room number for the class

        [Required(ErrorMessage = "Class maximum size is required.")]
        public int SSFMAXSIZE { get; set; }  // Maximum class size

        public int SSFCLASSSIZE { get; set; }  // Current class size

        [Required(ErrorMessage = "Status is required.")]
        [MaxLength(10)]
        public string? SSFSTATUS { get; set; }  // Status of the schedule (e.g., active, closed)

        [MaxLength(3)]
        public string? SSFXM { get; set; }  // AM/PM

        [MaxLength(50)]
        public string? SSFSECTION { get; set; }  // Section (e.g., A, B)

        [Required(ErrorMessage = "Year is required.")]
        public int SSFSCHOOLYEAR { get; set; }  // School year of the schedule

        public string? SSFSSEM { get; set; }  // Semester (e.g., 1st sem, 2nd sem)

        // One-to-many relationship: each schedule can have multiple enrollments
        public ICollection<EnrollmentDetailFile>? EnrollmentDetailFiles { get; set; }  // Navigation property to EnrollmentDetailFile
    }
}
