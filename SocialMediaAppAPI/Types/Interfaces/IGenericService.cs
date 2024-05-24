namespace SocialMediaAppAPI.Types.Interfaces
{
    public interface IGenericService<T> where T : class
    {
        IQueryable<T>? GetAllItems();

        T? GetItemById(Guid id);

        void Add(T item);

        void Update(T item);

        void Delete(T item);
    }
}
