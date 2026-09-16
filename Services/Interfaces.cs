using Models.DTO;
using Models.Interfaces;

namespace Services;

public interface IAdminService
{
  public Task SeedAsync(int seedCount);
}
public interface IAttractionService
{
  public Task<ResponsePageDto<IAttraction>> ReadAttractionsAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize);
}
public interface IUserService
{

}
public interface IReviewService
{
  
}