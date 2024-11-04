namespace EnrollmentPortal.Models.ViewModel
{
    public class ScheduleViewModel
    {
        public int? scheduleId { get; set; } = 0;
        public int? subjectId { get; set; } = 0;
        public string edpCode { get; set; } = string.Empty;
        public string subjectCode { get; set; } = string.Empty;
        public int? subjectUnits { get; set; } = 0;
        public DateTime? startTime { get; set; }
        public DateTime? endTime { get; set; }
        public string ampm { get; set; } = string.Empty;
        public string days {  get; set; } = string.Empty;
        public string room { get; set; } = string.Empty;
        public int? maxSize { get; set; } = 0;
        public int? classSize { get; set; } = 0;
    }
}
