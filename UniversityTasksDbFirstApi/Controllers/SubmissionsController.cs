using Microsoft.AspNetCore.Mvc;
using UniversityTasksDbFirstApi.DTOs;
using UniversityTasksDbFirstApi.Services;

namespace UniversityTasksDbFirstApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubmissionsController : ControllerBase
{
    private readonly SubmissionService _service;

    public SubmissionsController(SubmissionService service)
    {
        _service = service;
    }

    // POST /api/submissions
    [HttpPost]
    public async Task<IActionResult> Create(CreateSubmissionDto dto)
    {
        var result = await _service.CreateSubmissionAsync(dto);

        if (!result.Success)
            return BadRequest(result.Error);

        var submission = result.Submission!;

        return CreatedAtAction(
            nameof(GetById),
            new { id = submission.SubmissionId },
            new SubmissionDto
            {
                SubmissionId = submission.SubmissionId,
                Student = submission.Student.FirstName + " " + submission.Student.LastName,
                Assignment = submission.Assignment.Title,
                RepositoryUrl = submission.RepositoryUrl,
                Status = submission.Status,
                Score = submission.Score,
                Feedback = submission.Feedback
            });
    }

    // helper endpoint (not required but useful for CreatedAtAction)
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var submission = await _service.GetByIdAsync(id);

        if (submission == null)
            return NotFound();

        return Ok(submission);
    }

    // PUT /api/submissions/{id}/grade
    [HttpPut("{id}/grade")]
    public async Task<IActionResult> Grade(int id, GradeSubmissionDto dto)
    {
        var result = await _service.GradeSubmissionAsync(id, dto);

        if (!result.Success)
            return BadRequest(result.Error);

        return NoContent();
    }

    // DELETE /api/submissions/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteSubmissionAsync(id);

        if (!result.Success)
            return BadRequest(result.Error);

        return NoContent();
    }
}