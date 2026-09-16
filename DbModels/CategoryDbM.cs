using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

using Models;
using Models.Interfaces;
using Seido.Utilities.SeedGenerator;

namespace DbModels;
[Table("Categories", Schema = "supusr")]
public class CategoryDbM : Category, ISeed<CategoryDbM>, IEquatable<CategoryDbM>
{
    [Key]
    public override Guid CategoryId { get; set; }
    public override string CategoryName { get; set; }
    public override bool Seeded {get; set;}
    [NotMapped]
    public override List<IAttraction> Attractions {get => AttractionsDbM.ToList<IAttraction>(); set => throw new NotImplementedException();}
    [JsonIgnore]
    public List<AttractionDbM> AttractionsDbM {get;set;}

      #region Equatable implementation
  public bool Equals(CategoryDbM other) => (other != null) && ((CategoryName) ==
        (other.CategoryName));

  public override bool Equals(object obj) => Equals(obj as CategoryDbM);
  public override int GetHashCode() => (CategoryName).GetHashCode();
  #endregion
    #region constructors
    public CategoryDbM() { }
    #endregion
        
    

    public override CategoryDbM Seed(SeedGenerator seeder)
    {
        base.Seed(seeder);
        return this;
    }
}
