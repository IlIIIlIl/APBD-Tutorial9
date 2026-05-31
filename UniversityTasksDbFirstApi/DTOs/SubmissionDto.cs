namespace UniversityTasksDbFirstApi.DTOs;

public class SubmissionDto
{
    public int SubmissionId { get; set; }

    public string Student { get; set; } = null!;

    public string Assignment { get; set; } = null!;

    public string RepositoryUrl { get; set; } = null!;

    public string Status { get; set; } = null!;

    public int? Score { get; set; }

    public string? Feedback { get; set; }
}