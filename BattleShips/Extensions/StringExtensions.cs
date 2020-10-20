using System;
using System.Linq;

namespace Extensions
{
    public static class StringExtensions
    {
        public static string Repeat(this string text, int count)
        {
            return !string.IsNullOrEmpty(text) ? string.Concat(Enumerable.Repeat(text, count)) : "";
        }
    }
}