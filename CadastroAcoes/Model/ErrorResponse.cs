using System.Text.Json.Serialization;

namespace Model
{
    public class ErrorResponse
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("details")]
        public string? Details { get; set; }
    }
}
