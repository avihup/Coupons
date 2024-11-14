using TestCase.Models.Database;

namespace TestCase.Interfaces.Repositories
{
    public interface IKiosksRespository
    {
        Task<KioskDto> GetByIdAsync(Guid id);
        Task<KioskDto> GetByAccessToken(string accessToken);
    }
}
