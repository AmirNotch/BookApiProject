﻿using BookApiProj.Dtos;
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
    public class ReviewersController : ControllerBase
    {
        private IReviewerRepository _reviewerRepository;
        private IReviewRepository _reviewRepository;

        public ReviewersController(IReviewerRepository reviewerRepository, IReviewRepository reviewRepository)
        {
            _reviewerRepository = reviewerRepository;
            _reviewRepository = reviewRepository;
        }


        //api/reviewers
        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewerDto>))]
        public async Task<IActionResult> GetReviewers()
        {
            var reviewers = _reviewerRepository.GetReviewers();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reviewerDto = new List<ReviewerDto>();

            foreach (var reviewer in reviewers)
            {
                reviewerDto.Add(new ReviewerDto { Id = reviewer.Id, FirstName = reviewer.FirstName, LastName = reviewer.LastName });
            }

            return Ok(reviewerDto);
        }


        //api/reviewers/reviewerId
        [HttpGet("{reviewerId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewerDto>))]
        public async Task<IActionResult> GetReviewer(int reviewerId)
        {
            if (!_reviewerRepository.ReviewerExists(reviewerId))
            {
                return NotFound();
            }

            var reviewer = _reviewerRepository.GetReviewer(reviewerId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reviewerDto = new ReviewerDto
            {
                Id = reviewer.Id,
                FirstName = reviewer.FirstName,
                LastName = reviewer.LastName
            };

            return Ok(reviewerDto);
        }


        //api/reviewers/reviewerId/reviews
        [HttpGet("{reviewerId}/reviews")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDto>))]
        public async Task<IActionResult> GetReviewsByReviewer(int reviewerId)
        {
            if (!_reviewerRepository.ReviewerExists(reviewerId))
            {
                return NotFound();
            }

            var reviews = _reviewerRepository.GetReviewsByReviewer(reviewerId);

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


        //api/reviewers/reviewId/reviewer
        [HttpGet("{reviewId}/reviewer")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewerDto>))]
        public async Task<IActionResult> GetReviewerOfAReview(int reviewId)
        {
            if (!_reviewRepository.ReviewExists(reviewId))
            {
                return NotFound();
            }

            var reviewer = _reviewerRepository.GetReviewerOfAReview(reviewId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reviewerDto = new ReviewerDto
            {
                Id = reviewer.Id,
                FirstName = reviewer.FirstName,
                LastName = reviewer.LastName
            };

            return Ok(reviewerDto);
        }

    }
}