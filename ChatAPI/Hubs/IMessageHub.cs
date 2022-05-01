namespace ChatAPI.Hubs
{
    public interface IMessageHub
    {
        Task ReceiveMessage(string message);

    }
}
