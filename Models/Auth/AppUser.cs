namespace TestCase.Models.Auth
{
    public class AppUser
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public Guid? ClientId { get; set; }
        public string ClientName { get; set; }
    }
}
