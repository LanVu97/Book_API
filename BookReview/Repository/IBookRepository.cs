using BookReview.Dto;
using BookReview.Helper;
using BookReview.Models;

namespace BookReview.Repository
{
    public interface IBookRepository
    {
        ICollection<Book> GetBooks(int pageNumber, int pageSize);

        int CountBook();
        Book GetBook(int id);
        Book GetBook(string title);
        decimal GetBookRating(int bookId);
        bool BookExists(int bookId);
        public Book GetBookTrimToUpper(BookForCreateDto bookCreate);
        bool CreateBook(int ownerId, int categoryId, Book book);

        bool UpdateBook(int ownerId, int categoryId, Book book);
        bool DeleteBook(Book book);
        bool Save();
    }
}
