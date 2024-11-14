using TestCase.Models.Database;

namespace TestCase.Interfaces.Repositories
{
    public interface IUsersRespository
    {
        Task<UserDto> GetByIdAsync(Guid id);
        Task<UserDto> GetUserNameAsync(string userName);
    }
}
