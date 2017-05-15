namespace Upgrader
{
    internal static class StringHelper
    {
        internal static string Truncate(string @string, int maxLength)
        {
            return @string.Length >= maxLength ? @string.Substring(0, maxLength) : @string;
        }
    }
}
