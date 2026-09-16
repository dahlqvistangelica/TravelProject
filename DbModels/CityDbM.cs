// using System.ComponentModel.DataAnnotations;
// using System.ComponentModel.DataAnnotations.Schema;
// using Newtonsoft.Json;

// using Models;
// using Models.Interfaces;
// using Seido.Utilities.SeedGenerator;

// namespace DbModels;
// [Table("Cities")]
// public class CityDbM : City, ISeed<CityDbM>
// {
//     [Key]
//     public override Guid CityId { get; set; }
 
//     public override string Name {get; set; }
//     [JsonIgnore]
//     public Guid? CountryId {get; set;}
//     [NotMapped]
//     public override ICountry Country { get => CountryDbM; set => throw new NotImplementedException(); }
//     [JsonIgnore]
//     [ForeignKey("CountryId")]
//     public CountryDbM CountryDbM {get; set;}
//     [NotMapped]
//     public override List<IAddress> Addresses {get => AddressesDbM.ToList<IAddress>(); set => throw new NotImplementedException();}
//     [JsonIgnore]
//     public List<AddressDbM> AddressesDbM {get; set;}
    
//     #region constructors
//     public CityDbM() { }
//     #endregion

//     public override CityDbM Seed(SeedGenerator seeder)
//     {
//         base.Seed(seeder);
//         return this;
//     }
// }
