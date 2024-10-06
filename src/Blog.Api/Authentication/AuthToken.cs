using System.Text.Json.Serialization;

namespace Blog.Api.Authentication
{
    public sealed class AuthToken
    {
        [JsonPropertyName("result")]
        public AuthResult Result { get; set; }
    }

    public sealed class AuthResult
    {
        [JsonPropertyName("access_token")]
        public string Access_token { get; set; }

        [JsonPropertyName("expires_in")]
        public DateTime Expires_in { get; set; }

        [JsonPropertyName("user")]
        public AuthUser User { get; set; }
    }

    public sealed class AuthClaim
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }

    public sealed class AuthUser
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("sobrenome")]
        public string Sobrenome { get; set; }

        [JsonPropertyName("nomeCompleto")]
        public string NomeCompleto { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("claims")]
        public IEnumerable<AuthClaim> Claims { get; set; } = new List<AuthClaim>();
    }
}
