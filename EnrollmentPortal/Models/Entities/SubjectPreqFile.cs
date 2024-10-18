using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EnrollmentPortal.Models.Entities
{
    public class SubjectPreqFile
    {
        [Key]
        public int Id { get; set; }

        public int SubjectFileId { get; set; }
        public SubjectFile? SubjectFile { get; set; }

        public int PrerequisiteSubjectId { get; set; }
        public SubjectFile? PrerequisiteSubject { get; set; }

        [MaxLength(50)]
        public string? SUBJCATEGORY { get; set; }
    }
}
