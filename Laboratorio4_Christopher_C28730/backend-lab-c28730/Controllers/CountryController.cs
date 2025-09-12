using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using backend_lab_c28730.Models;
using backend_lab_c28730.Services;

namespace backend_lab_c28730.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class CountryController : ControllerBase
  {
    private readonly CountryService countryService;

    public CountryController()
    {
      countryService = new CountryService();
    }

    [HttpGet]
    public List<CountryModel> Get()
    {
      var countries = countryService.GetCountries();
      return countries;
    }

    [HttpPost]
    public async Task<ActionResult<bool>> CreateCountry(CountryModel country)
    {
      if (country == null) {
        return BadRequest();
      }

      var result = countryService.CreateCountry(country);
      if (string.IsNullOrEmpty(result)) {
        return Ok(result);
      } else {
        return BadRequest();
      }
    }
  }
}