using System.Runtime.Serialization;

namespace UserService.Enums;

public enum Sexuality
{
    [EnumMember(Value = "Not specified")]
    Unknown,
    [EnumMember(Value = "Gay")]
    Gay,
    [EnumMember(Value = "Lesbian")]
    Lesbian,
    [EnumMember(Value = "Bisexual")]
    Bisexual,
    [EnumMember(Value = "Trans")]
    Trans
}