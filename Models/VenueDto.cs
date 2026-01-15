namespace Jarvis.Web.Models;

public class VenueDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Acronym { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
    public string Keywords { get; set; } = string.Empty;
    public List<string> Tracks { get; set; } = new();

    public bool IsExpanded { get; set; } = false;
}