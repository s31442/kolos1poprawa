using kolos.DTOs;

namespace kolos.Services;

public interface IProjectsService
{
    public Task<ProjectResponseDTO> GetProjectByIdAsync(int id, CancellationToken cancellationToken);
    Task<bool> AddArtifactAndProjectAsync(NewArtifactWithProjectDTO request);
}