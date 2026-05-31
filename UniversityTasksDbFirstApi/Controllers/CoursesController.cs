using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityTasksDbFirstApi.Data;
using UniversityTasksDbFirstApi.DTOs;

namespace UniversityTasksDbFirstApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController : ControllerBase
    {
        private readonly UniversityTasksDbContext _context;

        public CoursesController(UniversityTasksDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCourses(
            bool activeOnly = false)
        {
            var query = _context.Courses
                .AsNoTracking();

            if (activeOnly)
            {
                query = query.Where(c => c.IsActive);
            }

            var result = await query
                .Select(c => new CourseDto
                {
                    CourseId = c.CourseId,
                    Code = c.Code,
                    Name = c.Name,
                    Credits = c.Credits,
                    AssignmentCount = c.Assignments.Count()
                })
                .ToListAsync();

            return Ok(result);
        }
        
        [HttpGet("{idCourse}/assignments")]
        public async Task<IActionResult> GetAssignments(
            int idCourse,
            bool publishedOnly = false)
        {
            var courseExists = await _context.Courses
                .AnyAsync(c => c.CourseId == idCourse);

            if (!courseExists)
                return NotFound();

            var query = _context.Assignments
                .AsNoTracking()
                .Where(a => a.CourseId == idCourse);

            if (publishedOnly)
            {
                query = query.Where(a => a.IsPublished);
            }

            var result = await query
                .Select(a => new AssignmentDto
                {
                    AssignmentId = a.AssignmentId,
                    Title = a.Title,
                    DueDate = a.DueDate,
                    MaxPoints = a.MaxPoints,
                    IsPublished = a.IsPublished,
                    SubmissionCount = a.Submissions.Count()
                })
                .ToListAsync();

            return Ok(result);
        }
        
        
        [HttpGet("{idStudent}/dashboard")]
        public async Task<IActionResult> GetDashboard(int idStudent)
        {
            var student = await _context.Students
                .AsNoTracking()
                .Where(s => s.StudentId == idStudent)
                .Select(s => new StudentDashboardDto
                {
                    StudentId = s.StudentId,
                    IndexNumber = s.IndexNumber,
                    FullName = s.FirstName + " " + s.LastName,
                    IsActive = s.IsActive,

                    Enrollments = s.Enrollments
                        .Select(e => new StudentEnrollmentDto
                        {
                            CourseName = e.Course.Name,
                            Status = e.Status
                        })
                        .ToList(),

                    Submissions = s.Submissions
                        .Select(sub => new StudentSubmissionDto
                        {
                            AssignmentTitle = sub.Assignment.Title,
                            Status = sub.Status,
                            Score = sub.Score
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (student == null)
                return NotFound();

            return Ok(student);
        }
        
        
    }
    
    
}
