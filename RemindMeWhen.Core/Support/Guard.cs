using System;

namespace Knapcode.RemindMeWhen.Core.Support
{
    public static class Guard
    {
        public static void ArgumentNotNull(object argument, string paramName)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }
    }
}