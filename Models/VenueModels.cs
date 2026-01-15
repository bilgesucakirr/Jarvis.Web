using System.ComponentModel.DataAnnotations;

namespace Jarvis.Web.Models;

public class CreateVenueModel
{
    [Required(ErrorMessage = "Venue name is required.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Acronym is required (e.g. ICAI).")]
    public string Acronym { get; set; } = string.Empty;

    public string Type { get; set; } = "Conference";

    [Required(ErrorMessage = "Aim & Scope is required.")]
    public string AimAndScope { get; set; } = string.Empty;

    [Required(ErrorMessage = "Keywords are required.")]
    public string Keywords { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string OrganizerEmail { get; set; } = string.Empty;

    // Dosya yüklendikten sonra dönen URL buraya yazılır
    public string? ReviewFormUrl { get; set; }

    // UI tarafında yönetilen dinamik track listesi
    public List<string> Tracks { get; set; } = new() { "General Track" };
}

public class InviteReviewerModel
{
    public Guid SubmissionId { get; set; }
    public Guid ReviewerUserId { get; set; }
    public string ReviewerEmail { get; set; } = string.Empty;
    public string SubmissionTitle { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
}