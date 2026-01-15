namespace Jarvis.Web.Models;

public class ReviewAssignmentModel
{
    public Guid AssignmentId { get; set; }
    public Guid SubmissionId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime DueAt { get; set; }
    public string Title { get; set; } = "Manuscript Review Task";
}

public class ReviewAssignmentDetailDto
{
    public Guid AssignmentId { get; set; }
    public Guid SubmissionId { get; set; }
    public Guid VenueId { get; set; }
    public string Status { get; set; } = string.Empty;
}