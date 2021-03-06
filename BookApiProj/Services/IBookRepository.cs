using BookApiProj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookApiProj.Services
{
    public interface IBookRepository
    {
        ICollection<Book> GetBooks();
        Book GetBook(int bookId);
        Book GetBook(string bookIsbn);
        bool BookExists(int bookId);
        bool BookExists(string bookIsbn);
        bool IsDuplicateIsbn(int bookId, string bookIsbn);
        decimal GetBookRating(int bookId);

        bool CreateBook(List<int> authorsId, List<int> categoriesId, Book book);
        bool UpdateBook(List<int> authorsId, List<int> categoriesId, Book book);
        bool DeleteBook(Book book);
        bool Save();
    }
}
