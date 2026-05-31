namespace UniversityTasksDbFirstApi.DTOs;

public class CourseDto
{
    public int CourseId { get; set; }

    public string Code { get; set; }

    public string Name { get; set; }

    public int Credits { get; set; }

    public int AssignmentCount { get; set; }
}