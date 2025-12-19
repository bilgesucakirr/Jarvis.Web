namespace Jarvis.Web.Models;

public class VenueDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Acronym { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}