using System;
using Ninject.Activation;
using Ninject.Syntax;

namespace Knapcode.RemindMeWhen.Core.Extensions
{
    public static class NinjectExtensions
    {
        public static IBindingInNamedWithOrOnSyntax<T> WhenInjectedIntoDescendentOf<T>(this IBindingWhenSyntax<T> when, Type ancestor)
        {
            return when.When(r => IsTypeInRequestedByAncestor(r, ancestor));
        }

        private static bool IsTypeInRequestedByAncestor(IRequest r, Type ancestor)
        {
            while (r != null && !ancestor.IsAssignableFrom(r.Target.Member.ReflectedType))
            {
                r = r.ParentRequest;
            }

            return r != null;
        }
    }
}