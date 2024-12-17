using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace UserService.Data;

public class EncryptedValueConverter<T> : ValueConverter<T, string>
{
    public EncryptedValueConverter()
        : base(
            v => !EqualityComparer<T>.Default.Equals(v, default) 
                ? EncryptionHelper.Encrypt(v.ToString()) 
                : null, // Encrypt during save
            v => !string.IsNullOrEmpty(v) 
                ? (T)Convert.ChangeType(EncryptionHelper.Decrypt(v), Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T)) 
                : default // Decrypt during retrieval
        )
    { }
}
