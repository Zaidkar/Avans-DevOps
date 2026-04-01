using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Application.Repositories
{
    public interface IUserRepository
    {
        List<User> GetAll();
        List<User> GetTesters();
        User? GetById(int id);
    }
}
