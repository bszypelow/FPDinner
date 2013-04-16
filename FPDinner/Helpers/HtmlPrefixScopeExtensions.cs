using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Web.Mvc.Html
{
    /// <summary>
    /// Provides support for nested dynamic collections.
    /// Taken from blog post:
    /// http://blog.gutek.pl/post/2011/05/12/Edycja-zagniezdzonych-list-w-MVC-3-(Czesc-1).aspx
    /// and slightly modified.
    /// </summary>
    public static class HtmlPrefixScopeExtensions
    {
        private const string IdsToReuseKey = "__htmlPrefixScopeExtensions_IdsToReuse_";
        private const string ViewDataPrefixKey = "__prefix";
        private const string IndexNameWithDot = ".index";

        public static MvcHtmlString AddDynamicRowLink(this HtmlHelper html, string linktTitle, string actionName, string selector, object htmlAttributes = null)
        {
            var attributes = new Dictionary<string, object>
                               {
                                   { "data-prefix", html.ViewData.TemplateInfo.HtmlFieldPrefix },
                                   { "data-placeholder", selector },
                                   { "title", linktTitle },
                                   { "class", "addRow" }
                               };

            foreach (var attribute in HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes))
            {
                attributes[attribute.Key] = attribute.Value;
            }

            return html.ActionLink(linktTitle, actionName, null, attributes);
        }

        public static IDisposable BeginCollectionItemFor<TModel>(this HtmlHelper html, Expression<Func<TModel, IEnumerable>> expression)
        {
            var member = expression.Body as MemberExpression;
            if (member == null)
            {
                throw new ArgumentException(string.Format(
                    "Expression '{0}' must refer to a property.",
                    expression));
            }

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
            {
                throw new ArgumentException(string.Format(
                    "Expression '{0}' must refer to a property.",
                    expression));
            }

            return html.BeginCollectionItem(member.Member.Name);
        }

        public static IDisposable BeginCollectionItem(this HtmlHelper html, string collectionName)
        {
            var idsToReuse = GetIdsToReuse(html.ViewContext.HttpContext, collectionName);
            string itemIndex = idsToReuse.Count > 0 ? idsToReuse.Dequeue() : Guid.NewGuid().GetHashCode().ToString("x");

            // Section for nested lists, checking if we have prefix from current context
            // and if we don't maybe we have passed prefix using ViewData - its useful when
            // generating Row values by Ajax Request to controller
            var templatePrefix = html.ViewData.TemplateInfo.HtmlFieldPrefix;
            string viewDataPrefix = html.ViewData.ContainsKey(ViewDataPrefixKey)
                                        ? html.ViewData[ViewDataPrefixKey] as string
                                        : string.Empty;

            var currentPrefix = !string.IsNullOrEmpty(templatePrefix)
                                    ? templatePrefix
                                    : viewDataPrefix;

            string dot = string.IsNullOrEmpty(currentPrefix)
                             ? string.Empty
                             : ".";

            // myListProp[0].myCollection
            string fullPrefix = string.Format("{0}{1}{2}", currentPrefix, dot, collectionName);

            // autocomplete="off" is needed to work around a very annoying Chrome behaviour whereby it reuses old values 
            // after the user clicks "Back", which causes the xyz.index and xyz[...] values to get out of sync.   // .
            html.ViewContext.Writer.WriteLine(string.Format(
                CultureInfo.InvariantCulture,
                "<input type=\"hidden\" name=\"{0}{1}\" autocomplete=\"off\" value=\"{2}\" />",
                fullPrefix,
                IndexNameWithDot,
                html.Encode(itemIndex)));

            var fieldPrefix = string.Format(CultureInfo.InvariantCulture, "{0}[{1}]", fullPrefix, itemIndex);

            return html.BeginHtmlFieldPrefixScope(fieldPrefix);
        }

        public static IDisposable BeginHtmlFieldPrefixScope(this HtmlHelper html, string htmlFieldPrefix)
        {
            return new HtmlFieldPrefixScope(html.ViewData.TemplateInfo, htmlFieldPrefix);
        }

        private static Queue<string> GetIdsToReuse(HttpContextBase httpContext, string collectionName)
        {
            // We need to use the same sequence of IDs following a server-side validation failure,  
            // otherwise the framework won't render the validation error messages next to each item.
            string key = IdsToReuseKey + collectionName;
            var queue = (Queue<string>)httpContext.Items[key];

            if (queue != null)
            {
                return queue;
            }

            httpContext.Items[key] = queue = new Queue<string>();
            var previouslyUsedIds = httpContext.Request[collectionName + IndexNameWithDot];
            if (!string.IsNullOrEmpty(previouslyUsedIds))
            {
                foreach (var previouslyUsedId in previouslyUsedIds.Split(','))
                {
                    queue.Enqueue(previouslyUsedId);
                }
            }

            return queue;
        }

        /// <summary>
        /// Represents a scope in the view where a specified HTML field prefix is added.
        /// </summary>
        private class HtmlFieldPrefixScope : IDisposable
        {
            private readonly TemplateInfo templateInfo;
            private readonly string previousHtmlFieldPrefix;

            public HtmlFieldPrefixScope(TemplateInfo templateInfo, string htmlFieldPrefix)
            {
                this.templateInfo = templateInfo;

                this.previousHtmlFieldPrefix = templateInfo.HtmlFieldPrefix;
                this.templateInfo.HtmlFieldPrefix = htmlFieldPrefix;
            }

            public void Dispose()
            {
                this.templateInfo.HtmlFieldPrefix = this.previousHtmlFieldPrefix;
            }
        }
    }
}