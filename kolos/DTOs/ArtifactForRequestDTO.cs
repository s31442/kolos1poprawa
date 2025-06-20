using System.ComponentModel.DataAnnotations;

namespace kolos.DTOs;

public class ArtifactForRequestDTO
{
    [Required]
    public int ArtifactId { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public DateTime OriginDate { get; set; }
    [Required]
    public int InstitutionId { get; set; }
    
}