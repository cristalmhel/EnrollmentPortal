namespace EnrollmentPortal.Models.ViewModel
{
    public class RequisiteViewModel
    {
        public int? Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int? Units { get; set; }
        public string Category { get; set; } = string.Empty;
    }
}
