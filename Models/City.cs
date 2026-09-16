// using Models.Interfaces;
// using Seido.Utilities.SeedGenerator;

// namespace Models;

// public class City : ICity, ISeed<City>
// {
//   public virtual Guid CityId {get; set;}
//   public virtual string Name {get; set;}
//   public virtual ICountry Country {get; set;}
//   public virtual bool Seeded {get; set;} = false;
//   public virtual List<IAddress> Addresses {get; set;} 

//   public City() {}
//   public City(City org)
//   {
//     CityId = org.CityId;
//     Name = org.Name;
//     Country = org.Country;
//     Addresses = (org.Addresses != null)? org.Addresses.Select(a => new Address((Address) a)).ToList<IAddress>() : null;
//     Seeded = org.Seeded;
//   }

//   public virtual City Seed(SeedGenerator seeder)
//   {
//     Seeded = true;
//     CityId = Guid.NewGuid();
//     Name = seeder.City();
//     return this;
//   }
// }