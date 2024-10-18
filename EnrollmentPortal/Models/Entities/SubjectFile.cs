using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EnrollmentPortal.Models.Entities
{
    public class SubjectFile
    {
        [Key]
        public int Id { get; set; }  // Primary Key for the SubjectFile

        [Required(ErrorMessage = "Subject Code is required.")]
        [MaxLength(20)]
        public string? SFSUBJCODE { get; set; }  // Subject Code

        [MaxLength(100)]
        public string? SFSUBJDESC { get; set; }  // Subject Description

        [Required(ErrorMessage = "Units is required.")]
        public int SFSUBJUNITS { get; set; }  // Subject Units

        [Required(ErrorMessage = "Regular Offering is required.")]
        [MaxLength(50)]
        public string? SFSUBJREGOFRNG { get; set; }  // Regular Offering

        [Required(ErrorMessage = "School Year is required.")]
        [MaxLength(10)]
        public string? SFSUBJSCHLYR { get; set; }  // School Year

        [MaxLength(50)]
        public string? SFSUBJCATEGORY { get; set; }  // Subject Category

        [MaxLength(20)]
        public string? SFSUBJSTATUS { get; set; }  // Subject Status

        [MaxLength(50)]
        public string? SFSUBJCURRCODE { get; set; }  // Curriculum Code

        // Foreign key to Course
        public int CourseId { get; set; }
        public Course? Course { get; set; }

        // One-to-many relationships
        public ICollection<SubjectPreqFile>? SubjectPreqFile { get; set; }
        public ICollection<SubjectSchedFile>? SubjectSchedFile { get; set; }
        public ICollection<EnrollmentDetailFile>? EnrollmentDetailFiles { get; set; }
    }
}
