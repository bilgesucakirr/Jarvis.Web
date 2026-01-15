namespace Jarvis.Web.Models;

public class VenueDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ReviewFormUrl { get; set; }


    public string? OrganizerEmail { get; set; }

    public string Keywords { get; set; } = string.Empty;
    public Guid ActiveEditionId { get; set; }
    public Guid ActiveCfpId { get; set; }
    public List<TrackDto> Tracks { get; set; } = new();
}