using Tasker.Models.Enums;

namespace Tasker.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;

        public byte[] PasswordHash { get; set; } = null!;
        public byte[] PasswordSalt { get; set; } = null!;

        public UserRole Role { get; set; } = UserRole.User;

        public ICollection<TaskItem> TaskItems { get; set; } = new List<TaskItem>();
    }
}
