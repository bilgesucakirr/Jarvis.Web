using System.ComponentModel.DataAnnotations;

namespace Jarvis.Web.Models;

public class SubmissionModel
{
    public Guid VenueId { get; set; }
    public Guid VenueEditionId { get; set; }
    public Guid CallForPapersId { get; set; }

    [Required(ErrorMessage = "Please select a track")]
    public Guid TrackId { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Abstract { get; set; } = string.Empty;

    [Required]
    public string Keywords { get; set; } = string.Empty;

    public int Type { get; set; } 

    // Etik Beyanlar
    public bool IsOriginal { get; set; }
    public bool IsNotElsewhere { get; set; }
    public bool HasConsent { get; set; }

    public List<AuthorModel> Authors { get; set; } = new();
}