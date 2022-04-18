namespace AuthorizationWithCustomClaim.Services
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public List<string> Claims { get; set; }
        public string UserId { get; set; }
    }
}
