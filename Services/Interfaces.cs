using Models.DTO;
using Models.Interfaces;

namespace Services;

public interface IAdminService
{
  public Task<ResponseItemDto<GstUsrInfoAllDto>> RobustSeedingAsync();
  public Task<ResponseItemDto<GstUsrInfoAllDto>> DbInfoAsync();
  public Task<ResponseItemDto<GstUsrInfoAllDto>> RemoveSeedAsync(bool seeded);
}
public interface IAttractionService
{
  public Task<ResponsePageDto<IAttraction>> ReadAttractionsAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize);
  public Task<ResponseItemDto<IAttraction>> ReadAttractionAsync(Guid id, bool flat);
  public Task<ResponseItemDto<IAttraction>> CreateAttractionAsync(AttractionCuDto itemDto);
  public Task<ResponseItemDto<IAttraction>> UpdateAttractionAsync(AttractionCuDto itemDto);
  public Task<ResponseItemDto<IAttraction>> DeleteAttractionAsync(Guid id);
}
public interface IUserService
{
  public Task<ResponsePageDto<IUser>> ReadUsersAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize);
  public Task<ResponseItemDto<IUser>> ReadUserAsync(Guid id, bool flat);
  public Task<ResponseItemDto<IUser>> CreateUserAsync(UserCUdto itemDto);
  public Task<ResponseItemDto<IUser>> UpdateUserAsync(UserCUdto itemDto);
  public Task<ResponseItemDto<IUser>> DeleteUserAsync(Guid id);
}
public interface IReviewService
{
  public Task<ResponseItemDto<IReview>> ReadReviewAsync(Guid id, bool flat);
  public Task<ResponseItemDto<IReview>> CreateReviewAsync(ReviewCUdto itemDto);
  
}