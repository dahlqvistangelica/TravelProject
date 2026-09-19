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

        public async Task<ResponseItemDto<IAttraction>> ReadAttractionAsync(Guid id, bool flat)
    {
        IAttraction item;
        if(!flat)
        {
            var query = _dbContext.Attractions.AsNoTracking()
            .Include(i => i.ReviewsDbM)
            .ThenInclude(i => i.UserDbM)
            .Include(i => i.AddressDbM)
            .Include(i => i.CategoryDbM)
            .Where(i => i.AttractionId == id);

            item = await query.FirstOrDefaultAsync<IAttraction>();
        }
        else
        {
            var query = _dbContext.Attractions.AsNoTracking()
            .Where(i => i.AttractionId == id);

            item = await query.FirstOrDefaultAsync<IAttraction>();
        }

        if(item == null)
        {
            throw new ArgumentException($"Attraction with id {id} not found");
        }
        return new ResponseItemDto<IAttraction>()
        {
            #if DEBUG
            ConnectionString = _dbContext.dbConnection,
            #endif

            Item = item
        };
        
    }
    public async Task<ResponseItemDto<IAttraction>> DeleteAttractionAsync(Guid id)
    {
        var query1 = _dbContext.Attractions
            .Where(i => i.AttractionId == id);

        var item = await query1.FirstOrDefaultAsync<AttractionDbM>();

        if(item == null)
        {
            throw new ArgumentException($"Attraction with id {id} not found");
        }

        _dbContext.Attractions.Remove(item);
        await _dbContext.SaveChangesAsync();
        return new ResponseItemDto<IAttraction>()
        {
            #if DEBUG
            ConnectionString = _dbContext.dbConnection,
            #endif

            Item = item
        };
    }

    public async Task<ResponseItemDto<IAttraction>> UpdateAttractionAsync (AttractionCuDto itemDto)
    {
        var query1 = _dbContext.Attractions
            .Where(i => i.AttractionId == itemDto.AttractionId);
        var item = await query1
        .Include(i => i.AddressDbM)
        .Include(i => i.CategoryDbM)
        .Include(i => i.ReviewsDbM)
        .FirstOrDefaultAsync<AttractionDbM>();

        if(item == null)
        {
            throw new ArgumentException($"Attraction with id {itemDto.AttractionId} not found");
        }



        item.UpdateFromDTO(itemDto);

        await navProp_AttractionCUdto_to_AttractionDbM(itemDto, item);

        _dbContext.Attractions.Update(item);
        await _dbContext.SaveChangesAsync();

        return await ReadAttractionAsync(item.AttractionId, false);

    }

    public async Task<ResponseItemDto<IAttraction>> CreateAttractionAsync (AttractionCuDto itemDto)
    {
        if(itemDto.AttractionId != null)
        {
            throw new ArgumentException($"Attraction with id {itemDto.AttractionId} already exists");
        }
        var query2 = _dbContext.Attractions
            .Where(i => (i.Name == itemDto.Name) && 
            (i.CategoryDbM.CategoryId == itemDto.CategoryId) &&
            (i.AddressDbM.AddressId == itemDto.AddressId));
        var existingItem = await query2.FirstOrDefaultAsync<AttractionDbM>();
        if(existingItem != null)
        {
            throw new ArgumentException($"Attraction with name {itemDto.Name} already exists");
        }
        var item = new AttractionDbM(itemDto);

        await navProp_AttractionCUdto_to_AttractionDbM(itemDto, item);

        _dbContext.Attractions.Add(item);
        await _dbContext.SaveChangesAsync();

        return await ReadAttractionAsync(item.AttractionId, false);
    }
    private async Task navProp_AttractionCUdto_to_AttractionDbM(AttractionCuDto itemDtoSrc, AttractionDbM itemDst)
    {
        itemDst.AddressDbM = (itemDtoSrc.AddressId != null) ? await _dbContext.Addresses.FirstOrDefaultAsync(
            i => (i.AddressId == itemDtoSrc.AddressId)) : null;

        itemDst.CategoryDbM = (itemDtoSrc.CategoryId != null) ? await _dbContext.Categories.FirstOrDefaultAsync(
            i => (i.CategoryId == itemDtoSrc.CategoryId)) : null;


        List<ReviewDbM> reviews = null;

        if(itemDtoSrc.ReviewsId != null)
        {
            reviews = new List<ReviewDbM>();
            foreach(var id in itemDtoSrc.ReviewsId)
            {
                var review = await _dbContext.Reviews
                .FirstOrDefaultAsync(i => i.ReviewId == id);
                if(review == null)
                {
                    throw new ArgumentException($"Review with id {id} not found");
                }
                    reviews.Add(review);
            }

        }
        itemDst.ReviewsDbM = reviews;
        
    }
}
