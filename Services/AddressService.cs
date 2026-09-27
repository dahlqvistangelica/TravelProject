using Microsoft.Extensions.Logging;
using Models;
using DbRepos;
using Models.DTO;
using Models.Interfaces;
namespace Services;

public class AddressServiceDb: IAddressService
{
  private readonly ILogger<AddressServiceDb> _logger = null;
  private readonly AddressDbRepos _repo = null;

  public AddressServiceDb(AddressDbRepos repo)
  {
    _repo = repo;
  }
  public AddressServiceDb(AddressDbRepos repo, ILogger<AddressServiceDb> logger)
  {
    _logger = logger;
    _repo = repo;
  }

  public Task<ResponseItemDto<IAddress>> ReadAddressAsync(Guid id, bool flat) => _repo.ReadAddressAsync(id, flat);
  public Task<ResponsePageDto<IAddress>> ReadAddressesAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize) => _repo.ReadAddressesAsync(seeded, flat, filter, pageNumber, pageSize);
}