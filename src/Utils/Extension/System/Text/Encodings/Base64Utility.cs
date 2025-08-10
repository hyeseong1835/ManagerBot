
namespace System.Text.Encodings;
public static class Base64Utility
{
    public static string UlongToBase64(ulong id) => Convert.ToBase64String(BitConverter.GetBytes(id));
    public static ulong Base64ToUlong(string base64) => BitConverter.ToUInt64(Convert.FromBase64String(base64));
}