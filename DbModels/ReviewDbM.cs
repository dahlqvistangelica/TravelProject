using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

using Models;
using Models.Interfaces;
using Seido.Utilities.SeedGenerator;
using Models.DTO;

namespace DbModels;
[Table("Reviews", Schema = "supusr")]
public class ReviewDbM : Review, ISeed<ReviewDbM>, IEquatable<ReviewDbM>
{
    [Key]
    public override Guid ReviewId { get; set; }
    [JsonIgnore]
    public Guid UserId {get; set;}
    [NotMapped]
    public override IUser User {get => UserDbM; set => throw new NotImplementedException(); }
    [JsonIgnore]
    [ForeignKey("UserId")]
    public UserDbM UserDbM {get; set;} = null;
    [JsonIgnore]
    public Guid AttractionId {get; set;}
    [NotMapped]
    public override IAttraction Attraction {get => AttractionDbM; set => throw new NotImplementedException();}
    [JsonIgnore]
    [ForeignKey("AttractionId")]
    public AttractionDbM AttractionDbM {get; set;} = null;
    [Column(TypeName = "varchar(max)")]
    public override string Comment {get; set;}

      #region Equatable implementation
  public bool Equals(ReviewDbM other) => (other != null) && ((User, Attraction, Comment) ==
        (other.User, other.Attraction, other.Comment));

  public override bool Equals(object obj) => Equals(obj as ReviewDbM);
  public override int GetHashCode() => (User,Attraction,Comment).GetHashCode();
  #endregion
    
    #region constructors
    public ReviewDbM(){ }

    public ReviewDbM(ReviewCuDto dto): this()
    {
        UpdateFromDTO(dto);
    }
    #endregion

    public ReviewDbM UpdateFromDTO(ReviewCuDto org)
    {
        if(org == null) return null;
        Comment = org.Comment;
        return this;
    }
    public override ReviewDbM Seed(SeedGenerator seeder)
    {
        base.Seed(seeder);
        return this;
    }
}


