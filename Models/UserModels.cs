using System.ComponentModel.DataAnnotations;

namespace Jarvis.Web.Models;

public class UserProfileDto
{
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Interests { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; } // YENİ
    public IList<string> Roles { get; set; } = new List<string>();
    public string? Affiliation { get; set; }
    public string? Country { get; set; }
    public string? Biography { get; set; }
    public string? Title { get; set; }
}

public class UpdateProfileRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Interests { get; set; } = string.Empty;
    public string? Affiliation { get; set; }
    public string? Country { get; set; }
    public string? Biography { get; set; }
    public string? Title { get; set; }
}

public class UserListDto
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public IList<string> Roles { get; set; } = new List<string>();
    public string Interests { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; } // YENİ
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

public class AreaOfInterestDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class ChangePasswordModel
{
    [Required]
    public string CurrentPassword { get; set; } = string.Empty;
    [Required, MinLength(6)]
    public string NewPassword { get; set; } = string.Empty;
    [Required, Compare(nameof(NewPassword))]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}