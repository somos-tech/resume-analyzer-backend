using Microsoft.SemanticKernel;
using System.Text.Json;
using SomosTech.ResumeExtractor.Extensions;

namespace SomosTech.ResumeExtractor.SK
{
    public class ResumeTipsSkill
    {
        private readonly IPromptsFactory _promptsFactory;

        private readonly Kernel _kernel;

        public ResumeTipsSkill(Kernel kernel, IPromptsFactory promptsFactory)
        {
            _promptsFactory = promptsFactory;
            _kernel = kernel;
        }

        public async Task<IList<dynamic>?> GetTips(string resumeContent, string jobRole, string analyzerVersion = "v1")
        {
            var formatResume = _promptsFactory.GetResumeTipsPrompt(analyzerVersion);

            var context = new KernelArguments
            {
                { "ResumeContent", resumeContent! },
                { "Role", jobRole! },
            };

            var result = await formatResume.InvokeAsync(_kernel, context);

            if (!result.ToString().IsValidJson())
            {
                throw new Exception("Invalid prompt result, not valid json");
            }

            var tips = JsonSerializer.Deserialize<IList<dynamic>>(result.ToString(), new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            return tips;
        }
    }
}
