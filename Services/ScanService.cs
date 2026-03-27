using Anthropic.SDK;
using Anthropic.SDK.Constants;
using Anthropic.SDK.Messaging;
using ReceiptProject1.DTOs.ItemDTOs;
using ReceiptProject1.DTOs.ReceiptDTOs;
using System.Text.Json;

namespace ReceiptProject1.Services
{
    public class ScanService
    {
        private readonly IConfiguration _config;

        public ScanService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<CreateReceiptDTO> ScanReceiptAsync(IFormFile image)
        {
            try
            {
                using var ms = new MemoryStream();
                await image.CopyToAsync(ms);
                var base64 = Convert.ToBase64String(ms.ToArray());
                var mediaType = image.ContentType;

                var client = new AnthropicClient(_config["Anthropic:ApiKey"]!);

                var messages = new List<Message>
            {
                new Message
                {
                    Role = RoleType.User,
                    Content = new List<ContentBase>
                    {
                        new ImageContent
                        {
                            Source = new ImageSource
                            {
                                Type = SourceType.base64,
                                MediaType = mediaType,
                                Data = base64
                            }
                        },
                        new TextContent
                        {
                            Text = @"Extract all line items from this receipt.
                            Return ONLY a valid JSON object with this exact structure, no other text:
                            {
                                ""storeName"": ""string"",
                                ""date"": ""YYYY-MM-DD"",
                                ""items"": [
                                    {
                                        ""title"": ""string"",
                                        ""price"": 0.00,
                                        ""quantity"": 1
                                    }
                                ]
                            }"
                        }
                    }
                }
            };

                var response = await client.Messages.GetClaudeMessageAsync(new MessageParameters
                {
                    Model = AnthropicModels.Claude45Haiku,
                    MaxTokens = 1024,
                    Messages = messages
                });

                var json = response.Content[0].ToString()!;

                json = json.Trim();
                if (json.StartsWith("```json"))
                    json = json.Substring(7);
                if (json.StartsWith("```"))
                    json = json.Substring(3);
                if (json.EndsWith("```"))
                    json = json.Substring(0, json.Length - 3);
                json = json.Trim();

                var scanned = JsonSerializer.Deserialize<CreateReceiptDTO>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })!;

                return new CreateReceiptDTO
                {
                    StoreName = scanned.StoreName,
                    Date = scanned.Date,
                    Items = scanned.Items.Select(i => new CreateItemDTO
                    {
                        Title = i.Title,
                        Price = i.Price,
                        Quantity = (int)i.Quantity  // ← convert decimal to int
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Scan failed: {ex.Message} | Inner: {ex.InnerException?.Message}");
            }
        }
        private class ScannedReceiptDTO
        {
            public string StoreName { get; set; } = "";
            public string Date { get; set; } = "";
            public List<ScannedItemDTO> Items { get; set; } = new();
        }

        private class ScannedItemDTO
        {
            public string Title { get; set; } = "";
            public decimal Price { get; set; }
            public decimal Quantity { get; set; }  // ← decimal to accept 1.0
        }
    }
}
