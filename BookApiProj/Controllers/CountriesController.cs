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
    public class CountriesController : ControllerBase
    {
        private ICountryRepository _countryRepository;
        private IAuthorRepository _authorRepository;

        public CountriesController(ICountryRepository countryRepository, IAuthorRepository authorRepository)
        {
            _countryRepository = countryRepository;
            _authorRepository = authorRepository;
        }

        //api/countries
        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CountryDto>))]
        public IActionResult GetCountries()
        {
            var countries = _countryRepository.GetCountries().ToList();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var countriesDto = new List<CountryDto>();
            foreach (var country in countries)
            {
                countriesDto.Add(new CountryDto
                {
                    Id = country.Id,
                    Name = country.Name,
                });
            }

            return Ok(countriesDto);
        }


        //api/countries/countryId
        [HttpGet("{countryId}", Name = "GetCountry")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CountryDto>))]
        public IActionResult GetCountry(int countryId)
        {
            if (!_countryRepository.CountryExists(countryId))
                return NotFound();

            var country = _countryRepository.GetCountry(countryId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var countryDto = new CountryDto()
            {
                Id = country.Id,
                Name = country.Name
            };

            return Ok(countryDto);
        }


        //api/countries/authors/authorId
        [HttpGet("authors/{authorId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CountryDto>))]
        public async Task<IActionResult> GetCountryOfAnAuthor(int authorId)
        {
            if (!_authorRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var country = _countryRepository.GetCountryOfAnAuthor(authorId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var countryDto = new CountryDto()
            {
                Id = country.Id,
                Name = country.Name
            };

            return Ok(countryDto);
        }


        //api/countries/countryId/authors
        [HttpGet("{countryId}/authors")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDto>))]
        public async Task<IActionResult> GetAuthorsFromACountry(int countryId)
        {
            if (!_countryRepository.CountryExists(countryId))
                return NotFound();

            var authors = _countryRepository.GetAuthorsFromCountry(countryId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

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



        //api/countries
        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        [ProducesResponseType(200, Type = typeof(Country))]
        public async Task<IActionResult> CreateCountry([FromBody]Country countryToCreate)
        {
            if (countryToCreate == null)
            {
                return BadRequest(ModelState);
            }

            var country = _countryRepository.GetCountries()
                            .Where(c => c.Name.Trim().ToUpper() == countryToCreate.Name.Trim().ToUpper())
                            .FirstOrDefault();

            if (country != null)
            {
                ModelState.AddModelError("", $"Country {countryToCreate.Name} alreade exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_countryRepository.CreateCountry(countryToCreate))
            {
                ModelState.AddModelError("", $"Something went wrong saving {countryToCreate.Name}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetCountry", new { countryId = countryToCreate.Id }, countryToCreate);
        }




        //api/countries/countryId
        [HttpPut("{countryId}")]
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        [ProducesResponseType(200, Type = typeof(Country))]
        public async Task<IActionResult> UpdateCountry([FromRoute]int countryId, [FromBody]Country updatedCountryInfo)
        {

            updatedCountryInfo.Id = countryId;

            if (updatedCountryInfo == null)
            {
                return BadRequest(ModelState);
            }

            /*if (countryId != updatedCountryInfo.Id)
            {
                return BadRequest(ModelState);
            }*/

            if (!_countryRepository.CountryExists(countryId))
            {
                return NotFound();
            }

            if (_countryRepository.IsDuplicateCountryName(countryId, updatedCountryInfo.Name))
            {
                ModelState.AddModelError("", $"Country {updatedCountryInfo.Name} alreade exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_countryRepository.UpdateCountry(updatedCountryInfo))
            {
                ModelState.AddModelError("", $"Something went wrong updating {updatedCountryInfo.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }


        //api/countries/countryId
        [HttpDelete("{countryId}")]
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult DeleteCountry(int countryId)
        {
            if (!_countryRepository.CountryExists(countryId))
            {
               return NotFound();
            }

            var deleteCountry = _countryRepository.GetCountry(countryId);

            if (_countryRepository.GetAuthorsFromCountry(countryId).Count() > 0)
            {
                ModelState.AddModelError("", $"Country {deleteCountry.Name} " +
                                                                "can not be deleted because authors used this country");
                return StatusCode(409, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_countryRepository.DeleteCountry(deleteCountry))
            {
                ModelState.AddModelError("", $"Something went wrong deleting {deleteCountry.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
