using System;

namespace task01
{
    public static class StringExtensions
    {
        public static bool IsPalindrome(this string str)
        {
            if (str != "")
            {
                str = str.ToLower();
                string filtered = string.Empty;
                string reversed = string.Empty;
                foreach (char c in str)
                {
                    if (!char.IsPunctuation(c) && !char.IsWhiteSpace(c))
                    {
                        filtered += c;
                    }
                }
                if (filtered != "")
                {
                    for (int i = filtered.Length - 1; i >= 0; i--)
                    {
                        reversed += filtered[i];
                    }
                    return filtered == reversed;
                }
                return false;
            }
            return false;
        }
    }
}
