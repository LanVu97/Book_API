using BookReview.Data;
using BookReview.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;

namespace BookReview.Repository
{
    public class UserRepository : IUserRepository
    {
        private DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;   
        }

        public bool CreateUser(User user)
        {
            _context.Add(user);
            return Save();
        }

        public User GetUser(string name)
        {
            return _context.Users.Where(u => u.Username == name).FirstOrDefault();
        }

        public ICollection<User> GetUsers()
        {
            return _context.Users.ToList();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }


    }
}
