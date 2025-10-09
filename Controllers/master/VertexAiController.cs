using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Google.Apis.Auth.OAuth2;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace TracePca.Controllers.master
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromptController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _projectId;
        private readonly string _model;
        private readonly string _serviceAccountPath;

        public PromptController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;

            _projectId = _configuration["GoogleVertexAI:ProjectId"];
            _model = _configuration["GoogleVertexAI:Model"];
            _serviceAccountPath = _configuration["GoogleVertexAI:ServiceAccountKeyFilePath"];
        }

        /// <summary>
        /// Main endpoint to handle user prompts.
        /// Checks database, then JSON files, then default response.
        /// Optional fallback to Vertex AI Gemini.
        /// </summary>
        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] UserPromptRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Prompt))
                return BadRequest("Prompt cannot be empty.");

            // 1. Try to get response from database
            string dbResponse = await GetResponseFromDatabase(request.Prompt);
            if (!string.IsNullOrEmpty(dbResponse))
                return Ok(new { response = dbResponse });

            // 2. Try to get response from JSON files
            string fileResponse = GetResponseFromFile(request.Prompt);
            if (!string.IsNullOrEmpty(fileResponse))
                return Ok(new { response = fileResponse });

            // 3. Default fallback response
            string defaultResponse = "Sorry, I could not find an answer.";

            // 4. Optional: call Vertex AI if no match found
            if (request.UseVertexAIFallback)
            {
                string aiResponse = await AskVertexAI(request.Prompt);
                if (!string.IsNullOrEmpty(aiResponse))
                    return Ok(new { response = aiResponse });
            }

            return Ok(new { response = defaultResponse });
        }

        /// <summary>
        /// Fetch answer from database (example using async Task)
        /// Replace with your actual database query logic
        /// </summary>
        private async Task<string> GetResponseFromDatabase(string prompt)
        {
            // TODO: implement database query
            // Example: SELECT Answer FROM FAQ WHERE Question LIKE '%prompt%'
            await Task.Delay(10); // simulate async DB call
            return null; // return actual answer if found
        }

        /// <summary>
        /// Fetch answer from JSON file(s)
        /// </summary>
        private string GetResponseFromFile(string prompt)
        {
            try
            {
                // Example: JSON file path
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "faq.json");
                if (!System.IO.File.Exists(filePath)) return null;

                var jsonData = System.IO.File.ReadAllText(filePath);
                var faqList = JsonConvert.DeserializeObject<List<FAQItem>>(jsonData);

                var match = faqList
                    .FirstOrDefault(f => f.Question.Contains(prompt, System.StringComparison.OrdinalIgnoreCase));

                return match?.Answer;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Call Vertex AI Gemini 2.5 Flash Lite (text only)
        /// </summary>
        private async Task<string> AskVertexAI(string prompt)
        {
            try
            {
                // Authenticate using service account JSON
                var credential = GoogleCredential.FromFile(_serviceAccountPath)
                    .CreateScoped("https://www.googleapis.com/auth/cloud-platform");

                var accessToken = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                // Correct endpoint for Gemini 2.5 Flash Lite
                var uri = $"https://{_configuration["GoogleVertexAI:Location"]}-aiplatform.googleapis.com/v1/projects/{_projectId}/locations/{_configuration["GoogleVertexAI:Location"]}/models/{_model}/gemini-1.5-flash-001:generateContent";

                // Request body
                var requestBody = new
                {
                    instances = new[]
                    {
                        new { content = prompt }  // Wrap prompt in 'content'
                    },
                    parameters = new
                    {
                        temperature = 0.2,
                        maxOutputTokens = 1024
                    }
                };

                var jsonContent = JsonConvert.SerializeObject(requestBody);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(uri, httpContent);
                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    return $"Vertex AI Error: {errorResponse}";
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                dynamic responseObject = JsonConvert.DeserializeObject(responseContent);

                // Extract generated text
                string generatedText = responseObject?.predictions?[0]?.content ?? null;
                return generatedText;
            }
            catch (System.Exception ex)
            {
                return $"Vertex AI Exception: {ex.Message}";
            }
        }

        /// <summary>
        /// Request DTO
        /// </summary>
        public class UserPromptRequest
        {
            public string Prompt { get; set; }              // User text input
            public bool UseVertexAIFallback { get; set; }   // Whether to fallback to Vertex AI if no match
        }

        /// <summary>
        /// JSON file FAQ structure
        /// </summary>
        public class FAQItem
        {
            public string Question { get; set; }
            public string Answer { get; set; }
        }
    }
}
