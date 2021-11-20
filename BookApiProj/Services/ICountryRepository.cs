using BookApiProj.Models;
using System.Collections.Generic;

namespace BookApiProj.Services
{
    public interface ICountryRepository
    {
        ICollection<Country> GetCountries();
        Country GetCountry(int countryId);
        Country GetCountryOfAnAuthor(int authorId);
        ICollection<Author> GetAuthorsFromCountry(int countryId);
        bool CountryExists(int countryId);
    }
}
