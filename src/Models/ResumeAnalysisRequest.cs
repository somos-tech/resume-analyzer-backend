namespace SomosTech.ResumeExtractor.Models
{
    public class ResumeAnalysisRequest
    {
        public string Base64Content { get; set; } = default!;

        public string AnalyzerVersion = "v1";
    }
}