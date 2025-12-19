public class SubmitReviewModel
{
    public decimal OverallScore { get; set; }
    public int Confidence { get; set; }
    public string CommentsToAuthor { get; set; } = string.Empty;
    public string? CommentsToEditor { get; set; }
}