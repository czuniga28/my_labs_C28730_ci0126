using backend_lab_c28730.Models;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Data.SqlClient;

namespace backend_lab_c28730.Repositories {
  public class CountryRepository {
    private readonly string _connectionString;
    public CountryRepository() {
      var builder = WebApplication.CreateBuilder();
      _connectionString = builder.Configuration.GetConnectionString("CountryContext");
    }
    public List<CountryModel> GetCountries() {
      using var connection = new SqlConnection(_connectionString);
      string query = "SELECT * FROM dbo.Country";
      return connection.Query<CountryModel>(query).ToList();
    }

    public bool CreateCountry(CountryModel country) {
      using var connection = new SqlConnection(_connectionString);
      var query = @"INSERT INTO [dbo].[Country] ([Name], [Continent], [Language]) VALUES (@Name, @Continent, @Language)";
      var affectedRows = connection.Execute(query, new {
        Name = country.Name,
        Continent = country.Continent,
        Language = country.Language
      });
      return affectedRows >= 1;
    }
  }
}