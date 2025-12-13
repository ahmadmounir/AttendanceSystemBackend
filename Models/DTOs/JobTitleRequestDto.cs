namespace AttendanceSystemBackend.Models.DTOs
{
    public class JobTitleRequestDto
    {
        public string TitleName { get; set; }
        public float MinSalary { get; set; }
        public float MaxSalary { get; set; }
    }
}
