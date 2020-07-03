using System.Text;
using System.Text.RegularExpressions;

namespace GhostNodes.DestinationAssembly
{
    /// <summary>
    /// Represents a wildcard running on the
    /// <see cref="System.Text.RegularExpressions"/> engine.
    /// </summary>
    public class Wildcard : Regex
    {
        /// <summary>
        /// Initializes a wildcard with the given search pattern.
        /// </summary>
        /// <param name="pattern">The wildcard pattern to match.</param>
        public Wildcard(string pattern)
            : base(WildcardToRegex(pattern))
        {
        }

        /// <summary>
        /// Initializes a wildcard with the given search pattern and options.
        /// </summary>
        /// <param name="pattern">The wildcard pattern to match.</param>
        /// <param name="options">A combination of one or more
        /// <see cref="System.Text.RegexOptions"/>.</param>
        public Wildcard(string pattern, RegexOptions options)
            : base(WildcardToRegex(pattern), options)
        {
        }

        /// <summary>
        /// Converts a wildcard to a regex.
        /// </summary>
        /// <param name="pattern">The wildcard pattern to convert.</param>
        /// <returns>A regex equivalent of the given wildcard.</returns>
        public static string WildcardToRegex(string pattern)
        {
            StringBuilder sb = new StringBuilder(pattern.Length + 8);
            sb.Append("^");
            for (int i = 0; i < pattern.Length; i++)
            {
                char c = pattern[i];
                switch (c)
                {
                    case '*':
                        sb.Append(".*"); break;
                    case '?':
                        sb.Append("."); break;
                    case '\\':
                        if (i < pattern.Length - 1)
                            sb.Append(Regex.Escape(pattern[++i].ToString()));
                        break;
                    default:
                        sb.Append(Regex.Escape(pattern[i].ToString()));
                        break;
                }
            }
            sb.Append("$");
            return sb.ToString();

            //string s = "^" + Regex.Escape(pattern) + "$"; 
            //s = Regex.Replace(s, @"(?<!\\)\\\*", @".*"); // Negative Lookbehind
            //s = Regex.Replace(s, @"\\\\\\\*", @"\*");
            //s = Regex.Replace(s, @"(?<!\\)\\\?", @".");  // Negative Lookbehind
            //s = Regex.Replace(s, @"\\\\\\\?", @"\?"); 
            //return Regex.Replace(s, @"\\\\\\\\", @"\\"); 

            //return "^" + Regex.Escape(pattern).
            // Replace("\\*", ".*").
            // Replace("\\?", ".") + "$";
        }
    }
}
