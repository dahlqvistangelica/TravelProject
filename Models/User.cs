using Models.Interfaces;
using Seido.Utilities.SeedGenerator;
namespace Models;

public class User : IUser, ISeed<User>, IEquatable<User>
{
  public virtual Guid UserId { get; set; }
  public virtual string FirstName { get; set; }
  public virtual string LastName { get; set; }
  public virtual string Email {get; set;}
  public virtual List<IReview> Reviews {get; set;} = new List<IReview>();
  public virtual bool Seeded {get; set;} = false;

      #region IEquatable
   public bool Equals(User other) => (other != null) && ((this.FirstName, this.LastName, this.Email) ==
        (other.FirstName, other.LastName, other.Email));

    public override bool Equals(object obj) => Equals(obj as User);
    public override int GetHashCode() => (FirstName,LastName,Email).GetHashCode();
    #endregion

  public User() {}

  public User(User org)
  {
    UserId = org.UserId;
    FirstName = org.FirstName;
    LastName = org.LastName;
    Email = org.Email;
    Reviews = (org.Reviews != null)? org.Reviews.Select(r => new Review((Review) r)).ToList<IReview>() : null;
    Seeded = org.Seeded;
  }
  public virtual User Seed(SeedGenerator seeder)
  {
    Seeded = true;
    UserId = Guid.NewGuid();
    FirstName = seeder.FirstName;
    LastName = seeder.LastName;
    Email = seeder.Email(FirstName, LastName);
    return this;
  }
}