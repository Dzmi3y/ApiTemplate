namespace API.Contracts.Responses
{
    public class AuthSuccessResponse
    {
        // [JsonProperty("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        // [JsonProperty("refresh_token")]
        public Guid RefreshToken { get; set; }

        // [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        // [JsonProperty("token_type")]
        public string TokenType => "Bearer";
    }

}
