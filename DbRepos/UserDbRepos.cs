using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Data;

using DbContext;
using Models.DTO;
using Models.Interfaces;
using DbModels;

namespace DbRepos;

public class UserDbRepos
{
    private ILogger<UserDbRepos> _logger;
    private readonly MainDbContext _dbContext;

    public UserDbRepos(ILogger<UserDbRepos> logger, MainDbContext context)
    {
        _logger = logger;
        _dbContext = context;
    }
    /// <summary>
    /// Reads a paginated list of users from the database based on the provided parameters.
    /// The method allows filtering by seeded status and a search filter, and supports both flat and
    /// detailed representations of the user data.
    /// </summary>    
    public async Task<ResponsePageDto<IUser>> ReadUsersAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize)
    {
        filter ??= "";
        IQueryable<UserDbM> query;
        if(flat)
        {
            query = _dbContext.Users.AsNoTracking();
        }
        else
        {
            query = _dbContext.Users.AsNoTracking()
                    .Include(i => i.ReviewsDbM)
                    .ThenInclude(i => i.AttractionDbM);
        }
        var ret = new ResponsePageDto<IUser>()
        {
            #if DEBUG
            ConnectionString = _dbContext.dbConnection,
            #endif

            DbItemsCount = await query
            .Where(i => (i.Seeded == seeded) &&
                        ((i.FirstName.ToLower().Contains(filter)) ||
                         (i.LastName.ToLower().Contains(filter)) ||
                         (i.Email.ToLower().Contains(filter)))).CountAsync(),
            PageItems = await query
                        .Where(i => (i.Seeded == seeded) &&
                        ((i.FirstName.ToLower().Contains(filter)) ||
                         (i.LastName.ToLower().Contains(filter)) ||
                         (i.Email.ToLower().Contains(filter))))
                        
                        .Skip(pageNumber*pageSize)
                        .Take(pageSize)
                        .ToListAsync<IUser>(),

                        PageNr = pageNumber,
                        PageSize = pageSize
        };
        return ret;
    }
    /// <summary>
    /// Reads a single user from the database based on the provided ID and representation type (flat or detailed).
    /// If the user is not found, an ArgumentException is thrown.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="flat"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<ResponseItemDto<IUser>> ReadUserAsync(Guid id, bool flat)
    {
        IUser item;
        if(flat)
        {
            item = await _dbContext.Users.AsNoTracking()
                .FirstOrDefaultAsync(i => i.UserId == id);
        }
        else
        {
            item = await _dbContext.Users.AsNoTracking()
                .Include(i => i.ReviewsDbM)
                .ThenInclude(i => i.AttractionDbM)
                .FirstOrDefaultAsync(i => i.UserId == id);
        }

        if(item == null)
        {
            throw new ArgumentException($"User with {id} not found");
        }

        return new ResponseItemDto<IUser>()
        {
            #if DEBUG
            ConnectionString = _dbContext.dbConnection,
            #endif

            Item = item
        };
    }

    /// <summary>
    /// Updates an existing user in the database based on the provided UserCuDto.
    /// If the user does not exist, an ArgumentException is thrown.
    /// </summary>
    /// 
    public async Task<ResponseItemDto<IUser>> UpdateUserAsync(UserCuDto itemDto)
    {
        var query1 = _dbContext.Users
            .Where(i => i.UserId == itemDto.UserId);
        var item = await query1
            .Include(i => i.ReviewsDbM)
            .FirstOrDefaultAsync<UserDbM>();

        if(item == null) throw new ArgumentException($"User with {itemDto.UserId} not found");

        var query2 = _dbContext.Users
            .Where(i => ((i.FirstName == itemDto.FirstName) && 
                  (i.LastName == itemDto.LastName) && 
                  (i.Email == itemDto.Email)));
        var existingUser = await query2.FirstOrDefaultAsync<UserDbM>();
        if(existingUser != null && existingUser.UserId != itemDto.UserId)
        {
            throw new ArgumentException($"User with {existingUser.UserId} already exists");
        }

        item.UpdateFromDTO(itemDto);

        await navProp_UserCUdto_to_UserDbM(itemDto, item);
        _dbContext.Users.Update(item);
        await _dbContext.SaveChangesAsync();
        return await ReadUserAsync(item.UserId, false);
    }

/// <summary>
/// Deletes a user from the database based on the provided ID.
/// If the user is not found, an ArgumentException is thrown.
/// </summary>
/// <param name="itemDto"></param>
/// <returns></returns>
/// <exception cref="ArgumentException"></exception>
    public async Task<ResponseItemDto<IUser>> CreateUserAsync(UserCuDto itemDto)
    {
        if(itemDto.UserId != null)
        {
            throw new ArgumentException($"User with id {itemDto.UserId} already exists");
        }
        var query2 = _dbContext.Users.
            Where(i => ((i.FirstName == itemDto.FirstName) && 
                  (i.LastName == itemDto.LastName) && 
                  (i.Email == itemDto.Email)));
        var existingUser = await query2.FirstOrDefaultAsync<UserDbM>();
        if(existingUser != null)
        {
            throw new ArgumentException($"User with {existingUser.UserId} already exists");
        }
        var item = new UserDbM(itemDto);
        await navProp_UserCUdto_to_UserDbM(itemDto, item);
        _dbContext.Users.Add(item);
        await _dbContext.SaveChangesAsync();
        return await ReadUserAsync(item.UserId, false);
    }

/// <summary>
/// Deletes a user from the database based on the provided ID.
/// If the user is not found, an ArgumentException is thrown.
/// </summary>
/// <param name="id"></param>
/// <returns></returns>
/// <exception cref="ArgumentException"></exception>
    public async Task<ResponseItemDto<IUser>> DeleteUserAsync(Guid id)
    {
        var query1 = _dbContext.Users
                    .Where(i => i.UserId == id);

        var item = await query1.FirstOrDefaultAsync<UserDbM>();

        if(item == null)
        {
            throw new ArgumentException($"User with id {id} not found");
        }

        _dbContext.Users.Remove(item);
        await _dbContext.SaveChangesAsync();
        return new ResponseItemDto<IUser>()
        {
            #if DEBUG
            ConnectionString = _dbContext.dbConnection,
            #endif

            Item = item
        };
    }

/// <summary>
/// Updates an existing user in the database based on the provided UserCuDto.
/// If the user does not exist, an ArgumentException is thrown.
/// </summary>
/// <param name="itemDtoSrc"></param>
/// <param name="itemDst"></param>
/// <returns></returns>
/// <exception cref="ArgumentException"></exception>
    public async Task navProp_UserCUdto_to_UserDbM(UserCuDto itemDtoSrc, UserDbM itemDst)
    {
        List<ReviewDbM> reviews = null;
        if(itemDtoSrc.ReviewsId != null)
        {
            reviews = new List<ReviewDbM>();
            foreach(var id in itemDtoSrc.ReviewsId)
            {
            
                var r = await _dbContext.Reviews.FirstOrDefaultAsync(i => i.ReviewId == id);
                if(r == null)
                {
                    throw new ArgumentException($"Review with {id} not found");
                }
                reviews.Add(r);
            }
        }
        itemDst.ReviewsDbM = reviews;
    }
}
