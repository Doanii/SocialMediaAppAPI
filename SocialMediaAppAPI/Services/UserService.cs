using SocialMediaAppAPI.Data;
using SocialMediaAppAPI.Models;
using SocialMediaAppAPI.Types.Interfaces;

namespace SocialMediaAppAPI.Services
{
    public class UserService : IGenericService<User>, IUserService
    {
        private readonly APIDbContext context;

        public UserService(APIDbContext _context)
        {
            this.context = _context;
        }

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
