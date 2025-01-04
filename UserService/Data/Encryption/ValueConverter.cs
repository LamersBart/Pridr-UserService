using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace UserService.Data.Encryption;

public class EncryptedValueConverter<T> : ValueConverter<T?, string?> where T : struct
{
    public EncryptedValueConverter()
        : base(
            v => v.HasValue ? EncryptionHelper.Encrypt(v.Value.ToString()!) : null, // Encrypt if not null
            v => !string.IsNullOrEmpty(v) 
                ? (T?)Convert.ChangeType(EncryptionHelper.Decrypt(v), Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T))
                : default(T?) // Explicitly handle nullable default
        )
    { }
}

public class EncryptedReferenceConverter<T> : ValueConverter<T?, string?> where T : class
{
    public EncryptedReferenceConverter()
        : base(
            v => v != null ? EncryptionHelper.Encrypt(v.ToString()!) : null, // Encrypt if not null
            v => !string.IsNullOrEmpty(v)
                ? (T?)Convert.ChangeType(EncryptionHelper.Decrypt(v), typeof(T)) 
                : null // Decrypt
        )
    { }
}
