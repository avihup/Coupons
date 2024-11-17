using TestCase.Models.Database;

namespace TestCase.Interfaces.Repositories
{
    public interface IUsersRespository
    {
        Task<UserDto> GetByIdAsync(Guid id, Guid? clientId = null);
        Task<UserDto> GetUserNameAsync(string userName);

        Task<UserDto> CreateAsync(UserDto user);
        Task<IEnumerable<UserDto>> GetAllAsync(Guid? clientId = null);
    }
}
