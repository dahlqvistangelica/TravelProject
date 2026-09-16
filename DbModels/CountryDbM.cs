// using System.ComponentModel.DataAnnotations;
// using System.ComponentModel.DataAnnotations.Schema;
// using Newtonsoft.Json;

// using Models;
// using Models.Interfaces;
// using Seido.Utilities.SeedGenerator;

// namespace DbModels;
// [Table("Countries")]
// public class CountryDbM : Country, ISeed<CountryDbM>
// {
//     [Key]
//     public override Guid CountryId { get; set; }
 
//     public override string Name {get; set; }
//     public override string Description {get; set;}
//     [NotMapped]
//     public override List<ICity> Cities {get => CitiesDbM.ToList<ICity>(); set => throw new NotImplementedException(); }
//     [JsonIgnore]
//     public List<CityDbM> CitiesDbM {get; set;}
    
//     #region constructors
//     public CountryDbM() { }
//     #endregion

//     public override CountryDbM Seed(SeedGenerator seeder)
//     {
//         base.Seed(seeder);
//         return this;
//     }
// }
