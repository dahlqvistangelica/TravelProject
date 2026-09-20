using Models.Interfaces;
using Seido.Utilities.SeedGenerator;

namespace Models;

public class City : ICity, ISeed<City>, IEquatable<City>
{
  public virtual Guid CityId {get; set;}
  public virtual string Name {get; set;}
  public virtual ICountry Country {get; set;}
  public virtual bool Seeded {get; set;} = false;
  public virtual List<IAddress> Addresses {get; set;} 

  #region IEquatable
      public bool Equals(City other) => (other != null) && ((this.Name, this.Country) ==
        (other.Name, other.Country));

    public override bool Equals(object obj) => Equals(obj as City);
    public override int GetHashCode() => (Name, Country).GetHashCode();
    #endregion
  
  public City() {}
  public City(City org)
  {
    CityId = org.CityId;
    Name = org.Name;
    Country = org.Country;
    Addresses = (org.Addresses != null)? org.Addresses.Select(a => new Address((Address) a)).ToList<IAddress>() : null;
    Seeded = org.Seeded;
  }

  public virtual City Seed(SeedGenerator seeder)
  {
    Seeded = true;
    CityId = Guid.NewGuid();
    Name = seeder.City(Country?.Name);
    return this;
  }
}