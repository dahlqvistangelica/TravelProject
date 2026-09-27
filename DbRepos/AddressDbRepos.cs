using Microsoft.Extensions.Logging;
using DbContext;
using Models.DTO;
using Models.Interfaces;
using DbModels;
using Microsoft.EntityFrameworkCore;
using Azure;

namespace DbRepos;

public class AddressDbRepos
{
    private readonly ILogger<AddressDbRepos> _logger;
    private readonly MainDbContext _dbContext;

    public AddressDbRepos(ILogger<AddressDbRepos> logger, MainDbContext context)
    {
        _logger = logger;
        _dbContext = context;
    }
/// <summary>
/// Reads a paginated list of addresses from the database based on the provided parameters.
/// The method allows filtering by seeded status and a search filter, and supports both flat and detailed representations of the address data.
/// </summary>
/// <param name="seeded"></param>
/// <param name="flat"></param>
/// <param name="filter"></param>
/// <param name="pageNumber"></param>
/// <param name="pageSize"></param>
/// <returns></returns>
    public async Task<ResponsePageDto<IAddress>> ReadAddressesAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize)
    {
        filter??= "";
        IQueryable<AddressDbM> query;
        if(flat)
        {
            query = _dbContext.Addresses.AsNoTracking().Include(i => i.AttractionId);
        }
        else
        {
        query = _dbContext.Addresses.AsNoTracking()
            .Include(i => i.AttractionDbM)
            .Include(i => i.CityDbM)
            .ThenInclude(i => i.CountryDbM);
        }
        var ret = new ResponsePageDto<IAddress>()
        {
            #if DEBUG
            ConnectionString = _dbContext.dbConnection,
            #endif

            DbItemsCount = await query
            .Where(i => i.Seeded == seeded &&
                        (i.Street.ToLower().Contains(filter) ||
                         i.CityDbM.Name.ToLower().Contains(filter) ||
                         i.CityDbM.CountryDbM.Name.ToLower().Contains(filter)))
            .CountAsync(),

            PageItems = await query
            .Where(i => i.Seeded == seeded &&
                        (i.Street.ToLower().Contains(filter) ||
                         i.CityDbM.Name.ToLower().Contains(filter) ||
                         i.CityDbM.CountryDbM.Name.ToLower().Contains(filter)))
            .Skip(pageNumber * pageSize)
            .Take(pageSize)
            .ToListAsync<IAddress>(),

            PageNr = pageNumber,
            PageSize = pageSize
        };
        return ret;
    }
    /// <summary>
    /// Reads a single address from the database based on the provided ID and representation type (flat or detailed).
    /// If the address is not found, an ArgumentException is thrown.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="flat"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<ResponseItemDto<IAddress>> ReadAddressAsync(Guid id, bool flat)
    {
        IAddress item;
        if(!flat)
        {
            var query = _dbContext.Addresses.AsNoTracking()
            .Include(i => i.AttractionDbM)
            .Where(i => i.AddressId == id);
            item = await query.FirstOrDefaultAsync<IAddress>();
        }
        else
        {
            var query = _dbContext.Addresses.AsNoTracking()
            .Include(i => i.AttractionDbM)
            .Include(i => i.CityDbM)
            .ThenInclude(i => i.CountryDbM)
            .Where(i => i.AddressId == id);
            item = await query.FirstOrDefaultAsync<IAddress>();
        }
        if(item == null)
        {
            throw new ArgumentException($"Item {id} is not existing");
        }
        return new ResponseItemDto<IAddress>()
        {
            #if DEBUG
            ConnectionString = _dbContext.dbConnection,
            #endif

            Item = item
        };
    }
}
