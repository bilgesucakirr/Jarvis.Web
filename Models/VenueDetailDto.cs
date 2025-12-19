namespace Jarvis.Web.Models;

public class VenueDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid ActiveEditionId { get; set; }
    public Guid ActiveCfpId { get; set; }
    public List<TrackDto> Tracks { get; set; } = new();
}