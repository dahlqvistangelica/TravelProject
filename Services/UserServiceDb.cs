using Microsoft.Extensions.Logging;
using Models;
using Models.DTO;
using Models.Interfaces;
using DbRepos;

namespace Services;

public class UserServiceDb: IUserService
{
  private readonly ILogger<UserServiceDb> _logger = null;
  private readonly UserDbRepos _repo = null;

  public UserServiceDb(UserDbRepos repo)
  {
    _repo = repo;
  }
  public UserServiceDb(UserDbRepos repo, ILogger<UserServiceDb> logger)
  {
    _logger = logger;
    _repo = repo;
  }

  public Task<ResponsePageDto<IUser>> ReadUsersAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize) => _repo.ReadUsersAsync(seeded, flat, filter, pageNumber, pageSize);
  public Task<ResponseItemDto<IUser>> ReadUserAsync(Guid id, bool flat) => _repo.ReadUserAsync(id, flat);
  public Task<ResponseItemDto<IUser>> CreateUserAsync(UserCuDto itemDto) => _repo.CreateUserAsync(itemDto);
  public Task<ResponseItemDto<IUser>> UpdateUserAsync(UserCuDto itemDto) => _repo.UpdateUserAsync(itemDto);
  public Task<ResponseItemDto<IUser>> DeleteUserAsync(Guid id) => _repo.DeleteUserAsync(id);  

}