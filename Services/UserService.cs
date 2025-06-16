using acordemus.Models;
using MongoDB.Driver;

namespace acordemus.Services
{
    public interface IUserService
    {
        Task<List<User>> GetAsync();
        Task<User> GetByIdAsync(string id);
        Task<User> CreateAsync(User user, HttpContext context);
    }
    public class UserService(IMongoDatabase database) : IUserService
    {
        private readonly IMongoCollection<User> _userCollection = database.GetCollection<User>("users");

        public async Task<User> CreateAsync(User user, HttpContext context)
        {
            user.CreatedAt = DateTime.Now;

            await _userCollection.InsertOneAsync(user);

            return user;
        }

        public Task<List<User>> GetAsync() => _userCollection.Find(_ => true).ToListAsync();

        public Task<User> GetByIdAsync(string id) => _userCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }
}
