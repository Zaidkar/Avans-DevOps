using Avans_DevOps.AvansDevOps.Domain.Enum;

namespace Avans_DevOps.AvansDevOps.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public SprintRole Role { get; set; }
        public string Email { get; set; } = string.Empty;
        public string SlackChannel { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;


    }
}
