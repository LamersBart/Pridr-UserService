using System.Runtime.Serialization;

namespace UserService.Enums;

public enum RelationStatus
{ 
    [EnumMember(Value = "Not specified")]
    Unknown,
    [EnumMember(Value = "Single")]
    Single,
    [EnumMember(Value = "Committed")]
    Committed,
    [EnumMember(Value = "Open relation")]
    OpenRelation,
    [EnumMember(Value = "Engaged")]
    Engaged,
    [EnumMember(Value = "Married")]
    Married
}