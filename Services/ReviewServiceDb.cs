using Microsoft.Extensions.Logging;
using Models;
using DbRepos;
using Models.DTO;
using Models.Interfaces;
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

  public Task<ResponseItemDto<IReview>> ReadReviewAsync(Guid id, bool flat) => _repo.ReadReviewAsync(id, flat);
  public Task<ResponseItemDto<IReview>> CreateReviewAsync(ReviewCUdto itemDto) => _repo.CreateReviewAsync(itemDto);
  public Task<ResponseItemDto<IReview>> DeleteReviewAsync(Guid id) => _repo.DeleteReviewAsync(id);
}