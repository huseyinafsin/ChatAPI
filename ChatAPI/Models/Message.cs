namespace ChatAPI.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime SendTime { get; set; }
        public AppUser SenderUser { get; set; }
  
    }
}
