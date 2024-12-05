using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UserService.Enums;

namespace UserService.Models;

public class Profile
{
    [Key]
    [Required]
    public int Id { get; set; }
    [Required]
    
    [ForeignKey("User")]
    public int UserId { get; set; }
    public required Sexuality Sexuality { get; set; }
    [Required]
    public required LookingFor LookingFor { get; set; }
    [Required]
    public required double Latitude { get; set; }
    [Required]
    public required double Longitude { get; set; }
    public DateTime? Birthday { get; set; }
    public double? Weight { get; set; }
    public double? Height { get; set; }
    public RelationStatus? RelationStatus { get; set; }
    public int? PartnerUserId { get; set; }
    public string? UserName { get; set; }

    // public Profile(Sexuality sexuality, DateTime? birthday, double? weight, double? height, RelationStatus? relationStatus, int? partnerUserId, LookingFor lookingFor)
    // {
    //     _sexuality = sexuality;
    //     _birthday = birthday;
    //     _weight = weight;
    //     _height = height;
    //     _relationStatus = relationStatus;
    //     _partnerUserId = partnerUserId;
    //     _lookingFor = lookingFor;
    // }
    //
    // public void UpdateSexuality(Sexuality sexuality) => _sexuality = sexuality;
    // public void UpdateDateTime(DateTime birthday) => _birthday = birthday;
    // public void UpdateWeight(double weight) => _weight = weight;
    // public void UpdateHeight(double height) => _height = height;
    // public void UpdateRelationStatus(RelationStatus relationStatus) => _relationStatus = relationStatus;
    // public void UpdatePartnerUserId(int userId) => _partnerUserId = userId;
    // public void UpdateLookingFor(LookingFor lookingFor) => _lookingFor = lookingFor;
    //
    // public Sexuality GetSexuality() => _sexuality;
    // public DateTime? GetBirthday() => _birthday;
    // public double? GetWeight() => _weight;
    // public double? GetHeight() => _height;
    // public RelationStatus? GetRelationStatus() => _relationStatus;
    // public int? GetPartnerUserId() => _partnerUserId;
    // public LookingFor GetLookingFor() => _lookingFor;
}
