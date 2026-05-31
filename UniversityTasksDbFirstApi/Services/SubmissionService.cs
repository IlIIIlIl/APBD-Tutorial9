using Microsoft.EntityFrameworkCore;
using UniversityTasksDbFirstApi.Data;
using UniversityTasksDbFirstApi.DTOs;
using UniversityTasksDbFirstApi.Models;

namespace UniversityTasksDbFirstApi.Services;

public class SubmissionService
{
    private readonly UniversityTasksDbContext _context;

    public SubmissionService(UniversityTasksDbContext context)
    {
        _context = context;
    }

    public async Task<(bool Success, string? Error, Submission? Submission)>
        CreateSubmissionAsync(CreateSubmissionDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.RepositoryUrl))
            return (false, "RepositoryUrl cannot be empty.", null);

        if (!dto.RepositoryUrl.StartsWith("https://"))
            return (false, "RepositoryUrl must start with https://", null);

        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.StudentId == dto.StudentId);

        if (student == null)
            return (false, "Student does not exist.", null);

        if (!student.IsActive)
            return (false, "Student is inactive.", null);

        var assignment = await _context.Assignments
            .FirstOrDefaultAsync(a => a.AssignmentId == dto.AssignmentId);

        if (assignment == null)
            return (false, "Assignment does not exist.", null);

        if (!assignment.IsPublished)
            return (false, "Assignment is not published.", null);

        var enrolled = await _context.Enrollments.AnyAsync(e =>
            e.StudentId == dto.StudentId &&
            e.CourseId == assignment.CourseId &&
            (e.Status == "Active" || e.Status == "Completed"));

        if (!enrolled)
            return (false,
                "Student is not enrolled in the assignment course.",
                null);

        var alreadySubmitted = await _context.Submissions.AnyAsync(s =>
            s.StudentId == dto.StudentId &&
            s.AssignmentId == dto.AssignmentId);

        if (alreadySubmitted)
            return (false,
                "Student already submitted this assignment.",
                null);

        var submission = new Submission
        {
            AssignmentId = dto.AssignmentId,
            StudentId = dto.StudentId,
            RepositoryUrl = dto.RepositoryUrl,
            SubmittedAt = DateTime.UtcNow,
            Status = DateTime.UtcNow > assignment.DueDate
                ? "Late"
                : "Submitted"
        };

        _context.Submissions.Add(submission);

        await _context.SaveChangesAsync();
        
        var fullSubmission = await _context.Submissions
            .Include(s => s.Student)
            .Include(s => s.Assignment)
            .FirstAsync(s => s.SubmissionId == submission.SubmissionId);

        return (true, null, fullSubmission);
    }

    public async Task<(bool Success, string? Error)>
        GradeSubmissionAsync(int submissionId, GradeSubmissionDto dto)
    {
        var submission = await _context.Submissions
            .Include(s => s.Assignment)
            .FirstOrDefaultAsync(s => s.SubmissionId == submissionId);

        if (submission == null)
            return (false, "Submission not found.");

        if (dto.Score < 0)
            return (false, "Score cannot be negative.");

        if (dto.Score > submission.Assignment.MaxPoints)
            return (false,
                $"Score cannot exceed {submission.Assignment.MaxPoints}.");


        
        submission.Score = dto.Score;
        submission.Feedback = dto.Feedback;
        submission.Status = "Graded";

        await _context.SaveChangesAsync();

        return (true, null);
    }

    public async Task<(bool Success, string? Error)>
        DeleteSubmissionAsync(int submissionId)
    {
        var submission = await _context.Submissions
            .FirstOrDefaultAsync(s => s.SubmissionId == submissionId);

        if (submission == null)
            return (false, "Submission not found.");

        if (submission.Status == "Graded")
            return (false,
                "Graded submissions cannot be deleted.");

        _context.Submissions.Remove(submission);

        await _context.SaveChangesAsync();

        return (true, null);
    }
    
    public async Task<SubmissionDto?> GetByIdAsync(int id)
    {
        return await _context.Submissions
            .AsNoTracking()
            .Where(s => s.SubmissionId == id)
            .Select(s => new SubmissionDto
            {
                SubmissionId = s.SubmissionId,
                Student = s.Student.FirstName + " " + s.Student.LastName,
                Assignment = s.Assignment.Title,
                RepositoryUrl = s.RepositoryUrl,
                Status = s.Status,
                Score = s.Score,
                Feedback = s.Feedback
            })
            .FirstOrDefaultAsync();
    }
}