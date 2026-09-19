using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Data;

using DbContext;
using Models.DTO;
using Models.Interfaces;
using DbModels;

namespace DbRepos;

public class ReviewDbRepos
{
    private ILogger<ReviewDbRepos> _logger;
    private readonly MainDbContext _dbContext;

    public ReviewDbRepos(ILogger<ReviewDbRepos> logger, MainDbContext context)
    {
        _logger = logger;
        _dbContext = context;
    }
    public async Task<ResponseItemDto<IReview>> ReadReviewAsync(Guid id, bool flat)
    {
        IReview item;
        if(!flat)
        {
            var query = _dbContext.Reviews.AsNoTracking()
                    .Include(i => i.UserDbM)
                    .Include(i => i.AttractionDbM)
                    .ThenInclude(i => i.AddressDbM)
                    .Where(i => i.ReviewId == id);

            item = await query.FirstOrDefaultAsync<IReview>();
        }
        else
        {
            var query = _dbContext.Reviews.AsNoTracking()
                    .Where(i => i.ReviewId == id);

            item = await query.FirstOrDefaultAsync<IReview>();
        }
        if(item == null)
        {
            throw new ArgumentException($"Review with id {id} not found");
        }
        return new ResponseItemDto<IReview>()
        {
            #if DEBUG
            ConnectionString = _dbContext.dbConnection,
            #endif
            Item = item
        };
    }
    public async Task<ResponseItemDto<IReview>> CreateReviewAsync(ReviewCUdto itemDto)
    {
        if(itemDto.ReviewId != null)
        {
            throw new ArgumentException($"ReviewId must be null for creation");
        }

        var query2 = _dbContext.Reviews
            .Where(i => (i.UserDbM.UserId == itemDto.UserId) && (i.AttractionDbM.AttractionId == itemDto.AttractionId));
        var existingReview = await query2.FirstOrDefaultAsync<ReviewDbM>();
        if(existingReview != null)
        {
            throw new ArgumentException($"Review for user {itemDto.UserId} and attraction {itemDto.AttractionId} already exists");
        }
        var item = new ReviewDbM(itemDto);
        await navProp_ReviewCUdto_to_ReviewDbM(itemDto, item);
        _dbContext.Reviews.Add(item);
        await _dbContext.SaveChangesAsync();
        return await ReadReviewAsync(item.ReviewId, false);

    }

    public async Task navProp_ReviewCUdto_to_ReviewDbM(ReviewCUdto itemDtoSrc, ReviewDbM itemDst)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(
            u => (u.UserId == itemDtoSrc.UserId));
        
        if(user == null)
        {
            throw new ArgumentException($"User with id {itemDtoSrc.UserId} not found");
        }
        itemDst.UserDbM = user;
    }
}
