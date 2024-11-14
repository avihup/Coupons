using TestCase.Models.Auth;

namespace TestCase.Interfaces.Auth
{
    public interface IBindingService
    {
        Task<BindingResponse> LoginAsync(BindingRequest request);
    }
}
