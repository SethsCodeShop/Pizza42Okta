using Newtonsoft.Json;

namespace Pizza42Okta
{
    public class AuthenticationTokenUser
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("scope")]
        public string Scope { get; set; }
        [JsonProperty("expires_in")]
        public string ExpiresIn { get; set; }
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
    }
}
