using Microsoft.SemanticKernel;
using System.Text.Json;
using SomosTech.ResumeExtractor.Extensions;

namespace SomosTech.ResumeExtractor.SK
{
    public class ResumeAnalyzerSkill
    {
        private readonly IPromptsFactory _promptsFactory;

        private readonly Kernel _kernel;

        public ResumeAnalyzerSkill(Kernel kernel, IPromptsFactory promptsFactory)
        {
            _promptsFactory = promptsFactory;
            _kernel = kernel;
        }

        public async Task<dynamic?> GetResume(string rawResumeContent, string analyzerVersion = "v1")
        {
            var formatResume = _promptsFactory.GetResumeAnalyzerPrompt(analyzerVersion);

            var context = new KernelArguments
            {
                { "ResumeContent", rawResumeContent! }
            };

            var result = await formatResume.InvokeAsync(_kernel, context);

            if (!result.ToString().IsValidJson())
            {
                throw new Exception("Invalid prompt result, not valid json");
            }

            var resume = JsonSerializer.Deserialize<dynamic>(result.ToString(), new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            return resume;
        }
    }
}
