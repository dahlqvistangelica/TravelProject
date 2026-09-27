using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Data;

using DbContext;
using Configuration;
using Models;
using Models.DTO;
using DbModels;
using Seido.Utilities.SeedGenerator;
using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace DbRepos;

public class AdminDbRepos
{
    private const string _seedSource = "./app-seeds.json";
    private readonly ILogger<AdminDbRepos> _logger;
    private Encryptions _encryptions;
    private readonly MainDbContext _dbContext;
    #region Constructors
    public AdminDbRepos(ILogger<AdminDbRepos> logger, Encryptions encryptions, MainDbContext context)
    {
        _logger = logger;
        _encryptions = encryptions;
        _dbContext = context;
    }
    #endregion
        public async Task<ResponseItemDto<GstUsrInfoAllDto>> DbInfoAsync()
    {
        var info = new GstUsrInfoAllDto();
        info.Db = await _dbContext.InfoDbView.FirstAsync();
        info.Attractions = await _dbContext.InfoAttractionsView.ToListAsync();
        info.Users = await _dbContext.InfoUsersView.ToListAsync();
        info.Cities = await _dbContext.InfoCitiesView.ToListAsync();

        return new ResponseItemDto<GstUsrInfoAllDto>
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif

            Item = info
        };
    }

   public async Task<ResponseItemDto<GstUsrInfoAllDto>> RobustSeedingAsync()
    {
        //Remove all seeded data first, then seed again
        await RemoveSeedAsync(true);

        //Create a new seed generator and generate the data
        var fn = Path.GetFullPath(_seedSource);
        var seeder = new SeedGenerator(fn);
        
        var countries = seeder.UniqueItemsToList<CountryDbM>(4);
        var categories = seeder.UniqueItemsToList<CategoryDbM>(30);
        var attractions = seeder.ItemsToList<AttractionDbM>(1000);
        var users = seeder.ItemsToList<UserDbM>(50);
        var addresses = new List<AddressDbM>();
        foreach(var country in countries)
        {
            var countryCities = Enumerable.Range(0, 25)
                .Select(_ => new CityDbM { CountryDbM = country }.Seed(seeder))
                .ToList();
            country.CitiesDbM = countryCities;
            foreach(var city in countryCities)
            {
                var cityAddresses = Enumerable.Range(0, 50)
                    .Select(_ => new AddressDbM
                    {
                        CityDbM = city,
                    }.Seed(seeder))
                    .ToList();
                city.AddressesDbM = cityAddresses;
                addresses.AddRange(cityAddresses);
            }
        }
        foreach(var attraction in attractions)
        {
            attraction.CategoryDbM = seeder.FromList(categories);
            attraction.AddressDbM = seeder.FromList(addresses);
            attraction.ReviewsDbM = seeder.ItemsToList<ReviewDbM>(seeder.Next(0, 21));
            foreach(var review in attraction.ReviewsDbM)
            {
                review.UserDbM = seeder.FromList(users);
                review.AttractionDbM = attraction;
            }
        }

        _dbContext.Countries.AddRange(countries);
        _dbContext.Cities.AddRange(countries.SelectMany(country => country.CitiesDbM));
        _dbContext.Addresses.AddRange(addresses);
        _dbContext.Categories.AddRange(categories);
        _dbContext.Users.AddRange(users);
        _dbContext.Attractions.AddRange(attractions);
        
        await _dbContext.SaveChangesAsync();
        return await DbInfoAsync();
    }

    public async Task<ResponseItemDto<GstUsrInfoAllDto>> RemoveSeedAsync(bool seeded)
    {
        var connection = _dbContext.Database.GetDbConnection();
        using var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;

        List<DbParameter> parameters;
        command.CommandText="supusr.spDeleteAll";
        parameters = new List<DbParameter>
        {
            new SqlParameter("seededParam", seeded),
            new SqlParameter("nrAttractionsAffected", SqlDbType.Int) {Direction = ParameterDirection.Output},
            new SqlParameter("nrAddressesAffected", SqlDbType.Int) {Direction = ParameterDirection.Output},
            new SqlParameter("nrCategoriesAffected", SqlDbType.Int) {Direction = ParameterDirection.Output},
            new SqlParameter("nrReviewsAffected", SqlDbType.Int) {Direction = ParameterDirection.Output},
            new SqlParameter("nrUsersAffected", SqlDbType.Int) {Direction = ParameterDirection.Output},
            new SqlParameter("nrCitiesAffected", SqlDbType.Int) {Direction = ParameterDirection.Output},
            new SqlParameter("nrCountriesAffected", SqlDbType.Int) {Direction = ParameterDirection.Output}
        };

        command.Parameters.AddRange(parameters.ToArray());

        if(connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }
        
        using var reader = await command.ExecuteReaderAsync();
        GstUsrInfoDbDto result_set = null;
        if(reader.HasRows)
        {
            await reader.ReadAsync();

            result_set = new GstUsrInfoDbDto
            {
                NrSeededAttractions = Convert.ToInt32(reader["NrSeededAttractions"]),
                NrUnseededAttractions = Convert.ToInt32(reader["NrUnseededAttractions"]),
                NrAttractionsWithAddress = Convert.ToInt32(reader["NrAttractionsWithAddress"]),
                NrSeededAddresses = Convert.ToInt32(reader["NrSeededAddresses"]),
                NrUnseededAddresses = Convert.ToInt32(reader["NrUnseededAddresses"]),
                NrSeededCategories = Convert.ToInt32(reader["NrSeededCategories"]),
                NrUnSeededCategories = Convert.ToInt32(reader["NrUnSeededCategories"]),
                NrSeededUsers = Convert.ToInt32(reader["NrSeededUsers"]),
                NrUnseededUsers = Convert.ToInt32(reader["NrUnseededUsers"]),
                NrSeededReviews = Convert.ToInt32(reader["NrSeededReviews"]),
                NrUnseededReviews = Convert.ToInt32(reader["NrUnseededReviews"])
            };
        }
        await reader.CloseAsync();

        return await DbInfoAsync();
    }
}
