using kolos.DTOs;
using kolos.Services;
using Microsoft.AspNetCore.Mvc;

namespace kolos.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectsService _projectService;

    public ProjectsController(IProjectsService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProjectByIdAsync(int id, CancellationToken cancellationToken)
    {
        var project = await _projectService.GetProjectByIdAsync(id, cancellationToken);
        return Ok(project);
    }

    [HttpPost]
    public async Task<IActionResult> AddArtifactAndProjectAsync([FromBody] NewArtifactWithProjectDTO request)
    {
        var result = await _projectService.AddArtifactAndProjectAsync(request);

        if (!result)
            return NotFound("Artifact not found");

        return Created("", null); 
    }

    
}