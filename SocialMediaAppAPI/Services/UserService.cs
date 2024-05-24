using SocialMediaAppAPI.Interfaces;
using SocialMediaAppAPI.Models;

namespace SocialMediaAppAPI.Services
{
    public class UserService : IGenericService<User>, IUserService
    {
        public void Add(User item)
        {
            throw new NotImplementedException();
        }

        public void Delete(User item)
        {
            throw new NotImplementedException();
        }

        public IQueryable<User>? GetAllItems()
        {
            throw new NotImplementedException();
        }

        public User? GetItemById(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(User item)
        {
            throw new NotImplementedException();
        }

    }
}
