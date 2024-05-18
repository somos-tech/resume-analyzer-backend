using Microsoft.SemanticKernel;
using Microsoft.AspNetCore.OpenApi;
using SomosTech.ResumeExtractor.Middleware;
using SomosTech.ResumeExtractor;
using SomosTech.ResumeExtractor.Models;
using SomosTech.ResumeExtractor.SK;
using Azure.AI.DocumentIntelligence;
using Azure;
using Azure.Storage.Blobs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton((sp) =>
{
    var builder = Kernel.CreateBuilder();

    //Get this from the appsettings.json
    var configuration = sp.GetRequiredService<IConfiguration>();

    builder.AddAzureOpenAIChatCompletion(
                configuration["AZURE_OPENAI_MODEL"]!,
                configuration["AZURE_OPENAI_ENDPOINT"]!,
                configuration["AZURE_OPENAI_KEY"]!);

    return builder.Build();
}).AddSingleton((sp) =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();

    var credential = new AzureKeyCredential(configuration["AZURE_DOCUMENTAI_KEY"]!);

    return new DocumentIntelligenceClient(new Uri(configuration["AZURE_DOCUMENTAI_ENDPOINT"]!), credential);
}).AddSingleton<IPromptsFactory>((sp) =>
{
    var kernel = sp.GetRequiredService<Kernel>();

    return new PrompsFactory(kernel);
}).AddSingleton((sp) => {
    var kernel = sp.GetRequiredService<Kernel>();
    var promptsFactory = sp.GetRequiredService<IPromptsFactory>();

    return new ResumeAnalyzerSkill(kernel, promptsFactory);
}).AddSingleton((sp) => {
    var kernel = sp.GetRequiredService<Kernel>();
    var promptsFactory = sp.GetRequiredService<IPromptsFactory>();

    return new ResumeTipsSkill(kernel, promptsFactory);
}).AddSingleton((sp) => {
    var client = sp.GetRequiredService<DocumentIntelligenceClient>();

    return new DocumentAnalyzer(client);
}).AddSingleton((sp) =>
{
    var documentAnalyzer = sp.GetRequiredService<DocumentAnalyzer>();
    var resumeAnalyzerSkill = sp.GetRequiredService<ResumeAnalyzerSkill>();

    return new ResumeExtractorService(documentAnalyzer, resumeAnalyzerSkill);
}).AddSingleton((sp) =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();

    return new ResumeUploaderService(configuration["PREVIEW_STORAGE_CONNECTION_STRING"]!, "preview");
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ApiKeyValidator>();

app.MapGet("/", () => "Hello World!")
    .WithName("GetHelloWorld")
    .WithOpenApi();

app.MapPost("/analyze", async (ResumeExtractorService resumeExtractorService, ResumeAnalysisRequest request) =>
{
    var result = await resumeExtractorService.ProcessResume(request.Base64Content!, request.AnalyzerVersion);

    return result;
}).WithName("AnalyzeResume").WithOpenApi();

app.MapPost("/tips", async (ResumeTipsSkill skill, ResumeTipsRequest request) =>
{
    var result = await skill.GetTips(request!.ResumeTextContent, request!.Role);

    return result;
}).WithName("Tips").WithOpenApi();

app.MapPost("/upload-resume", async (ResumeUploaderService uploaderService, ResumeUploadRequest request) =>
{
    if (string.IsNullOrEmpty(request.User) || string.IsNullOrEmpty(request.Id))
    {
        throw new BadHttpRequestException("User and Id are required");
    }

    var fileName = request.User + "_" + request.Id;

    var result = await uploaderService.UploadBase64ResumeAsync(request.Base64Content, fileName, "pdf");

    return result;
}).WithName("").WithOpenApi();

app.Run();