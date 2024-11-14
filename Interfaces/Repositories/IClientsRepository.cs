using TestCase.Models.Database;

namespace TestCase.Interfaces.Repositories
{
    public interface IClientsRepository
    {
        Task<ClientDto> GetByIdAsync(Guid id);
    }
}
