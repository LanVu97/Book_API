using BookReview.Data;
using BookReview.Dto;
using BookReview.Helper;
using BookReview.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace BookReview.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly DataContext _context;

        public BookRepository(DataContext context)
        {
            _context = context;
        }

        public bool BookExists(int bookId)
        {
            return _context.Book.Any(b => b.Id == bookId);
        }

        public int CountBook()
        {
            return _context.Book.Count();
        }

        public bool CreateBook(int ownerId, int categoryId, Book book)
        {
            var bookOwnerEntity = _context.Owners.Where(a => a.Id == ownerId).FirstOrDefault();
            var category = _context.Categories.Where(a => a.Id == categoryId).FirstOrDefault();

            var bookOwner = new BookOwner()
            {
                Owner = bookOwnerEntity,
                Book = book,
            };

            _context.Add(bookOwner);

            var bookCategory = new BookCategory()
            {
                Category = category,
                Book = book,
            };

            _context.Add(bookCategory);
            _context.Add(book);

            return Save();
        }

        public bool DeleteBook(Book book)
        {
           _context.Remove(book);
            return Save();
        }

        public Book GetBook(int id)
        {
            return _context.Book.Where(b => b.Id == id).FirstOrDefault();

            
        }

        public Book GetBook(string title)
        {
            return _context.Book.Where(b => b.Title==title).FirstOrDefault();
        }

        public decimal GetBookRating(int bookId)
        {
            var review = _context.Reviews.Where(r => r.Book.Id == bookId);

            if(review.Count() <= 0)
            {
                return 0;
            }

            return (decimal)review.Sum(r => r.Rating) / review.Count();
        }

        public ICollection<Book> GetBooks(int pageNumber, int pageSize)
        {
            // return PagedList<Book>.ToPagedList(_context.Book.OrderBy(p => p.Id), bookParameters.PageNumber, bookParameters.PageSize);
            return _context.Book.OrderBy(p => p.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public Book GetBookTrimToUpper(BookForCreateDto bookCreate)
        {
            return _context.Book.Where(c => c.Title.Trim().ToUpper() == bookCreate.Title.TrimEnd().ToUpper())
                        .FirstOrDefault();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;

        }

        public bool UpdateBook(int ownerId, int categoryId, Book book)
        {
            _context.Update(book);
            return Save();
        }
    }
}
