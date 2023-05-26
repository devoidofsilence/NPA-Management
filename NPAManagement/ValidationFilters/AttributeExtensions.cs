using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace NPAManagement.ValidationFilters
{
    public static class AttributeExtensions
    {
        public static RouteValueDictionary DisabledIf(
            this object htmlAttributes,
            bool disabled
        )
        {
            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            if (disabled)
            {
                attributes["disabled"] = "disabled";
            }
            return attributes;
        }

        public static RouteValueDictionary ReadonlyIf(
            this object htmlAttributes,
            bool readOnly
        )
        {
            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            if (readOnly)
            {
                attributes["readonly"] = "readonly";
            }
            return attributes;
        }
    }
}