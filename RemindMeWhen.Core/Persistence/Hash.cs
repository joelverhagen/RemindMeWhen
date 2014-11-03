using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Knapcode.RemindMeWhen.Core.Persistence
{
    public struct Hash
    {
        private readonly string _hashName;
        private readonly byte[] _value;

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
    }
}