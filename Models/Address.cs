using Models.Interfaces;
using Seido.Utilities.SeedGenerator;

namespace Models;

public class Address : IAddress, ISeed<Address>, IEquatable<Address>
{
  public virtual Guid AddressId {get; set;}
  public virtual Guid? AttractionId => Attraction?.AttractionId;
  public virtual string Street {get; set;}
  public virtual int ZipCode {get; set;}
  public virtual ICity City {get; set;}
  public virtual bool Seeded {get; set;} = false;
  public virtual IAttraction Attraction {get; set;}

  #region IEquatable
      public bool Equals(Address other) => (other != null) && ((this.Street, this.ZipCode, this.City) ==
        (other.Street, other.ZipCode, other.City));

    public override bool Equals(object obj) => Equals(obj as Address);
    public override int GetHashCode() => (Street, ZipCode, City).GetHashCode();
    #endregion
  public Address() {}
  public Address(Address org)
  {
    AddressId = org.AddressId;
    Street = org.Street;
    ZipCode = org.ZipCode;
    City = org.City;
    Attraction = org.Attraction;
    Seeded = org.Seeded;
  }

  public virtual Address Seed(SeedGenerator seeder)
  {
    Seeded = true;
    AddressId = Guid.NewGuid();
    Street = seeder.StreetAddress(City.Country.Name);
    ZipCode = seeder.ZipCode;
    
    return this;
  }

}