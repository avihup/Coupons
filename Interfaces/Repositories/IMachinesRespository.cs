using TestCase.Models.Database;

namespace TestCase.Interfaces.Repositories
{
    public interface IMachinesRepository
    {
        Task<MachineDto> GetByIdAsync(Guid id);
        Task<MachineDto> GetByAccessToken(string accessToken);
    }
}
