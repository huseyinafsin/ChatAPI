namespace ChatAPI.Abstracts
{
    public interface IUnitOfWork
    {
        void Commit();
        Task CommitAsync();
    }
}
