public class Image
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? FileName { get; set; }
       public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ImageTag> ImageTags { get; set; } = new List<ImageTag>();
}