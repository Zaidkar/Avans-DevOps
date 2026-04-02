using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Application.Repositories.Fakes
{
    public class FakeUserRepository : IUserRepository
    {
        private readonly List<User> _users =
        [
            new User { Id = Guid.NewGuid(), Name = "Alice",  Email = "alice@example.com", SlackChannel = "#dev", PhoneNumber = "+31610000001" },
            new User { Id = Guid.NewGuid(), Name = "Bob", Email = "bob@example.com", SlackChannel = "#qa", PhoneNumber = "+31610000002" },
            new User { Id = Guid.NewGuid(), Name = "Charlie", Email = "charlie@example.com", SlackChannel = "#scrum", PhoneNumber = "+31610000003" }
        ];

        public List<User> GetAll()
        {
            return _users;
        }

        public User? GetById(Guid id)
        {
            return _users.FirstOrDefault(user => user.Id == id);
        }
    }
}
