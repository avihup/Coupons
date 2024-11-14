namespace TestCase.Models.Auth
{
    public class Device
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DeviceType { get; set; }
        public Guid ClientId { get; set; }
        public string ClientName { get; set; }
    }
}
