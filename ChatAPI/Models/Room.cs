using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatAPI.Models;

namespace ChatAPI.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public AppUser Admin { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}
