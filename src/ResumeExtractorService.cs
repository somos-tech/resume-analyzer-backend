using SomosTech.ResumeExtractor.SK;

namespace SomosTech.ResumeExtractor
{
    public class ResumeExtractorService
    {
        private readonly DocumentAnalyzer documentAnalyzer;

        private readonly ResumeAnalyzerSkill resumeAnalyzerSkill;

        public ResumeExtractorService(DocumentAnalyzer documentAnalyzer, ResumeAnalyzerSkill resumeAnalyzerSkill)
        {
            this.documentAnalyzer = documentAnalyzer;
            this.resumeAnalyzerSkill = resumeAnalyzerSkill;
        }

        public async Task<dynamic> ProcessResume(string base64Content, string version = "v1")
        {
            byte[] bytes = Convert.FromBase64String(base64Content);

            var resumeContent = await documentAnalyzer.AnalyzeDocument(BinaryData.FromBytes(bytes));

            var resume = await resumeAnalyzerSkill.GetResume(resumeContent, version);

            return resume!;
        }
    }
}