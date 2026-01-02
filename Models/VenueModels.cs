using System.ComponentModel.DataAnnotations;

namespace Jarvis.Web.Models;

public class CreateVenueModel
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Acronym { get; set; } = string.Empty;

    [Required]
    public string Type { get; set; } = "Conference";

    [Required]
    public string Description { get; set; } = string.Empty;
}

public class InviteReviewerModel
{
    public Guid SubmissionId { get; set; }
    public Guid ReviewerUserId { get; set; }
    public DateTime? DueDate { get; set; }
}