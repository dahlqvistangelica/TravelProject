using Microsoft.Extensions.Logging;
using Models;
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

}