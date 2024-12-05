using System.Runtime.Serialization;

namespace UserService.Enums;

public enum LookingFor
{
    [EnumMember(Value = "Friendship")]
    Friendship,
    [EnumMember(Value = "Relation")]
    Relation,
    [EnumMember(Value = "Fun")]
    Fun
}