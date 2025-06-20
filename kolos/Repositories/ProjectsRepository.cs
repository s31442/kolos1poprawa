using System.Data.SqlClient;
using System.Data;
using kolos.DTOs;
using Microsoft.Data.SqlClient;

namespace kolos.Repositories;

public class ProjectsRepository : IProjectsRepository
{
    private readonly IConfiguration _configuration;

    public ProjectsRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<ProjectResponseDTO?> GetProjectByIdAsync(int projectId, CancellationToken cancellationToke)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var command = new SqlCommand(@"
    SELECT
            [p].[ProjectId], [p].[Objective], [p].[StartDate], [p].[EndDate],
                [a].[ArtifactId], [a].[Name] AS [ArtifactName], [a].[OriginDate], [a].[InstitutionId],
                [i].[Name] AS [InstitutionName], [i].[FoundedYear]
    FROM [Preservation_Project] AS [p]
    JOIN [Artifact] AS [a] ON [a].[ArtifactId] = [p].[ArtifactId]
    JOIN [Institution] AS [i] ON [i].[InstitutionId] = [a].[InstitutionId]
    WHERE [p].[ProjectId] = @projectId", connection);
        command.Parameters.AddWithValue("@projectId", projectId);

        using var reader = await command.ExecuteReaderAsync();
        
        ProjectResponseDTO? project = null;
        
        while (await reader.ReadAsync())
        {
            if (project == null)
            {
                project = new ProjectResponseDTO
                {
                    ProjectId = reader.GetInt32(reader.GetOrdinal("ProjectId")),
                    Objective = reader.GetString(reader.GetOrdinal("Objective")),
                    StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                    EndDate = reader.IsDBNull(3) ? null : reader.GetDateTime(reader.GetOrdinal("EndDate")),
                    Artifact = new ArtifactDTO
                    {
                        Name = reader.GetString(reader.GetOrdinal("ArtifactName")),
                        OriginDate = reader.GetDateTime(reader.GetOrdinal("OriginDate")),
                        Institution = new InstitutionDTO
                        {
                            InstitutionId = reader.GetInt32(reader.GetOrdinal("InstitutionId")),
                            Name = reader.GetString(reader.GetOrdinal("InstitutionName")),
                            FoundedYear = reader.GetInt32(reader.GetOrdinal("FoundedYear"))
                        }
                    },
                    StaffAssignments = new List<StaffAssignmentDTO>()
                };
            }
            
            var staff = new SqlCommand(@"
        SELECT
                [s].[FirstName], [s].[LastName], [s].[HireDate], [sa].[Role]
        FROM [Staff_Assignment] AS [sa]
        JOIN [Staff] AS [s] ON [s].[StaffId] = [sa].[StaffId]
        WHERE [sa].[ProjectId] = @projectId", connection);
            staff.Parameters.AddWithValue("@projectId", projectId);

            using var staffReader = await staff.ExecuteReaderAsync();
            while (await staffReader.ReadAsync())
            {
                project.StaffAssignments.Add(new StaffAssignmentDTO
                {
                    FirstName = staffReader.GetString(reader.GetOrdinal("FirstName")),
                    LastName = staffReader.GetString(reader.GetOrdinal("LastName")),
                    HireDate = staffReader.GetDateTime(reader.GetOrdinal("HireDate")),
                    Role = staffReader.GetString(reader.GetOrdinal("Role"))
                });
            }
        }

        return project;
    }

    public async Task<bool> AddArtifactAndProjectAsync(NewArtifactWithProjectDTO request)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            var insertArtifactCmd = new SqlCommand(@"
        INSERT INTO [Artifact] ([ArtifactId], [Name], [OriginDate], [InstitutionId])
        VALUES (@id, @name, @originDate, @institutionId)", connection, transaction);
            insertArtifactCmd.Parameters.AddWithValue("@id", request.Artifact.ArtifactId);
            insertArtifactCmd.Parameters.AddWithValue("@name", request.Artifact.Name);
            insertArtifactCmd.Parameters.AddWithValue("@originDate", request.Artifact.OriginDate);
            insertArtifactCmd.Parameters.AddWithValue("@institutionId", request.Artifact.InstitutionId);
            await insertArtifactCmd.ExecuteNonQueryAsync();
            
            var insertProjectCmd = new SqlCommand(@"
        INSERT INTO [Preservation_Project] ([ProjectId], [Objective], [StartDate], [EndDate], [ArtifactId])
        VALUES (@id, @objective, @startDate, @endDate, @artifactId)", connection, transaction);
            insertProjectCmd.Parameters.AddWithValue("@id", request.Project.ProjectId);
            insertProjectCmd.Parameters.AddWithValue("@objective", request.Project.Objective);
            insertProjectCmd.Parameters.AddWithValue("@startDate", request.Project.StartDate);
            insertProjectCmd.Parameters.AddWithValue("@endDate",
                (object?)request.Project.EndDate ?? DBNull.Value);
            insertProjectCmd.Parameters.AddWithValue("@artifactId", request.Artifact.ArtifactId);
            await insertProjectCmd.ExecuteNonQueryAsync();

            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
        
    }
}


