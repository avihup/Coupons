using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TestCase.Models.ViewModels
{
    public class UserViewModel
    {
        public Guid? Id { get; set; }
        public Guid? ClientId { get; set; }
        public string UserName { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}
