using kolos.DTOs;
using kolos.Exceptions;
using kolos.Repositories;

namespace kolos.Services;

public class ProjectsService : IProjectsService
{
    private readonly IProjectsRepository _projectsRepository;

    public ProjectsService(IProjectsRepository projectsRepository)
    {
        _projectsRepository = projectsRepository;
    }
    
    public async Task<ProjectResponseDTO> GetProjectByIdAsync(int id, CancellationToken cancellationToken)
    {
        if(id<=0)
            throw new BadRequestException("Invalid projectId");
        var project = await _projectsRepository.GetProjectByIdAsync(id, cancellationToken);
        return project;
    }

    public async Task<bool> AddArtifactAndProjectAsync(NewArtifactWithProjectDTO request)
    {
        return await _projectsRepository.AddArtifactAndProjectAsync(request);
    }
}