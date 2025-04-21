using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System;
using WebApplication1.Data;
using Microsoft.AspNetCore.Identity;
using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private static string _receivedString;
        private static QdrantClient qdrant = new QdrantClient("127.0.0.1", 6333);
        private static Dictionary<Guid, List<string>> _messageStore = new Dictionary<Guid, List<string>>();

        public WeatherForecastController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet("receive")]
        public async Task<IActionResult> ReceiveString([FromQuery] string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return BadRequest("Zadaný vstup je prázdný.");

            _receivedString = input;

            return Ok($"String pøijat: {_receivedString}");
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendStringToOtherEndpoint([FromBody] ChatMessage data)
        {
            if (string.IsNullOrWhiteSpace(data.Message))
                return BadRequest("Zpráva je prázdná.");

            if (!Guid.TryParse(data.guidId, out Guid messageGuid))
                return BadRequest("Neplatné GUID.");

            string messageContent = data.Message;

            // === 1. Embedding vstupu ===
            var embedPayload = new { model = "bge-m3", input = messageContent };
            var embedResponse = await _httpClient.PostAsJsonAsync("http://192.168.153.186:11434/api/embed", embedPayload);
            if (!embedResponse.IsSuccessStatusCode)
                return StatusCode((int)embedResponse.StatusCode, $"Chyba pøi embedování vstupu.");

            var embedJson = await embedResponse.Content.ReadAsStringAsync();
            using var embedDoc = JsonDocument.Parse(embedJson);
            var embeddingsElement = embedDoc.RootElement.GetProperty("embeddings");

            // Pøedpokládáme, že nás zajímá první pole embeddings
            var vector = embeddingsElement[0]
                .EnumerateArray()
                .Select(x => x.GetSingle())
                .ToArray();

            // === 2. Vyhledání v Qdrant ===
            var searchPayload = new
            {
                vector = vector,
                limit = 2,
                with_payload = true
            };

            var searchRes = await _httpClient.PostAsJsonAsync("http://localhost:6333/collections/dotted/points/search", searchPayload);
            if (!searchRes.IsSuccessStatusCode)
                return StatusCode((int)searchRes.StatusCode, "Chyba pøi vyhledávání v Qdrantu.");
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

            // === 3. Sestavení zpráv pro LLM ===
            if (!_messageStore.ContainsKey(messageGuid))
                _messageStore[messageGuid] = new List<string>();
            _messageStore[messageGuid].Add(messageContent); // pøidání vstupu

            var messages = new List<object>();

            // Kontext ze znalostní databáze
            foreach (var match in matches)
            {
                messages.Add(new { role = "system", content = $"Relevantní znalost: {match}" });
                Console.WriteLine(match);
            }

            // Historie zpráv
            messages.AddRange(_messageStore[messageGuid]
                .Select((msg, index) => new
                {
                    role = index % 2 == 0 ? "user" : "assistant",
                    content = msg
                }));

            var payload = new
            {
                model = "ideathon",
                messages = messages,
                stream = false
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("http://192.168.153.186:11434/api/chat", content);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var chatResponse = JsonSerializer.Deserialize<aiAssist>(responseBody, options);

                if (chatResponse?.Message != null)
                {
                    _messageStore[messageGuid].Add(chatResponse.Message.Content);
                }

                return Ok(responseBody);
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, $"Chyba: {error}");
            }
        }







        
        public class EmbedRequest
        {
            public string Text { get; set; }
        }
        
        [HttpPost("embed-txt-files")]
        public async Task<IActionResult> EmbedTxtFilesIndividually()
        {
            string folderPath = @"C:\Users\Honza\Desktop\datasetCode\scraped_texts";
            string embeddingUrl = "http://192.168.153.186:11434/api/embed";
            string qdrantUrl = "http://192.168.153.200:6333";
            string collectionName = "dotted";

            if (!Directory.Exists(folderPath))
                return NotFound("Složka neexistuje.");

            var txtFiles = Directory.GetFiles(folderPath, "*.txt");
            if (txtFiles.Length == 0)
                return BadRequest("Ve složce nejsou žádné .txt soubory.");

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
                    return StatusCode((int)createRes.StatusCode, $"Chyba pøi vytváøení kolekce: {err}");
                }
            }

            int idCounter = 0;

            foreach (var file in txtFiles)
            {
                var text = await System.IO.File.ReadAllTextAsync(file);

                // Poslat do embedding modelu
                var embedPayload = new { model = "bge-m3", input = text };
                var embedRes = await http.PostAsJsonAsync(embeddingUrl, embedPayload);

                if (!embedRes.IsSuccessStatusCode)
                    return StatusCode((int)embedRes.StatusCode, $"Chyba pøi embedování souboru: {file}");

                var resJson = await embedRes.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(resJson);

                if (!doc.RootElement.TryGetProperty("embeddings", out var embeddingsElement))
                    return StatusCode(500, "Odpovìï z embedding modelu neobsahuje 'embeddings'.");

                if (embeddingsElement.GetArrayLength() == 0)
                    return StatusCode(500, "Odpovìï obsahuje prázdné pole 'embeddings'.");

                var vector = embeddingsElement[0].EnumerateArray().Select(x => x.GetSingle()).ToArray();

                // Uložit do Qdrantu
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
                    return StatusCode((int)upsertRes.StatusCode, $"Chyba pøi ukládání do Qdrantu (soubor {file})");

                idCounter++;
            }

            return Ok("Všechny soubory byly úspìšnì embedovány a uloženy do Qdrantu.");
        }
        

    }
}
