using System.Text.RegularExpressions;
using Models.Interfaces;

namespace Models.DTO;

public class AttractionCuDto
{
  public Guid? AttractionId {get; set;}
  public bool Seeded {get; set;} = true;
  public string Name {get; set;}
  public Guid? CategoryId {get; set;}
  public string Description {get; set;}
  public Guid? AddressId {get; set;}
  public List<Guid> ReviewsId {get; set;} = null;

  public AttractionCuDto() {}
  public AttractionCuDto(IAttraction model)
  {
    this.AttractionId = model.AttractionId;
    this.Name = model.Name;
    this.Description = model.Description;
    this.CategoryId = model?.Category?.CategoryId;
    this.AddressId = model?.Address?.AddressId;
    this.ReviewsId = model.Reviews?.Select(r => r.ReviewId).ToList();
  }
  public void EnsureValidity()
  {
    if(!string.IsNullOrEmpty(Name) && !Regex.IsMatch(Name,@"^[a-zA-Z0-9åäöÅÄÖ\s-,./]*$"))
    {
        throw new ArgumentException($"Attraction name can only contain letters (a-z), numbers (0-9), spaces, and the following special characters: - , . /");
      }
    if(!string.IsNullOrEmpty(Description) && !Regex.IsMatch(Description,@"^[a-zA-Z0-9åäöÅÄÖ\s-,./]*$"))
    {
        throw new ArgumentException($"Attraction description can only contain letters (a-z), numbers (0-9), spaces, and the following special characters: - , . /");
      }
    if(ReviewsId == null)
    {
        throw new ArgumentException($"Attraction reviews cannot be null.");
      }
    }
  }


public class ReviewCuDto
{
  public Guid? ReviewId {get; set;}
  public Guid UserId {get; set;}
  public Guid AttractionId {get; set;}
  public string Comment {get; set;}

  public ReviewCuDto() {}

  public ReviewCuDto(IReview model)
  {
    ReviewId = model.ReviewId;
    UserId = model.User.UserId;
    AttractionId = model.Attraction.AttractionId;
    Comment = model.Comment;
  }

    public void EnsureValidity()
  {
    if(!string.IsNullOrEmpty(Comment) && !Regex.IsMatch(Comment,@"^[a-zA-ZåäöÅÄÖ\s-]*$"))
    {
        throw new ArgumentException($"Comment can only contain letters (a-ö), spaces, and the following special characters: -");
      }
    }
}

public class UserCuDto
{
  public Guid? UserId {get; set;}
  public string FirstName {get; set;}
  public string LastName {get; set;}
  public string Email {get; set;}
  public List<Guid> ReviewsId {get; set;} = new List<Guid>();

  public UserCuDto() {}

  public UserCuDto(IUser model)
  {
    UserId = model.UserId;
    FirstName = model.FirstName;
    LastName = model.LastName;
    Email = model.Email;
    ReviewsId = model.Reviews.Select(r => r.ReviewId).ToList();
  }

  public void EnsureValidity()
  {
    if(!string.IsNullOrEmpty(FirstName) && !Regex.IsMatch(FirstName,@"^[a-zA-ZåäöÅÄÖ\s-]*$"))
    {
        throw new ArgumentException($"User first name can only contain letters (a-z), spaces, and the following special characters: -");
      }
    if(!string.IsNullOrEmpty(LastName) && !Regex.IsMatch(LastName,@"^[a-zA-ZåäöÅÄÖ\s-]*$"))
    {
        throw new ArgumentException($"User last name can only contain letters (a-z), spaces, and the following special characters: -");
      }
    if(!string.IsNullOrEmpty(Email) && !Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
    {
        throw new ArgumentException($"User email must be a valid email address.");
      }
    }
  
}