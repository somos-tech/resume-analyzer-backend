namespace SomosTech.ResumeExtractor.Models
{
    public class ResumeRequest
    {
        public string Base64Content { get; set; } = default!;

        public string AnalyzerVersion = "v1";
    }
}