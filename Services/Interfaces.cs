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
  public Task<ResponsePageDto<IAttraction>> ReadAttractionsAsync(bool seeded, bool flat, string filterName, string filterDesc, string filterPlace, string filterCat, int pageNumber, int pageSize);
  public Task<ResponseItemDto<IAttraction>> ReadAttractionAsync(Guid id, bool flat);
  public Task<ResponsePageDto<IAttraction>> AttractionsWithoutReviewsAsync(bool seeded, bool flat, int pageNumber, int pageSize);
  public Task<ResponseItemDto<IAttraction>> CreateAttractionAsync(AttractionCuDto itemDto);
  public Task<ResponseItemDto<IAttraction>> UpdateAttractionAsync(AttractionCuDto itemDto);
  public Task<ResponseItemDto<IAttraction>> DeleteAttractionAsync(Guid id);
}
public interface IUserService
{
  public Task<ResponsePageDto<IUser>> ReadUsersAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize);
  public Task<ResponseItemDto<IUser>> ReadUserAsync(Guid id, bool flat);
  public Task<ResponseItemDto<IUser>> CreateUserAsync(UserCuDto itemDto);
  public Task<ResponseItemDto<IUser>> UpdateUserAsync(UserCuDto itemDto);
  public Task<ResponseItemDto<IUser>> DeleteUserAsync(Guid id);
}
public interface IReviewService
{
  public Task<ResponseItemDto<IReview>> ReadReviewAsync(Guid id, bool flat);
  public Task<ResponseItemDto<IReview>> CreateReviewAsync(ReviewCuDto itemDto);
  public Task<ResponseItemDto<IReview>> DeleteReviewAsync(Guid id);
}
public interface IAddressService
{
  public Task<ResponsePageDto<IAddress>> ReadAddressesAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize);
  public Task<ResponseItemDto<IAddress>> ReadAddressAsync(Guid id, bool flat);
}