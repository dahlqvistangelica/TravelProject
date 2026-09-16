using Microsoft.Extensions.Logging;
using Models;
using DbRepos;
using Models.DTO;
using Models.Interfaces;

namespace Services;

public class AttractionServiceDb: IAttractionService
{
  private readonly ILogger<AttractionServiceDb> _logger = null;
  private readonly AttractionDbRepos _repo = null;

  public AttractionServiceDb(AttractionDbRepos repo)
  {
    _repo = repo;
  }
  public AttractionServiceDb(AttractionDbRepos repo, ILogger<AttractionServiceDb> logger)
  {
    _logger = logger;
    _repo = repo;
  }

  public Task<ResponsePageDto<IAttraction>> ReadAttractionsAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize) => _repo.ReadAttractionsAsync(seeded, flat, filter, pageNumber, pageSize);

}