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
    public class ReviewsController : ControllerBase
    {
        private IReviewerRepository _reviewerRepository;
        private IReviewRepository _reviewRepository;
        private IBookRepository _bookRepository;


        public ReviewsController(IReviewerRepository reviewerRepository, IReviewRepository reviewRepository, IBookRepository bookRepository)
        {
            _reviewerRepository = reviewerRepository;
            _reviewRepository = reviewRepository;
            _bookRepository = bookRepository;
        }


        //api/reviews
        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDto>))]
        public async Task<IActionResult> GetReviews()
        {
            var reviews = _reviewRepository.GetReviews();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reviewsDto = new List<ReviewDto>();

            foreach (var review in reviews)
            {
                reviewsDto.Add(new ReviewDto
                {
                    Id = review.Id, 
                    Headline = review.Headline,
                    ReviewText = review.ReviewText,
                    Rating = review.Rating
                });
            }

            return Ok(reviewsDto);
        }

        //api/reviews/reviewId
        [HttpGet("{reviewId}", Name = "GetReview")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDto>))]
        public async Task<IActionResult> GetReview(int reviewId)
        {
            if (!_reviewRepository.ReviewExists(reviewId))
            {
                return NotFound();
            }

            var review = _reviewRepository.GetReview(reviewId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reviewDto = new ReviewDto
            {
                Id = review.Id,
                Headline = review.Headline,
                ReviewText = review.ReviewText,
                Rating = review.Rating
            };

            return Ok(reviewDto);
        }


        //api/reviews/books/bookId
        [HttpGet("books/{bookId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDto>))]
        public async Task<IActionResult> GetReviewsOfABook(int bookId)
        {
            if (!_bookRepository.BookExists(bookId))
            {
                return NotFound();
            }

            var reviews = _reviewRepository.GetReviewsOfABook(bookId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reviewsDto = new List<ReviewDto>();

            foreach (var review in reviews)
            {
                reviewsDto.Add(new ReviewDto
                {
                    Id = review.Id,
                    Headline = review.Headline,
                    ReviewText = review.ReviewText,
                    Rating = review.Rating
                });
            }

            return Ok(reviewsDto);
        }

        //api/reviews/reviewId/book
        [HttpGet("{reviewId}/book")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
        public async Task<IActionResult> GetBookOfAReview(int reviewId)
        {
            if (!_reviewRepository.ReviewExists(reviewId))
            {
                return NotFound();
            }

            var book = _reviewRepository.GetBookOfAReview(reviewId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var bookDto = new BookDto
            {

                Id = book.Id,
                Isbn = book.Isbn,
                Title = book.Title,
                DatePublished = book.DatePublished
            };

            return Ok(bookDto);
        }


        //api/reviews
        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [ProducesResponseType(201, Type = typeof(Review))]
        public async Task<IActionResult> CreateReview([FromBody] Review reviewToCreate)
        {
            if (reviewToCreate == null)
            {
                return BadRequest(ModelState);
            }

            if (!_reviewerRepository.ReviewerExists(reviewToCreate.Reviewer.Id))
            {
                ModelState.AddModelError("", "Reviewer doesn't exist!");
            }

            if (!_bookRepository.BookExists(reviewToCreate.Book.Id))
            {
                ModelState.AddModelError("", "Book doesn't exist!");
            }

            if (!ModelState.IsValid)
            {
                return StatusCode(404, ModelState);
            }

            reviewToCreate.Book = _bookRepository.GetBook(reviewToCreate.Book.Id);
            reviewToCreate.Reviewer = _reviewerRepository.GetReviewer(reviewToCreate.Reviewer.Id);
            
            
            /*var country = _categoryRepository.GetCategories()
                            .Where(c => c.Name.Trim().ToUpper() == categoryToCreate.Name.Trim().ToUpper())
                            .FirstOrDefault();*/

            /*if (country != null)
            {
                ModelState.AddModelError("", $"Category {categoryToCreate.Name} alreade exists");
                return StatusCode(422, ModelState);
            }*/

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_reviewRepository.CreateReview(reviewToCreate))
            {
                ModelState.AddModelError("", $"Something went wrong saving the review");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetReview", new { reviewId = reviewToCreate.Id }, reviewToCreate);
        }


        //api/reviews/reviewId
        [HttpPut("{reviewId}")]
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(200, Type = typeof(Review))]
        public async Task<IActionResult> UpdateReview([FromRoute] int reviewId, [FromBody] Review updatedReviewInfo)
        {

            updatedReviewInfo.Id = reviewId;

            if (updatedReviewInfo == null)
            {
                return BadRequest(ModelState);
            }

            if (!_reviewerRepository.ReviewerExists(updatedReviewInfo.Reviewer.Id))
            {
                ModelState.AddModelError("", "Reviewer doesn't exist!");
            }

            if (!_bookRepository.BookExists(updatedReviewInfo.Book.Id))
            {
                ModelState.AddModelError("", "Book doesn't exist!");
            }

            if (!ModelState.IsValid)
            {
                return StatusCode(404, ModelState);
            }

            updatedReviewInfo.Book = _bookRepository.GetBook(updatedReviewInfo.Book.Id);
            updatedReviewInfo.Reviewer = _reviewerRepository.GetReviewer(updatedReviewInfo.Reviewer.Id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_reviewRepository.UpdateReview(updatedReviewInfo))
            {
                ModelState.AddModelError("", $"Something went wrong updating the review");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }


        //api/reviews/reviewId
        [HttpDelete("{reviewId}")]
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult DeleteReview(int reviewId)
        {
            if (!_reviewRepository.ReviewExists(reviewId))
            {
                return NotFound();
            }

            var deleteReview = _reviewRepository.GetReview(reviewId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_reviewRepository.DeleteReview(deleteReview))
            {
                ModelState.AddModelError("", $"Something went wrong deleting {deleteReview.Headline}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

    }
}
