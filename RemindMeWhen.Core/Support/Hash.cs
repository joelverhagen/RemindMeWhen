using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Knapcode.RemindMeWhen.Core.Support
{
    public struct Hash
    {
        private readonly string _hashName;
        private readonly byte[] _value;

        public Hash(string hashString)
        {
            if (hashString == null)
            {
                throw new ArgumentNullException("hashString");
            }

            int lastHyphenIndex = hashString.LastIndexOf('-');
            if (lastHyphenIndex < 0)
            {
                throw new ArgumentException("The hash string must contain at least one hyphen.");
            }

            _hashName = hashString.Substring(0, lastHyphenIndex);

            string valueString = hashString.Substring(lastHyphenIndex + 1);
            _value = StringToByteArray(valueString);
        }

        public Hash(string hashName, IEnumerable<byte> value)
        {
            _hashName = hashName;
            _value = value.ToArray();
        }

        public byte[] Value
        {
            get { return _value; }
        }

        public string HashName
        {
            get { return _hashName; }
        }

        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}-{1}",
                HashName,
                Value != null ? BitConverter.ToString(Value).Replace("-", string.Empty) : string.Empty);
        }

        /// <summary>
        /// Convert a hex string to a byte array.
        /// Source: http://stackoverflow.com/a/321404/52749
        /// </summary>
        /// <param name="hex">The hexadecimal string.</param>
        /// <returns>The byte array.</returns>
        private static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x%2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }
    }
}