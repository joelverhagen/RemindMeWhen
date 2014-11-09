using System;
using System.Collections.Generic;
using System.Text;
using Knapcode.RemindMeWhen.Core.Clients;
using Knapcode.RemindMeWhen.Core.Models;

namespace Knapcode.RemindMeWhen.Core.Identities
{
    public static class IdentityTemplate
    {
        public static readonly IdentityMetadata DocumentIdentityMetadata = new IdentityMetadata(
            new[] {"Type", "TypeIdentity"},
            new[] {typeof (DocumentType), typeof (string)},
            "-");

        public static readonly IdentityMetadata EventIdentityMetadata = new IdentityMetadata(
            new[] {"Type", "Source", "SourceIdentity"},
            new[] {typeof (EventType), typeof (string), typeof (string)},
            "-");

        public static readonly IDictionary<string, IdentityMetadata> AllMetadata = new Dictionary<string, IdentityMetadata>
        {
            {"DocumentIdentity", DocumentIdentityMetadata},
            {"EventIdentity", EventIdentityMetadata},
        };

        public static string GetLowercase(string propertyName)
        {
            var builder = new StringBuilder(propertyName);
            builder[0] = char.ToLower(builder[0]);
            return builder.ToString();
        }

        public static string GetTypeName(Type type)
        {
            if (type == typeof (string))
            {
                return "string";
            }

            return type.Name;
        }

        public static IdentityProperty[] GetIdentityProperties(IdentityMetadata metadata)
        {
            var properties = new IdentityProperty[metadata.FieldCount];
            for (int i = 0; i < metadata.FieldCount; i++)
            {
                properties[i] = new IdentityProperty
                {
                    Index = i,
                    IsValueType = metadata.FieldTypes[i].IsValueType,
                    TypeName = GetTypeName(metadata.FieldTypes[i]),
                    FieldName = "_" + GetLowercase(metadata.FieldNames[i]),
                    LocalName = GetLowercase(metadata.FieldNames[i]),
                    PropertyName = metadata.FieldNames[i],
                    Seperator = i == metadata.FieldCount - 1 ? string.Empty : ", "
                };
            }

            return properties;
        }
    }
}