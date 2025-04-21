using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using ChatBotAPI.Models;

namespace ChatBotAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatBotController : Controller
    {
        private readonly HttpClient _httpClient;
        private static Dictionary<Guid, List<string>> _messageStore = new Dictionary<Guid, List<string>>();

        public ChatBotController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendStringToOtherEndpoint([FromBody] ChatMessage data)
        {
            if (string.IsNullOrWhiteSpace(data.Message))
                return BadRequest("The message is empty.");

            if (!Guid.TryParse(data.GuidId, out Guid messageGuid))
                return BadRequest("Invalid GUID.");

            string messageContent = data.Message;

            // === 1. Embedding vstupu ===
            var embedPayload = new { model = "bge-m3", input = messageContent };
            var embedResponse = await _httpClient.PostAsJsonAsync("http://host.docker.internal:11435/api/embed", embedPayload);
            if (!embedResponse.IsSuccessStatusCode)
                return StatusCode((int)embedResponse.StatusCode, $"Error when embedding input.");

            var embedJson = await embedResponse.Content.ReadAsStringAsync();
            using var embedDoc = JsonDocument.Parse(embedJson);
            var embeddingsElement = embedDoc.RootElement.GetProperty("embeddings");

            // We suppose we are interested in the first embeddings field
            var vector = embeddingsElement[0]
                .EnumerateArray()
                .Select(x => x.GetSingle())
                .ToArray();

            // === 2. Searching in Qdrant ===
            var searchPayload = new
            {
                vector = vector,
                limit = 2,
                with_payload = true
            };

            var searchRes = await _httpClient.PostAsJsonAsync("http://host.docker.internal:6333/collections/dotted/points/search", searchPayload);
            if (!searchRes.IsSuccessStatusCode)
                return StatusCode((int)searchRes.StatusCode, "Error when searching in Qdrant.");
            /*var searchRes = await qdrant.SearchAsync(
                collectionName: "memory",
                vector: vector,
                searchParams: new SearchParams { Exact = false },
                limit: 5
            );*/

            /*var searchRes = await qdrant.QueryAsync(
                collectionName: "{memory}",
                query: vector,
                payloadSelector: true,
                vectorsSelector: true,
                limit: 5
                collectionName: "{dotted}",
                query: vector,
                searchParams: new SearchParams { Exact = false, HnswEf = 128 },
                limit: 3
            );*/
            Console.WriteLine(searchRes);
            var searchJson = await searchRes.Content.ReadAsStringAsync();
            using var searchDoc = JsonDocument.Parse(searchJson);
            var matches = searchDoc.RootElement
                .GetProperty("result")
                .EnumerateArray()
                .Select(hit => hit.GetProperty("payload").GetProperty("text").GetString())
                .ToList();

            // === 3. Compiling messages for LLM ====
            if (!_messageStore.ContainsKey(messageGuid))
                _messageStore[messageGuid] = new List<string>();
            _messageStore[messageGuid].Add(messageContent); // adding an input

            var messages = new List<object>();

            // Context from the knowledge base
            foreach (var match in matches)
            {
                messages.Add(new { role = "system", content = $"Relevant knowledge: {match}" });
                Console.WriteLine(match);
            }

            // Message history
            messages.AddRange(_messageStore[messageGuid]
                .Select((msg, index) => new
                {
                    role = index % 3 == 0 ? "system" : index % 2 == 0 ? "user" : "assistant",
                    content = msg
                }));

            var payload = new
            {
                model = "ai_chatbot",
                messages = messages,
                stream = false
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("http://host.docker.internal:11434/api/chat", content);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var chatResponse = JsonSerializer.Deserialize<AIAssist>(responseBody, options);

                if (chatResponse?.Message != null)
                {
                    _messageStore[messageGuid].Add(chatResponse.Message.Content);
                }

                return Ok(responseBody);
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, $"Error: {error}");
            }
        }

        [HttpPost("embed-txt-files")]
        public async Task<IActionResult> EmbedTxtFilesIndividually()
        {
            string folderPath = "ScrappedData/";
            string embeddingUrl = "http://host.docker.internal:11435/api/embed";
            string qdrantUrl = "http://host.docker.internal:6333";
            string collectionName = "dotted";

            if (!Directory.Exists(folderPath))
                return NotFound("Folder does not exist.");

            var txtFiles = Directory.GetFiles(folderPath, "*.txt");
            if (txtFiles.Length == 0)
                return BadRequest("There are no .txt files in the folder.");

            var http = new HttpClient();


            var checkRes = await http.GetAsync($"{qdrantUrl}/collections/{collectionName}");
            if (!checkRes.IsSuccessStatusCode)
            {
                var createCollectionPayload = new
                {
                    vectors = new
                    {
                        size = 1024,
                        distance = "Dot"
                    }
                };

                var createContent = JsonContent.Create(createCollectionPayload);
                var createRes = await http.PutAsync($"{qdrantUrl}/collections/{collectionName}", createContent);

                if (!createRes.IsSuccessStatusCode)
                {
                    var err = await createRes.Content.ReadAsStringAsync();
                    return StatusCode((int)createRes.StatusCode, $"Error while creating a collection: {err}");
                }
            }

            int idCounter = 0;

            foreach (var file in txtFiles)
            {
                var text = await System.IO.File.ReadAllTextAsync(file);

                // Send to embedding model
                var embedPayload = new { model = "bge-m3", input = text };
                var embedRes = await http.PostAsJsonAsync(embeddingUrl, embedPayload);

                if (!embedRes.IsSuccessStatusCode)
                    return StatusCode((int)embedRes.StatusCode, $"Error while embedding a file: {file}");

                var resJson = await embedRes.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(resJson);

                if (!doc.RootElement.TryGetProperty("embeddings", out var embeddingsElement))
                    return StatusCode(500, "The response from the embedding model does not contain 'embeddings'.");

                if (embeddingsElement.GetArrayLength() == 0)
                    return StatusCode(500, "The response contains an empty 'embeddings' field.");

                var vector = embeddingsElement[0].EnumerateArray().Select(x => x.GetSingle()).ToArray();

                // Save to Qdrant
                var upsertPayload = new
                {
                    points = new[]
                    {
                        new
                        {
                            id = idCounter,
                            vector = vector,
                            payload = new { text = text }
                        }
                    }
                };
                var upsertContent = JsonContent.Create(upsertPayload);
                var upsertRes = await http.PutAsync($"{qdrantUrl}/collections/{collectionName}/points", upsertContent);

                if (!upsertRes.IsSuccessStatusCode)
                    return StatusCode((int)upsertRes.StatusCode, $"Error when saving to Qdrant (file {file})");

                idCounter++;
            }

            return Ok("All files were successfully embedded and saved to Qdrant.");
        }
    }
}
