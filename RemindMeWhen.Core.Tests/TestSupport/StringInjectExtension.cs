using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.UI;
using Knapcode.KitchenSink.Support;

namespace Knapcode.RemindMeWhen.Core.Tests.TestSupport
{
    /// <summary>
    /// Source: http://james.newtonking.com/archive/2008/03/29/formatwith-2-0-string-formatting-with-named-variables
    /// </summary>
    public static class StringExtensions
    {
        public static string FormatWith(this string format, object source)
        {
            return FormatWith(format, null, source);
        }

        public static string FormatWith(this string format, IFormatProvider provider, object source)
        {
            Guard.ArgumentNotNull(format, "format");

            var r = new Regex(
                @"(?<start>\{)+(?<property>[\w\.\[\]]+)(?<format>:[^}]+)?(?<end>\})+",
                RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

            var values = new List<object>();
            string rewrittenFormat = r.Replace(format, m =>
            {
                Group startGroup = m.Groups["start"];
                Group propertyGroup = m.Groups["property"];
                Group formatGroup = m.Groups["format"];
                Group endGroup = m.Groups["end"];

                values.Add((propertyGroup.Value == "0") ? source : DataBinder.Eval(source, propertyGroup.Value));
                return new string('{', startGroup.Captures.Count) + (values.Count - 1) + formatGroup.Value + new string('}', endGroup.Captures.Count);
            });

            return string.Format(provider, rewrittenFormat, values.ToArray());
        }
    }
}