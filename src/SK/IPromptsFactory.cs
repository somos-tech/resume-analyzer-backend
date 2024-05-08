using Microsoft.SemanticKernel;

namespace SomosTech.ResumeExtractor.SK
{
    public interface IPromptsFactory
    {
        KernelFunction GetResumeAnalyzerPrompt(string version = "v1");
    }
}
