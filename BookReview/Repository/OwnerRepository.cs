using BookReview.Data;
using BookReview.Models;
using System;

namespace BookReview.Repository
{
    public class OwnerRepository : IOwnerRepository
    {
        private DataContext _context;

        public OwnerRepository(DataContext context)
        {
            _context = context;
        }
        public bool CreateOwner(Owner owner)
        {
            _context.Add(owner);
            return Save();
        }

        public bool DeleteOwner(Owner owner)
        {
            _context.Remove(owner);
            return Save();
        }

        public Owner GetOwner(int ownerId)
        {
            return _context.Owners.Where(o => o.Id == ownerId).FirstOrDefault();
        }

        public ICollection<Owner> GetOwnerOfABook(int bookId)
        {
            return _context.BookOwners.Where(b => b.Book.Id == bookId).Select(b => b.Owner).ToList();
        }

        public ICollection<Owner> GetOwners()
        {
            return _context.Owners.ToList();
        }

        public ICollection<Book> GetBookByOwner(int ownerId)
        {
            return _context.BookOwners.Where(p => p.Owner.Id == ownerId).Select(p => p.Book).ToList();
        }

        public bool OwnerExists(int ownerId)
        {
            throw new NotImplementedException();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateOwner(Owner owner)
        {
            _context.Update(owner);
            return Save();
        }
    }
}
