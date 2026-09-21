using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

using Models;
using Models.Interfaces;
using Seido.Utilities.SeedGenerator;

namespace DbModels;
[Table("Countries", Schema = "supusr")]
public class CountryDbM : Country, ISeed<CountryDbM>, IEquatable<CountryDbM>
{
    [Key]
    public override Guid CountryId { get; set; }
 
    public override string Name {get; set; }
    
    [NotMapped]
    public override List<ICity> Cities {get => CitiesDbM.ToList<ICity>(); set => throw new NotImplementedException(); }
    [JsonIgnore]
    public List<CityDbM> CitiesDbM {get; set;}
      #region IEquatable
      public bool Equals(CountryDbM other) => (other != null) && ((this.Name) ==
        (other.Name));

    public override bool Equals(object obj) => Equals(obj as CountryDbM);
    public override int GetHashCode() => (Name).GetHashCode();
    #endregion
    
    #region constructors
    public CountryDbM() { }
    #endregion

    public override CountryDbM Seed(SeedGenerator seeder)
    {
        base.Seed(seeder);
        return this;
    }
}
