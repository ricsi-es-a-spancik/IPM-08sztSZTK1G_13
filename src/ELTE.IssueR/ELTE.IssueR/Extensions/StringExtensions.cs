using System;
using System.Text;

public static class StringExtensions
{
    /// <summary>
    /// Returns part of a string up to the specified number of characters, while maintaining full words
    /// </summary>
    /// <param name="s"></param>
    /// <param name="length">Maximum characters to be returned</param>
    /// <returns>String</returns>
    public static string Chop(this string s, int length)
    {
        if (String.IsNullOrEmpty(s))
            throw new ArgumentNullException(s);
        if (s.Length <= length)
            return s;
        var words = s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (words[0].Length > length)
            return words[0].Substring(0,length) + "...";
        var sb = new StringBuilder();

        foreach (var word in words)
        {
            if ((sb + word).Length > length)
                return string.Format("{0}...", sb.ToString().TrimEnd(' '));
            sb.Append(word + " ");
        }
        return string.Format("{0}...", sb.ToString().TrimEnd(' '));
    }
}