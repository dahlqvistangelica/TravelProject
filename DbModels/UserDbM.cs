using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

using Models;
using Models.Interfaces;
using Seido.Utilities.SeedGenerator;
using Models.DTO;

namespace DbModels;
[Table("Users", Schema = "supusr")]
public class UserDbM : User, ISeed<UserDbM>, IEquatable<UserDbM>
{
    [Key]
    public override Guid UserId { get; set; }
    
    public override string FirstName { get; set; }
    public override string LastName { get; set; }
    public override string Email {get; set;}
    [NotMapped]
    public override List<IReview> Reviews {get => ReviewsDbM.ToList<IReview>(); set => throw new NotImplementedException();}
    
    [JsonIgnore]
    public List<ReviewDbM> ReviewsDbM {get; set;}

      #region Equatable implementation
  public bool Equals(UserDbM other) => (other != null) && ((FirstName, LastName, Email) ==
        (other.FirstName, other.LastName, other.Email));

  public override bool Equals(object obj) => Equals(obj as UserDbM);
  public override int GetHashCode() => (FirstName, LastName, Email).GetHashCode();
  #endregion
    
    #region constructors
    public UserDbM() { }

    public UserDbM(UserCUdto dto): this()
    {
        UpdateFromDTO(dto);
    }

    public UserDbM UpdateFromDTO(UserCUdto org)
    {
        if(org == null) return null;
        FirstName = org.FirstName;
        LastName = org.LastName;
        Email = org.Email;
        return this;
    }
    public override UserDbM Seed(SeedGenerator seeder)
    {
        base.Seed(seeder);
        return this;
    }
    #endregion
}