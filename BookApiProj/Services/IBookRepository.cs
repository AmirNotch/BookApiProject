﻿using BookApiProj.Models;
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
        bool BookExists(string BookIsbn);
        bool IsDuplicatelIsbn(int bookId, string bookIsbn);
        decimal GetBookRating(int bookId);
    }
}
