using System;
using System.Security.Cryptography;
using System.Text;

namespace UnityCommon
{
    public static class CryptoUtils
    {
        /// <summary>
        /// Returns the SHA256 hash of the input string.
        /// </summary>
        public static byte[] Sha256 (string inputString)
        {
            var bytes = Encoding.UTF8.GetBytes(inputString);
            var sha256 = new SHA256Managed();
            return sha256.ComputeHash(bytes);
        }

        /// <summary>
        /// Returns URI-safe data with a given input length.
        /// </summary>
        /// <param name="length">Input length (nb. output will be longer).</param>
        public static string RandomDataBase64Uri (uint length)
        {
            var cryptoProvider = new RNGCryptoServiceProvider();
            var bytes = new byte[length];
            cryptoProvider.GetBytes(bytes);
            return Base64UriEncodeNoPadding(bytes);
        }

        /// <summary>
        /// Base64Uri no-padding encodes the given input buffer.
        /// </summary>
        public static string Base64UriEncodeNoPadding (byte[] buffer)
        {
            var base64 = Convert.ToBase64String(buffer);

            // Converts base64 to Base64Uri.
            base64 = base64.Replace("+", "-");
            base64 = base64.Replace("/", "_");
            // Strips padding.
            base64 = base64.Replace("=", "");

            return base64;
        }

        /// <summary>
        /// Generates a hash code for the specified <see cref="string"/>; this hash code is guaranteed not to change in the future.
        /// </summary>
        /// <param name="value">The <see cref="string"/> to hash.</param>
        /// <returns>A hash code for the specified <see cref="string"/>.</returns>
        /// <remarks>Based on <a href="http://www.azillionmonkeys.com/qed/hash.html">SuperFastHash</a>.</remarks>
        public static int PersistentHashCode (string value)
        {
            unchecked
            {
                // Check for degenerate input.
                if (string.IsNullOrEmpty(value))
                    return 0;

                int length = value.Length;
                uint hash = (uint)length;

                int remainder = length & 1;
                length >>= 1;

                // Main loop.
                int index = 0;
                for (; length > 0; length--)
                {
                    hash += value[index];
                    uint temp = (uint)(value[index + 1] << 11) ^ hash;
                    hash = (hash << 16) ^ temp;
                    index += 2;
                    hash += hash >> 11;
                }

                // Handle odd string length.
                if (remainder == 1)
                {
                    hash += value[index];
                    hash ^= hash << 11;
                    hash += hash >> 17;
                }

                // Force "avalanching" of final 127 bits.
                hash ^= hash << 3;
                hash += hash >> 5;
                hash ^= hash << 4;
                hash += hash >> 17;
                hash ^= hash << 25;
                hash += hash >> 6;

                return (int)hash;
            }
        }

        /// <summary>
        /// Generates a hash code in hex format for the specified <see cref="string"/>; the hash code is guaranteed not to change in the future.
        /// </summary>
        public static string PersistentHexCode (string value)
        {
            return string.Format("{0:x}", PersistentHashCode(value));
        }

    }
}
