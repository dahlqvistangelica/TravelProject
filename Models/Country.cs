using Models.Interfaces;
using Seido.Utilities.SeedGenerator;

namespace Models;

public class Country : ICountry, ISeed<Country>, IEquatable<Country>
{
  public virtual Guid CountryId {get; set;}
  public virtual string Name {get; set;}
  public virtual List<ICity> Cities {get; set;}
  public virtual bool Seeded {get; set;} = false;
    #region IEquatable
      public bool Equals(Country other) => (other != null) && ((this.Name) ==
        (other.Name));

    public override bool Equals(object obj) => Equals(obj as Country);
    public override int GetHashCode() => (Name).GetHashCode();
    #endregion
  public Country() {}
  public Country(Country org)
  {
    CountryId = org.CountryId;
    Name = org.Name;
    Cities = (org.Cities != null)? org.Cities.Select(c => new City((City) c)).ToList<ICity>() : null;
    Seeded = org.Seeded;
  }
  public virtual Country Seed(SeedGenerator seeder)
  {
    Seeded = true;
    CountryId = Guid.NewGuid();
    Name = seeder.Country;
    return this;
  }
}