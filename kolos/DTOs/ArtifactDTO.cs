namespace kolos.DTOs;

public class ArtifactDTO
{
    public int ArtifactId { get; set; }
    public string Name { get; set; }
    public DateTime OriginDate { get; set; }
    public int InstitutionId { get; set; }
    public InstitutionDTO Institution { get; set; }
}