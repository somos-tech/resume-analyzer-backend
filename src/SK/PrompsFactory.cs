using Microsoft.SemanticKernel;

namespace SomosTech.ResumeExtractor.SK
{
    public class PrompsFactory : IPromptsFactory
    {
        private readonly Kernel kernel;

        private KernelPlugin? prompts;

        public PrompsFactory(Kernel kernel)
        {
            this.kernel = kernel;
            LoadPrompts();
        }

        private void LoadPrompts()
        {
            this.prompts = kernel.CreatePluginFromPromptDirectory("Prompts");
        }

        public KernelFunction GetResumeAnalyzerPrompt(string version = "v1")
        {
            return prompts![$"resume_analyzer_{version}"];
        }

        public KernelFunction GetResumeTipsPrompt(string version = "v1")
        {
            return prompts![$"resume_tips_{version}"];
        }
    }
}