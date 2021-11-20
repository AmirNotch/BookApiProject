using BookApiProj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookApiProj.Services
{
    public interface ICategoryRepository
    {
        ICollection<Category> GetCategories();
        Category GetCategory(int categoryId);
        ICollection<Book> GetAllBooksForCategory(int categoryId);
        ICollection<Category> GetAllCategoriesForABook(int bookId);
        bool CategoryExists(int categoryId);
    }
}
