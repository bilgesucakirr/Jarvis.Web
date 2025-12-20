namespace Jarvis.Web.Models;

public class SubmissionStatsModel
{
    public Guid Id { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int ReviewersAssignedCount { get; set; }
    public int ReviewsCompletedCount { get; set; }
    public DateTime CreatedAt { get; set; }
}