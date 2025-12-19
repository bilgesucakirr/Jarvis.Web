namespace Jarvis.Web.Models;

public class AuthorModel
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Affiliation { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public bool IsCorresponding { get; set; }
}