using System.Text;
using System.Text.Json;

namespace DocRiskScanner.Services
{
    public class RiskIssue
    {
        public string Clause { get; set; } = "";
        public string RiskLevel { get; set; } = "";
        public string Explanation { get; set; } = "";
    }

    public class RiskAnalysisResult
    {
        public int RiskScore { get; set; }
        public string Summary { get; set; } = "";
        public List<RiskIssue> Issues { get; set; } = new();
    }

    public class GroqService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GroqService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _apiKey = config["Groq:ApiKey"] ?? throw new Exception("Groq API key not found. Did you set up User Secrets?");
        }

        public async Task<RiskAnalysisResult> AnalyzeDocumentAsync(string documentText)
        {
            if (documentText.Length > 8000)
            {
                documentText = documentText.Substring(0, 8000);
            }

            var prompt = $@"You are a document risk analyzer. Analyze the following document text and identify risky, ambiguous, or non-standard clauses.

Respond ONLY with valid JSON in exactly this format, no extra text, no markdown formatting:
{{
  ""riskScore"": <number 0-100, higher means riskier>,
  ""summary"": ""<2-3 sentence overview>"",
  ""issues"": [
    {{ ""clause"": ""<short quote or description, max 20 words>"", ""riskLevel"": ""High"", ""explanation"": ""<plain English explanation, 1-2 sentences>"" }}
  ]
}}

Document text:
{documentText}";

            var requestBody = new
            {
                model = "llama-3.3-70b-versatile",
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                temperature = 0.3
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.groq.com/openai/v1/chat/completions")
            {
                Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Bearer {_apiKey}");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseString);
            var content = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? "";

            content = content.Trim();
            if (content.StartsWith("```"))
            {
                content = content.Substring(content.IndexOf('\n') + 1);
                content = content.Substring(0, content.LastIndexOf("```"));
            }

            var result = JsonSerializer.Deserialize<RiskAnalysisResult>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result ?? new RiskAnalysisResult { Summary = "Could not analyze document." };
        }
    }
}