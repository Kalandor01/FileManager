using System.Text.RegularExpressions;

namespace FileManager
{
    /// <summary>
    /// Contains miscellaneous functions.
    /// </summary>
    public static partial class Utils
    {
        #region Constants
        internal static readonly string FILE_NAME_SEED_REPLACE_STRING = "*";

        /// <summary>
        /// Regex for <c>NaturalSort()</c>.
        /// </summary>
        [GeneratedRegex("(\\d+)|(\\D+)")]
        private static partial Regex NaturalSortRegex();
        #endregion

        #region Public functions
        /// <summary>
        /// ReadLine, but only accepts whole numbers.
        /// </summary>
        /// <param name="text">Text to write out when requesting the number.</param>
        /// <param name="errorText">Text to write out when the user inputs a wrong value.</param>
        /// <returns></returns>
        public static int ReadInt(string text, string errorText="Not a number!")
        {
            while (true)
            {
                Console.Write(text);
                if (int.TryParse(Console.ReadLine(), out int res))
                {
                    return res;
                }
                else
                {
                    Console.WriteLine(errorText);
                }
            }
        }

        /// <summary>
        /// Writes out text, and then waits for a key press.
        /// </summary>
        /// <param name="text">The text to write out.</param>
        public static void PressKey(string text)
        {
            Console.Write(text);
            Console.ReadKey(true);
            Console.WriteLine();
        }

        /// <summary>
        /// Function to sort a list of strings, with numbers correctly.<br/>
        /// by L.B <see href="https://stackoverflow.com/a/10000192">SOURCE</see>
        /// </summary>
        /// <param name="list">The list to sort</param>
        public static IEnumerable<string> NaturalSort(IEnumerable<string> list)
        {
            int maxLen = list.Select(s => s.Length).Max();

            static char PaddingChar(string s) => char.IsDigit(s[0]) ? ' ' : char.MaxValue;

            return list
                .Select(s =>
                    new
                    {
                        OrgStr = s,
                        SortStr = NaturalSortRegex().Replace(s, m => m.Value.PadLeft(maxLen, PaddingChar(m.Value)))
                    })
                .OrderBy(x => x.SortStr)
                .Select(x => x.OrgStr);
        }
        #endregion
    }
}
