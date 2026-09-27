using Models.Interfaces;
using Seido.Utilities.SeedGenerator;

namespace Models;

public class Attraction : IAttraction, ISeed<Attraction>
{
  public virtual Guid AttractionId { get; set; }
  public virtual string Name { get; set; }
  public virtual ICategory Category { get; set; }
  public virtual string Description {get; set;}
  public virtual IAddress Address {get; set;}
  public virtual List<IReview> Reviews {get; set;}
  public virtual bool Seeded {get; set;} = false;

  public Attraction() {}

  public Attraction(Attraction org)
  {
    AttractionId = org.AttractionId;
    Name         = org.Name;
    Category     = org.Category;
    Address = org.Address;
    Description = org.Description;
    Reviews = org.Reviews;
    Seeded = org.Seeded;
  }

  public virtual Attraction Seed(SeedGenerator seeder)
  {
    Seeded = true;
    AttractionId = Guid.NewGuid();
    Name = $"{seeder.AttractionFirstName} {seeder.AttractionSecondName}";
    Description = seeder.DescSentence;
    return this;
  }

}
