using TestCase.Models.Database;
using TestCase.Models.ViewModels;

namespace TestCase.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserViewModel> GetByIdAsync(Guid id, Guid? clientId = null);
        Task<IEnumerable<UserViewModel>> GetAllAsync(Guid? clientId = null);
        Task<UserViewModel> CreateAsync(CreateUserRequest request);
    }
}
