using BookApiProj.Dtos;
using BookApiProj.Models;
using BookApiProj.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookApiProj.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private IAuthorRepository _authorRepository;
        private IBookRepository _bookRepository;
        private ICountryRepository _countryRepository;

        public AuthorsController(IAuthorRepository authorRepository, IBookRepository bookRepository, ICountryRepository countryRepository)
        {
            _authorRepository = authorRepository;
            _bookRepository = bookRepository;
            _countryRepository = countryRepository;
        }

        //api/authors
        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDto>))]
        public async Task<IActionResult> GetAuthors()
        {
            var authors = _authorRepository.GetAuthors();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authorsDto = new List<AuthorDto>();

            foreach (var author in authors)
            {
                authorsDto.Add(new AuthorDto
                {
                    Id = author.Id,
                    FirstName = author.FirstName,
                    LastName = author.LastName
                });
            }

            return Ok(authorsDto);
        }


        //api/authors/authorId
        [HttpGet("{authorId}", Name = "GetAuthor")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDto>))]
        public async Task<IActionResult> GetAuthor(int authorId)
        {
            if (!_authorRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var author = _authorRepository.GetAuthor(authorId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authorDto = new AuthorDto
            {
                Id = author.Id,
                FirstName = author.FirstName,
                LastName = author.LastName
            };

            return Ok(authorDto);
        }



        //api/authors/authorId/books
        [HttpGet("{authorId}/books")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
        public async Task<IActionResult> GetBooksByAuthor(int authorId)
        {
            if (!_authorRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var books = _authorRepository.GetBooksByAuthor(authorId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var bookDto = new List<BookDto>();

            foreach (var book in books)
            {
                bookDto.Add(new BookDto()
                {
                    Id = book.Id,
                    Isbn = book.Isbn,
                    Title = book.Title,
                    DatePublished = book.DatePublished
                });
            }

            return Ok(bookDto);
        }




        //api/authors/books/authorId
        [HttpGet("books/{bookId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDto>))]
        public async Task<IActionResult> GetAuthorsOfABook(int bookId)
        {
            if (!_bookRepository.BookExists(bookId))
            {
                return NotFound();
            }

            var authors = _authorRepository.GetAuthorsOfABook(bookId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authorsDto = new List<AuthorDto>();

            foreach (var author in authors)
            {
                authorsDto.Add(new AuthorDto
                {
                    Id = author.Id,
                    FirstName = author.FirstName,
                    LastName = author.LastName
                });
            }

            return Ok(authorsDto);
        }


        //api/authors
        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [ProducesResponseType(201, Type = typeof(Author))]
        public async Task<IActionResult> CreateAuthor([FromBody] Author authorToCreate)
        {
            if (authorToCreate == null)
            {
                return BadRequest(ModelState);
            }

            if (!_countryRepository.CountryExists(authorToCreate.Country.Id))
            {
                ModelState.AddModelError("", "Country doesn't exist!");
            }

            authorToCreate.Country = _countryRepository.GetCountry(authorToCreate.Country.Id);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_authorRepository.CreateAuthor(authorToCreate))
            {
                ModelState.AddModelError("", $"Something went wrong saving the author " + $"{authorToCreate.FirstName} and {authorToCreate.LastName}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetAuthor", new { authorId = authorToCreate.Id }, authorToCreate);
        }

        //api/authors/authorId
        [HttpPut("{authorId}")]
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(200, Type = typeof(Author))]
        public async Task<IActionResult> UpdateAuthor([FromRoute] int authorId, [FromBody] Author updatedAuthorInfo)
        {

            updatedAuthorInfo.Id = authorId;

            if (updatedAuthorInfo == null)
            {
                return BadRequest(ModelState);
            }

            if (!_authorRepository.AuthorExists(authorId))
            {
                ModelState.AddModelError("", "Author doesn't exist!");
            }

            if (!_countryRepository.CountryExists(updatedAuthorInfo.Country.Id))
            {
                ModelState.AddModelError("", "Country doesn't exist!");
            }

            if (!ModelState.IsValid)
            {
                return StatusCode(404, ModelState);
            }

            updatedAuthorInfo.Country = _countryRepository.GetCountry(updatedAuthorInfo.Country.Id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_authorRepository.UpdateAuthor(updatedAuthorInfo))
            {
                ModelState.AddModelError("", $"Something went wrong updating the author " + $"{updatedAuthorInfo.FirstName} and {updatedAuthorInfo.LastName}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }


        //api/authors/authorId
        [HttpDelete("{authorId}")]
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult DeleteReview(int authorId)
        {
            if (!_authorRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var deleteAuthor = _authorRepository.GetAuthor(authorId);
            
            if (_authorRepository.GetBooksByAuthor(authorId).Count() > 0)
            {
                ModelState.AddModelError("", $"Author {deleteAuthor.FirstName}  {deleteAuthor.LastName} " +
                                                                "can not be deleted because author used this books");
                return StatusCode(409, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_authorRepository.DeleteAuthor(deleteAuthor))
            {
                ModelState.AddModelError("", $"Something went wrong deleting {deleteAuthor.FirstName}  {deleteAuthor.LastName}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
