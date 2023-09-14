namespace Blog.Client.Extensions
{
    public static class StringExt
    {
        public static string Truncate(this string value, int maxLength)
        {
            return string.IsNullOrEmpty(value) ? value : value.Length <= maxLength ? value : value[..maxLength];
        }
    }
}
