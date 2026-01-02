using System.ComponentModel.DataAnnotations;

namespace Jarvis.Web.Models;

// --- 1. Başvuru OLUŞTURMA Modeli (Mevcut Olan) ---
public class SubmissionModel
{
    public Guid VenueId { get; set; }
    public Guid VenueEditionId { get; set; }
    public Guid CallForPapersId { get; set; }

    [Required(ErrorMessage = "Please select a track")]
    public Guid TrackId { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Abstract { get; set; } = string.Empty;

    [Required]
    public string Keywords { get; set; } = string.Empty;

    public int Type { get; set; }

    // Etik Beyanlar
    public bool IsOriginal { get; set; }
    public bool IsNotElsewhere { get; set; }
    public bool HasConsent { get; set; }

    public List<AuthorModel> Authors { get; set; } = new();
}

// --- 2. Başvuru DETAY GÖRÜNTÜLEME Modelleri (Yeni Eklenenler) ---
public class SubmissionDetailModel
{
    public Guid Id { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Abstract { get; set; } = string.Empty;
    public string Keywords { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    // AuthorModel'i yukarıda zaten tanımlı olduğu için tekrar kullanabiliriz
    public List<AuthorModel> Authors { get; set; } = new();
    public List<FileModel> Files { get; set; } = new();
}

public class FileModel
{
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}