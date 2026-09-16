using Microsoft.Extensions.Logging;
using DbContext;
using Models.DTO;
using Models.Interfaces;
using DbModels;
using Microsoft.EntityFrameworkCore;

namespace DbRepos;

public class AttractionDbRepos
{
    private readonly ILogger<AttractionDbRepos> _logger;
    private readonly MainDbContext _dbContext;

    public AttractionDbRepos(ILogger<AttractionDbRepos> logger, MainDbContext context)
    {
        _logger = logger;
        _dbContext = context;
    }

    public async Task<ResponsePageDto<IAttraction>> ReadAttractionsAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize)
    {
        filter ??= "";
        IQueryable<AttractionDbM> query;
        if(flat)
        {
            query = _dbContext.Attractions.AsNoTracking();
        }
        else
        {
            query = _dbContext.Attractions.AsNoTracking()
                    .Include(i => i.AddressDbM)
                    .Include(i => i.CategoryDbM)
                    .Include(i => i.ReviewsDbM);
        }
        var ret = new ResponsePageDto<IAttraction>()
        {
            #if DEBUG
            ConnectionString = _dbContext.dbConnection,
            #endif

            DbItemsCount = await query
            .Where(i => (i.Seeded == seeded) &&
                        (i.Name.ToLower().Contains(filter)) ||
                        (i.CategoryDbM.CategoryName.ToLower().Contains(filter)) ||
                        (i.AddressDbM.City.ToLower().Contains(filter)) ||
                        (i.AddressDbM.Country.ToLower().Contains(filter))).CountAsync(),
            PageItems = await query
                        .Where(i => (i.Seeded == seeded) &&
                        (i.Name.ToLower().Contains(filter)) ||
                        (i.CategoryDbM.CategoryName.ToLower().Contains(filter)) ||
                        (i.AddressDbM.City.ToLower().Contains(filter)) ||
                        (i.AddressDbM.Country.ToLower().Contains(filter)))
                        
                        .Skip(pageNumber*pageSize)
                        .Take(pageSize)
                        .ToListAsync<IAttraction>(),

                        PageNr = pageNumber,
                        PageSize = pageSize
        };
        return ret;
    }
    public async Task<ResponsePageDto<IAttraction>> ReadAttractionsWithoutComments(bool seeded, bool flat, int pageNumber, int pageSize)
    {
        IQueryable<AttractionDbM> query;
        if(flat)
        {
            query = _dbContext.Attractions.AsNoTracking();
        }
        else
        {
            query = _dbContext.Attractions.AsNoTracking()
            .Include(i => i.ReviewsDbM)
            .Include(i => i.AddressDbM)
            .Include(i => i.CategoryDbM);
        }

        var ret = new ResponsePageDto<IAttraction>()
        {
            #if DEBUG
            ConnectionString = _dbContext.dbConnection,
            #endif

            DbItemsCount = await query
            .Where(i => (i.Seeded == seeded) && (i.ReviewsDbM.Count>0)).CountAsync(),
            PageItems = await query
            .Where(i => (i.Seeded == seeded) && (i.ReviewsDbM.Count>0))
            .Skip(pageNumber*pageSize)
            .Take(pageSize)
            .ToListAsync<IAttraction>(),

            PageNr = pageNumber,
            PageSize = pageSize
        };
        return ret;
    }
}
