using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FPDinner.Helpers
{
    public static class RavenExtensions
    {
        public static T FirstOrEmpty<T>(this IQueryable<T> @this) where T : class
        {
            return @this.FirstOrDefault() ?? default(T);
        }
    }
}