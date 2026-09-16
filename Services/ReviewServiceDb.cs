using Microsoft.Extensions.Logging;
using Models;
using DbRepos;

namespace Services;

public class ReviewServiceDb: IReviewService
{
  private readonly ILogger<ReviewServiceDb> _logger = null;
  private readonly ReviewDbRepos _repo = null;

  public ReviewServiceDb(ReviewDbRepos repo)
  {
    _repo = repo;
  }
  public ReviewServiceDb(ReviewDbRepos repo, ILogger<ReviewServiceDb> logger)
  {
    _logger = logger;
    _repo = repo;
  }

}