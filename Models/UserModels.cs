namespace Jarvis.Web.Models;

public class UserProfileDto
{
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Interests { get; set; } = string.Empty;
    public IList<string> Roles { get; set; } = new List<string>();
}

public class UpdateProfileRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Interests { get; set; } = string.Empty;
}

public class UserListDto
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public IList<string> Roles { get; set; } = new List<string>();
    public string Interests { get; set; } = string.Empty;
}

public class AssignRoleRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class ReviewerDto
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Interests { get; set; } = string.Empty;
}