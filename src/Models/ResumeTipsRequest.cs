namespace SomosTech.ResumeExtractor.Models
{
    public class ResumeTipsRequest
    {
        public string ResumeTextContent { get; set; } = default!;

        public string TipsVersion = "v1";

        public string Role { get; set; } = "software engineer";
    }
}