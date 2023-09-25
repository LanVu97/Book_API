using BookReview.Models;

namespace BookReview.Repository
{
    public interface IUserRepository
    {
        ICollection<User> GetUsers();
        bool CreateUser(User user);
        User GetUser(string name);
        bool Save();

    }
}
