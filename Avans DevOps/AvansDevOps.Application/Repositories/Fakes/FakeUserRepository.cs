using Avans_DevOps.AvansDevOps.Domain.Entities;
using Avans_DevOps.AvansDevOps.Domain.Enum;

namespace Avans_DevOps.AvansDevOps.Application.Repositories.Fakes
{
    public class FakeUserRepository : IUserRepository
    {
        private readonly List<User> _users =
        [
            new User { Id = 1, Name = "Alice", Role = UserRole.Developer, Email = "alice@example.com", SlackChannel = "#dev", PhoneNumber = "+31610000001" },
            new User { Id = 2, Name = "Bob", Role = UserRole.Tester, Email = "bob@example.com", SlackChannel = "#qa", PhoneNumber = "+31610000002" },
            new User { Id = 3, Name = "Charlie", Role = UserRole.ScrumMaster, Email = "charlie@example.com", SlackChannel = "#scrum", PhoneNumber = "+31610000003" }
        ];

        public List<User> GetAll()
        {
            return _users;
        }
        public List<User> GetTesters()
        {
            return _users.Where(user => user.Role == UserRole.Tester).ToList();
        }

        public User? GetById(int id)
        {
            return _users.FirstOrDefault(user => user.Id == id);
        }
    }
}
