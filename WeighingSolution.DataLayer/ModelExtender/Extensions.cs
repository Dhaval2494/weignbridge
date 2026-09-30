namespace WeighingSolution.DataLayer
{
    public static class Extensions
    {
        public static string ReplaceAt(this string value, int index, char newchar)
        {
            if (value == null || value.Length <= index || index < 0)
                return value;
            else
            {
                char[] chars = value.ToCharArray();
                chars[index] = newchar;
                return new string(chars);
            }
        }
    }
}
