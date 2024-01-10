using System.Numerics;

namespace FileManager
{
    /// <summary>
    /// Extensions for <c>BigInteger</c>.
    /// </summary>
    public static class BigIntegerExtensions
    {
        /// <summary>
        /// Helper number for <c>Sqrt</c>.
        /// </summary>
        private static readonly BigInteger FastSqrtSmallNumber = 4503599761588223UL;

        /// <summary>
        /// Square root calculator for <c>BigInteger</c>s.<br/>
        /// By MaxKlaxx <see href="https://stackoverflow.com/a/63909229">LINK</see>
        /// </summary>
        /// <param name="value">The <c>BigInteger</c>.</param>
        /// <returns>Square root of the value.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static BigInteger Sqrt(this BigInteger value)
        {
            if (value <= FastSqrtSmallNumber) // small enough for Math.Sqrt() or negative?
            {
                if (value.Sign < 0) throw new ArgumentException("Negative argument.");
                return (ulong)Math.Sqrt((ulong)value);
            }

            BigInteger root; // now filled with an approximate value
            int byteLen = value.ToByteArray().Length;
            if (byteLen < 128) // small enough for direct double conversion?
            {
                root = (BigInteger)Math.Sqrt((double)value);
            }
            else // large: reduce with bitshifting, then convert to double (and back)
            {
                root = (BigInteger)Math.Sqrt((double)(value >> (byteLen - 127) * 8)) << (byteLen - 127) * 4;
            }

            for (; ; )
            {
                var root2 = value / root + root >> 1;
                if ((root2 == root || root2 == root + 1) && IsSqrt(value, root)) return root;
                root = value / root2 + root2 >> 1;
                if ((root == root2 || root == root2 + 1) && IsSqrt(value, root2)) return root2;
            }
        }

        /// <summary>
        /// Returns if the <c>BigInteger</c>'s square root is equal to the calculated value.<br/>
        /// By MaxKlaxx <see href="https://stackoverflow.com/a/63909229">LINK</see>
        /// </summary>
        /// <param name="value">The <c>BigInteger</c>.</param>
        /// <param name="root">The calculated square root.</param>
        public static bool IsSqrt(BigInteger value, BigInteger root)
        {
            var lowerBound = root * root;

            return value >= lowerBound && value <= lowerBound + (root << 1);
        }
    }
}
