namespace AuthorizationWithCustomClaim.Entities
{
    public class ClaimRequest
    {
        public string RoleName { get; set; }

        public List<string> Claims { get; set; }
    }
}
