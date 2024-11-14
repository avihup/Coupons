namespace TestCase.Models.Auth
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public string UserName { get; set; }
        public Guid? ClientId { get; set; }
        public string ClientName { get; set; }
    }
}
