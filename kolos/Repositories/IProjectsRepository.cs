using kolos.DTOs;

namespace kolos.Repositories;

public interface IProjectsRepository
{
    public Task<ProjectResponseDTO> GetProjectByIdAsync(int projectId, CancellationToken cancellationToke);
    Task<bool> AddArtifactAndProjectAsync(NewArtifactWithProjectDTO request);
}