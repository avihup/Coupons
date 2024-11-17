using System.ComponentModel.DataAnnotations;

namespace TestCase.Models.ViewModels
{
    public class CreateUserRequest
    {
        public Guid? ClientId { get; set; }
        [Required]
        [Length(3, 100)]
        public string UserName { get; set; }
        [Required]
        [Length(8, 100)]
        public string Password { get; set; }
    }
}
