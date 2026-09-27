using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

using Models;
using Models.Interfaces;
using Seido.Utilities.SeedGenerator;
using Models.DTO;

namespace DbModels;
[Table("Attractions", Schema = "supusr")]
public class AttractionDbM : Attraction, ISeed<AttractionDbM>
{
  [Key]
  public override Guid AttractionId { get; set; } 
  [Required]
  public override string Name { get; set; }
  [JsonIgnore]
  public Guid? CategoryId {get; set;}
  [NotMapped]
  public override ICategory Category { get => CategoryDbM; set=> throw new NotImplementedException(); }
  [JsonIgnore]
  [ForeignKey("CategoryId")]
  public CategoryDbM CategoryDbM {get; set;}
  [Column(TypeName = "varchar(max)")]
  public override string Description {get; set;}
  [JsonIgnore]
  public Guid? AddressId {get; set;}
  [NotMapped]
  public override IAddress Address {get => AddressDbM; set => throw new NotImplementedException();}
  [JsonIgnore]
  [ForeignKey("AddressId")]
  [InverseProperty(nameof(AddressDbM.AttractionDbM))]
  public AddressDbM AddressDbM {get; set;}
  [NotMapped]
  public override List<IReview> Reviews {get => ReviewsDbM.ToList<IReview>() ?? new(); set => throw new NotImplementedException();}
  [JsonIgnore]
  public List<ReviewDbM> ReviewsDbM {get; set;} = new();
  public AttractionDbM() {}

  public AttractionDbM(AttractionCuDto dto): this()
  {
    UpdateFromDTO(dto);
  }

  public AttractionDbM UpdateFromDTO(AttractionCuDto org)
  {
    if(org == null) return null;

    Name = org.Name;
    Description = org.Description;
    return this;
  }

  public override AttractionDbM Seed(SeedGenerator seeder)
  {
    base.Seed(seeder);
    return this;
  }

}
