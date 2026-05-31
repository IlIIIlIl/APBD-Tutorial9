namespace UniversityTasksDbFirstApi.DTOs;

public class StudentDashboardDto
{
    public int StudentId { get; set; }

    public string IndexNumber { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public bool IsActive { get; set; }

    public List<StudentEnrollmentDto> Enrollments { get; set; } = [];

    public List<StudentSubmissionDto> Submissions { get; set; } = [];
}

public class StudentEnrollmentDto
{
    public string CourseName { get; set; } = null!;

    public string Status { get; set; } = null!;
}

public class StudentSubmissionDto
{
    public string AssignmentTitle { get; set; } = null!;

    public string Status { get; set; } = null!;

    public int? Score { get; set; }
}