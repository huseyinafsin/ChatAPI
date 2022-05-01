using Microsoft.AspNetCore.Identity;

namespace ChatAPI.Models
{
    public class AppUser : IdentityUser<int>
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public ICollection<Room> Rooms { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}
