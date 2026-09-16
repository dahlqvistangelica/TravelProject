using Models.Interfaces;
using Seido.Utilities.SeedGenerator;

namespace Models;

public class Review : IReview, ISeed<Review>, IEquatable<Review>
{
  public virtual Guid ReviewId { get; set; }
  public virtual IUser User {get; set; }
  public virtual IAttraction Attraction {get; set;}
  public virtual string Comment {get; set;}
  public virtual bool Seeded {get; set;} = false;

    #region IEquatable
   public bool Equals(Review other) => (other != null) && ((this.User, this.Comment, this.Attraction) ==
        (other.User, other.Comment, other.Attraction));

    public override bool Equals(object obj) => Equals(obj as Review);
    public override int GetHashCode() => (User, Attraction, Comment).GetHashCode();
    #endregion

  public Review() {}

  public Review(Review org)
  {
    ReviewId = org.ReviewId;
    User = org.User;
    Attraction = org.Attraction;
    Comment = org.Comment;
    Seeded = org.Seeded;
  }

  public virtual Review Seed(SeedGenerator seeder)
  {
    Seeded = true;
    ReviewId = Guid.NewGuid();
    Comment = seeder.Comment;
    return this;
  }

}