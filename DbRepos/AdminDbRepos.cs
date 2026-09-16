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
        private async Task<ResponseItemDto<GstUsrInfoAllDto>> DbInfo()
    {
        var info = new GstUsrInfoAllDto();
        info.Db = new GstUsrInfoDbDto
        {
            NrSeededAttractions = await _dbContext.Attractions.Where(f => f.Seeded).CountAsync(),
            NrUnseededAttractions = await _dbContext.Attractions.Where(f => !f.Seeded).CountAsync(),
            NrAttractionssWithAddress = await _dbContext.Attractions.Where(f => f.AddressId != null).CountAsync(),

            NrSeededAddresses = await _dbContext.Addresses.Where(f => f.Seeded).CountAsync(),
            NrUnseededAddresses = await _dbContext.Addresses.Where(f => !f.Seeded).CountAsync(),

            NrSeededUsers = await _dbContext.Users.Where(f => f.Seeded).CountAsync(),
            NrUnseededUsers = await _dbContext.Users.Where(f => !f.Seeded).CountAsync(),

            NrSeededReviews = await _dbContext.Reviews.Where(f => f.Seeded).CountAsync(),
            NrUnseededReviews = await _dbContext.Reviews.Where(f => !f.Seeded).CountAsync(),

            NrSeededCategories = await _dbContext.Categories.Where(f => f.Seeded).CountAsync(),
            NrUnSeededCategories = await _dbContext.Categories.Where(f=> !f.Seeded).CountAsync()
        };

        return new ResponseItemDto<GstUsrInfoAllDto>
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif

            Item = info
        };
    }

   public async Task<ResponseItemDto<GstUsrInfoAllDto>> SeedAsync(int nrOfItems)
    {

        var fn = Path.GetFullPath(_seedSource);
        var seeder = new SeedGenerator(fn);

        var categories = seeder.UniqueItemsToList<CategoryDbM>(nrOfItems);
        var addresses = seeder.ItemsToList<AddressDbM>(nrOfItems);
        var attractions = seeder.ItemsToList<AttractionDbM>(nrOfItems);
        var users = seeder.ItemsToList<UserDbM>(nrOfItems);
        foreach(var attraction in attractions)
        {
            attraction.CategoryDbM = seeder.FromList(categories);
            attraction.AddressDbM = seeder.FromList(addresses);
            attraction.ReviewsDbM = seeder.ItemsToList<ReviewDbM>(seeder.Next(0,50));
            foreach(var review in attraction.ReviewsDbM)
            {
                review.UserDbM = seeder.FromList(users);
            }
        }

        _dbContext.Attractions.AddRange(attractions);
        

        await _dbContext.SaveChangesAsync();
        return await DbInfo();
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
            new SqlParameter("nrAttrationsAffected", SqlDbType.Int) {Direction = ParameterDirection.Output},
            new SqlParameter("nrAddressesAffected", SqlDbType.Int) {Direction = ParameterDirection.Output},
            new SqlParameter("nrCategoriesAffected", SqlDbType.Int) {Direction = ParameterDirection.Output},
            new SqlParameter("nrReviewsAffected", SqlDbType.Int) {Direction = ParameterDirection.Output},
            new SqlParameter("nrUsersAffected", SqlDbType.Int) {Direction = ParameterDirection.Output}
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
                NrAttractionssWithAddress = Convert.ToInt32(reader["NrAttractionssWithAddress"]),
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

        return await DbInfo();
    }
}
