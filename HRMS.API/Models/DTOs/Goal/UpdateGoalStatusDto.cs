using System.ComponentModel.DataAnnotations;

public class UpdateGoalStatusDto
{
    [Required]
    public string Status { get; set; }
        = string.Empty;
}