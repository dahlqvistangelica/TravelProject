using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

using Models;
using Models.Interfaces;
using Seido.Utilities.SeedGenerator;

namespace DbModels;
[Table("Addresses", Schema = "supusr")]
public class AddressDbM : Address, ISeed<AddressDbM>, IEquatable<AddressDbM>
{
  [Key]
  public override Guid AddressId { get; set; }
  [Required]
  public override string Street { get; set; }
  [Required]
  public override int ZipCode { get; set; }
  [JsonIgnore]
  public Guid? CityId {get; set;}
  [NotMapped]
  public override ICity City {get => CityDbM; set => throw new NotImplementedException();}
  [JsonIgnore]
  [ForeignKey("CityId")]
  public CityDbM CityDbM {get; set;}
  
  #region Equatable implementation
  public bool Equals(AddressDbM other) => (other != null) && ((Street, ZipCode, City) ==
        (other.Street, other.ZipCode, other.City));

  public override bool Equals(object obj) => Equals(obj as AddressDbM);
  public override int GetHashCode() => (Street, ZipCode, City).GetHashCode();
  #endregion
  [JsonIgnore]
  public Guid? AttractionId {get; set;}
  [NotMapped]
  public override IAttraction Attraction { get => AttractionDbM; set => throw new NotImplementedException(); }
  [JsonIgnore]
  [ForeignKey("AttractionId")]
  public AttractionDbM AttractionDbM { get; set; } = null;

  public override AddressDbM Seed(SeedGenerator seeder)
  {
    base.Seed(seeder);
    return this;
  }

  public AddressDbM() {}
}