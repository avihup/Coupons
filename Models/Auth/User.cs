namespace TestCase.Models.Auth
{
    public class User
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public Guid? ClientId { get; set; }
        public string ClientName { get; set; }
    }
}
