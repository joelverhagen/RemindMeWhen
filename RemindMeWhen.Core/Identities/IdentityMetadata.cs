using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Knapcode.RemindMeWhen.Core.Support;

namespace Knapcode.RemindMeWhen.Core.Identities
{
    public class IdentityMetadata
    {
        private static readonly IEnumerable<KeyValuePair<Func<Type, bool>, TryParseDelegate>> TryParseDelegates = new Dictionary<Func<Type, bool>, TryParseDelegate>
        {
            {type => type.IsEnum, TryParseEnum},
            {type => type == typeof (string), TryParseString},
        };

        private readonly IList<TryParseDelegate> _delegates;

        public IdentityMetadata(IEnumerable<string> fieldNames, IEnumerable<Type> fieldTypes, string fieldSeperator)
        {
            // validate the field names argument
            Guard.ArgumentNotNull(fieldNames, "fieldNames");
            FieldNames = fieldNames.ToList().AsReadOnly();
            if (!FieldNames.Any())
            {
                throw new ArgumentException("There must be at least 1 field name provided.", "fieldNames");
            }

            // validate the field types argument
            Guard.ArgumentNotNull(fieldTypes, "fieldTypes");
            FieldTypes = fieldTypes.ToList().AsReadOnly();

            if (FieldNames.Count != FieldTypes.Count)
            {
                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "The number of field types ({0}) must be equal to the number of field names ({1}).",
                    FieldTypes.Count,
                    FieldNames.Count);
                throw new ArgumentException(message, "fieldTypes");
            }

            if (FieldTypes.Any(t => t == null))
            {
                throw new ArgumentException("Null field types are allowed.", "fieldTypes");
            }

            // validate the field seperator argument
            Guard.ArgumentNotNull(fieldSeperator, "fieldSeperator");
            FieldSeperator = fieldSeperator;
            if (fieldSeperator == string.Empty)
            {
                throw new ArgumentException("The field seperator cannot be an empty string", "fieldSeperator");
            }

            // get the field parser delegates
            var delegates = new List<TryParseDelegate>();
            foreach (Type fieldType in FieldTypes)
            {
                bool foundDelegate = false;
                foreach (var pair in TryParseDelegates)
                {
                    if (pair.Key(fieldType))
                    {
                        delegates.Add(pair.Value);
                        foundDelegate = true;
                        break;
                    }
                }

                if (!foundDelegate)
                {
                    string message = string.Format(
                        CultureInfo.InvariantCulture,
                        "The field type '{0}' is not supported.",
                        fieldType.FullName);
                    throw new ArgumentException(message, "fieldTypes");
                }
            }
            _delegates = delegates;
        }

        public IList<string> FieldNames { get; private set; }
        public IList<Type> FieldTypes { get; private set; }
        public string FieldSeperator { get; private set; }

        public int FieldCount
        {
            get { return FieldTypes.Count; }
        }

        private static bool TryParseString(Type type, string input, out object result)
        {
            result = input;
            return true;
        }

        private static bool TryParseEnum(Type type, string input, out object result)
        {
            try
            {
                result = Enum.Parse(type, input, false);
                return true;
            }
            catch (ArgumentException)
            {
                result = null;
                return false;
            }
        }

        public void ValidateFieldsAreParsable(object[] fields)
        {
            Guard.ArgumentNotNull(fields, "fields");

            string identity = ToString(fields);
            object[] parsedFields;

            // make sure the fields can be parsed
            if (!TryParse(identity, out parsedFields))
            {
                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "The provided fields resulted in the string identity '{0}', which is not parsable.",
                    identity);
                throw new ArgumentException(message, "fields");
            }

            // make sure the parsed fields are equal
            for (int i = 0; i < FieldCount; i++)
            {
                if (!fields[i].Equals(parsedFields[i]))
                {
                    string message = string.Format(
                        CultureInfo.InvariantCulture,
                        "The provided fields could be parsed, but the '{0}' field was parsed to '{1}' instead of '{2}'.",
                        FieldNames[i],
                        fields[i],
                        parsedFields[i]);
                    throw new ArgumentException(message, "fields");
                }
            }
        }

        public string ToString(object[] fields)
        {
            Guard.ArgumentNotNull(fields, "fields");
            if (fields.Length != FieldCount)
            {
                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "The provided number of fields should be {0}, not {1}.",
                    FieldCount,
                    fields.Length);
                throw new ArgumentException(message, "fields");
            }

            if (fields.Any(f => f == null))
            {
                throw new ArgumentException("Null fields fields are not permitted.", "fields");
            }

            for (int i = 0; i < FieldCount; i++)
            {
                if (FieldTypes[i] != fields[i].GetType())
                {
                    string message = string.Format(
                        CultureInfo.InvariantCulture,
                        "The field at index {0} (named '{1}') was type '{2}', instead of type '{3}'.",
                        i,
                        FieldNames[i],
                        fields[i].GetType().FullName,
                        FieldTypes[i].FullName);
                    throw new ArgumentException(message, "fields");
                }
            }

            return string.Join(
                FieldSeperator,
                fields.Select(f => f.ToString()));
        }

        public bool TryParse(string identity, out object[] fields)
        {
            fields = null;

            if (identity == null)
            {
                return false;
            }

            // get the pieces
            string[] pieces = identity.Split(new[] {FieldSeperator}, FieldCount, StringSplitOptions.None);
            if (pieces.Length != FieldCount)
            {
                return false;
            }

            // parse the pieces
            var fieldList = new List<object>();
            for (int i = 0; i < FieldCount; i++)
            {
                Type fieldType = FieldTypes[i];
                TryParseDelegate tryParseDelegate = _delegates[i];
                object field;
                if (!tryParseDelegate(fieldType, pieces[i], out field))
                {
                    return false;
                }
                fieldList.Add(field);
            }

            fields = fieldList.ToArray();
            return true;
        }

        public int CompareTo(object[] fieldsA, object[] fieldsB)
        {
            Guard.ArgumentNotNull(fieldsA, "fieldsA");
            Guard.ArgumentNotNull(fieldsB, "fieldsB");
            return String.Compare(ToString(fieldsA), ToString(fieldsB), StringComparison.Ordinal);
        }

        public bool Equals(object[] fieldsA, object[] fieldsB)
        {
            Guard.ArgumentNotNull(fieldsA, "fieldsA");
            Guard.ArgumentNotNull(fieldsB, "fieldsB");
            return CompareTo(fieldsA, fieldsB) == 0;
        }

        public int GetHashCode(object[] fields)
        {
            Guard.ArgumentNotNull(fields, "fields");
            return ToString(fields).GetHashCode();
        }

        private delegate bool TryParseDelegate(Type type, string input, out object result);
    }
}