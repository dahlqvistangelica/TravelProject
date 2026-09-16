using System.Text.RegularExpressions;
using Models.Interfaces;

namespace Models.DTO;

public class AttractionCUdto
{
  public Guid? AttractionId {get; set;}
  public bool Seeded {get; set;} = true;
  public string Name {get; set;}
  public Guid CategoryId {get; set;}
  public string Description {get; set;}
  public Guid AddressId {get; set;}
  public List<Guid> ReviewsId {get; set;} = new List<Guid>();

  public AttractionCUdto() {}
  public AttractionCUdto(IAttraction model)
  {
    this.AttractionId = model.AttractionId;
    this.Name = model.Name;
    this.Description = model.Description;
    this.CategoryId = model.Category.CategoryId;
    this.AddressId = model.Address.AddressId;
    this.ReviewsId = model.Reviews.Select(r => r.ReviewId).ToList();
  }
}

public class ReviewCUdto
{
  public Guid? ReviewId {get; set;}
  public Guid UserId {get; set;}
  public Guid AttractionId {get; set;}
  public string Comment {get; set;}

  public ReviewCUdto() {}

  public ReviewCUdto(IReview model)
  {
    ReviewId = model.ReviewId;
    UserId = model.User.UserId;
    AttractionId = model.Attraction.AttractionId;
    Comment = model.Comment;
  }
}

public class UserCUdto
{
  public Guid? UserId {get; set;}
  public string FirstName {get; set;}
  public string LastName {get; set;}
  public string Email {get; set;}
  public List<Guid> Reviews {get; set;} = new List<Guid>();

  public UserCUdto() {}

  public UserCUdto(IUser model)
  {
    UserId = model.UserId;
    FirstName = model.FirstName;
    LastName = model.LastName;
    Email = model.Email;
    Reviews = model.Reviews.Select(r => r.ReviewId).ToList();
  }
}