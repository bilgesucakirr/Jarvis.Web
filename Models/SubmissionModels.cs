using System.ComponentModel.DataAnnotations;

namespace Jarvis.Web.Models;

public class SubmissionModel
{
    public Guid VenueId { get; set; }
    public Guid VenueEditionId { get; set; }
    public Guid CallForPapersId { get; set; }

    [Required(ErrorMessage = "Please select a track")]
    public Guid TrackId { get; set; }

    [Required(ErrorMessage = "Title is required")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Abstract is required")]
    public string Abstract { get; set; } = string.Empty;

    [Required(ErrorMessage = "At least one keyword is required")]
    public string Keywords { get; set; } = string.Empty;

    public int Type { get; set; }

    public string? OrganizerEmail { get; set; }

    public bool IsOriginal { get; set; }
    public bool IsNotElsewhere { get; set; }
    public bool HasConsent { get; set; }

    public string SubmitterEmail { get; set; } = string.Empty;
    public string SubmitterName { get; set; } = string.Empty;

    public List<AuthorModel> Authors { get; set; } = new();
}

public class SubmissionDetailModel
{
    public Guid Id { get; set; }
    public Guid VenueId { get; set; }

    public string ReferenceNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Abstract { get; set; } = string.Empty;
    public string Keywords { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public List<AuthorModel> Authors { get; set; } = new();
    public List<FileModel> Files { get; set; } = new();
}

public class FileModel
{
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}