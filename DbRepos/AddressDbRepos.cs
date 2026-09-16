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

    public async Task<ResponsePageDto<IAddress>> ReadAddressesAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize)
    {
        filter??= "";
        IQueryable<AddressDbM> query = flat ?
        _dbContext.Addresses.AsNoTracking()
        : _dbContext.Addresses.AsNoTracking().Include(i => i.AttractionDbM);

        var ret = new ResponsePageDto<IAddress>()
        {
            #if DEBUG
            ConnectionString = _dbContext.dbConnection,
            #endif

            DbItemsCount = await query
            .Where(i => (i.Seeded == seeded) && ((i.Street.ToLower().Contains(filter)) || (i.City.ToLower().Contains(filter)) || (i.Country.ToLower().Contains(filter))))
            .CountAsync(),

            PageItems = await query
            .Where(i => (i.Seeded == seeded) && ((i.Street.ToLower().Contains(filter)) || (i.City.ToLower().Contains(filter)) || (i.Country.ToLower().Contains(filter))))
            .Skip(pageNumber * pageSize)
            .Take(pageSize)
            .ToListAsync<IAddress>(),

            PageNr = pageNumber,
            PageSize = pageSize
        };
        return ret;
    }

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
