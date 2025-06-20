using System.ComponentModel.DataAnnotations;

namespace kolos.DTOs;

public class ProjectForRequestDTO
{
    [Required]
    public int ProjectId { get; set; }
    [Required]
    public string Objective { get; set; }
    [Required]
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}