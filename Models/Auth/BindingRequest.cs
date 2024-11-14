using System.ComponentModel.DataAnnotations;

namespace TestCase.Models.Auth
{
    public class BindingRequest
    {
        [Required]
        public string AccessToken { get; set; }

    }
}
