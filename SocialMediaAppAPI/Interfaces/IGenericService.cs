namespace SocialMediaAppAPI.Interfaces
{
    public interface IGenericService<T> where T : class
    {
        IQueryable<T>? GetAllItems();

        T? GetItemById(int id);

        void Add(T item);

        void Update(T item);

        void Delete(T item);
    }
}
