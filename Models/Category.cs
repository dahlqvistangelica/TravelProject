using Models.Interfaces;
using Seido.Utilities.SeedGenerator;

namespace Models;

public class Category :ICategory, ISeed<Category>, IEquatable<Category>
{
   public virtual Guid CategoryId {get; set;}
   public virtual string CategoryName {get; set;}
   public virtual List<IAttraction> Attractions {get; set;}

   public virtual bool Seeded {get; set;} = false;
     #region Equatable implementation
  public bool Equals(Category other) => (other != null) && ((CategoryName) ==
        (other.CategoryName));

  public override bool Equals(object obj) => Equals(obj as Category);
  public override int GetHashCode() => (CategoryName).GetHashCode();
  #endregion
   public virtual Category Seed(SeedGenerator seeder)
   {
      Seeded = true;
      CategoryId = Guid.NewGuid();
      CategoryName = seeder.AttractionCategory;
      return this;
   }
}