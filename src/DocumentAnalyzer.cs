using Azure;
using Azure.AI.DocumentIntelligence;

namespace SomosTech.ResumeExtractor
{
    public class DocumentAnalyzer
    {
        private readonly DocumentIntelligenceClient documentIntelligenceClient;

        public DocumentAnalyzer(DocumentIntelligenceClient documentIntelligenceClient)
        {
            this.documentIntelligenceClient = documentIntelligenceClient;
        }

        public async Task<string> AnalyzeDocument(BinaryData content)
        {
            var analyzeDocumentContent = new AnalyzeDocumentContent
            {
                Base64Source = content
            };

            Operation<AnalyzeResult> operation = await documentIntelligenceClient.AnalyzeDocumentAsync(WaitUntil.Completed, "prebuilt-layout", analyzeDocumentContent);

            AnalyzeResult result = operation.Value;

            var resumeContent = string.Empty;

            for (int i = 0; i < result.Paragraphs.Count; i++)
            {
                DocumentParagraph paragraph = result.Paragraphs[i];

                resumeContent += $"\n{paragraph.Role} {paragraph.Content}";
            }

            return resumeContent;
        }
    }
}
