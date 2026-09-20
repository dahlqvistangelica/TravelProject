using Models.Interfaces;
using Seido.Utilities.SeedGenerator;

namespace Models;

public class Address : IAddress, ISeed<Address>, IEquatable<Address>
{
  public virtual Guid AddressId {get; set;}
  public virtual string Street {get; set;}
  public virtual int ZipCode {get; set;}
  public virtual ICity City {get; set;}
  public virtual ICountry Country {get; set;}
  public virtual bool Seeded {get; set;} = false;
  public virtual IAttraction Attraction {get; set;}

  #region IEquatable
      public bool Equals(Address other) => (other != null) && ((this.Street, this.ZipCode, this.City, this.Country) ==
        (other.Street, other.ZipCode, other.City, other.Country));

    public override bool Equals(object obj) => Equals(obj as Address);
    public override int GetHashCode() => (Street, ZipCode, City, Country).GetHashCode();
    #endregion
  public Address() {}
  public Address(Address org)
  {
    AddressId = org.AddressId;
    Street = org.Street;
    ZipCode = org.ZipCode;
    City = org.City;
    Country = org.Country;
    Attraction = org.Attraction;
    Seeded = org.Seeded;
  }

  public virtual Address Seed(SeedGenerator seeder)
  {
    Seeded = true;
    AddressId = Guid.NewGuid();
    Country.Name = seeder.Country;
    City.Name = seeder.City(Country.Name);
    Street = seeder.StreetAddress(Country.Name);
    ZipCode = seeder.ZipCode;
    
    return this;
  }

}