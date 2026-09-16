namespace Models.Interfaces;

public interface IAttraction
{
  public Guid AttractionId { get; set; }
  public string Name { get; set; }
  public ICategory Category {get; set;}
  public string Description {get; set;}
  public IAddress Address {get; set;}
  public List<IReview> Reviews {get; set;}
}

public interface ICategory
{
  public Guid CategoryId {get;set;}
  public string CategoryName {get; set;}
  public List<IAttraction> Attractions {get; set;}
}

public interface IUser
{
  public Guid UserId {get; set;}
  public string FirstName {get; set;}
  public string LastName {get; set;}
  public string Email {get; set;}
  public List<IReview> Reviews {get; set;}
}

public interface IReview
{
  public Guid ReviewId {get; set;}
  public IUser User {get; set; }
  public IAttraction Attraction {get; set;}
  public string Comment {get; set;}
}

// public interface ICity
// {
//   public Guid CityId {get;set;}
//   public ICountry Country {get;set;}
//   public string Name {get; set;}
//   public List<IAddress> Addresses {get; set;}
// }

// public interface ICountry
//   {
//     public Guid CountryId {get; set;}

//     public string Name {get; set;}
//     public string Description {get; set;}
//     public List<ICity> Cities {get; set;}

//   }

public interface IAddress
{
  public Guid AddressId {get; set;}
  public string Street {get; set;}
  public int ZipCode {get; set;}
  public string City {get; set;}
  public string Country {get; set;}
  public IAttraction Attraction {get; set;}
}