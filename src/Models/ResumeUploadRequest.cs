namespace SomosTech.ResumeExtractor.Models
{
    public class ResumeUploadRequest
    {
        public string Base64Content { get; set; } = default!;

        public string Id { get; set; } = default!;

        public string User { get; set; } = default!;
    }
}