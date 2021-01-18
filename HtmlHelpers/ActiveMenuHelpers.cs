using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BetterWithDona.HtmlHelpers
{
    public static class ActiveMenuHelpers
    {
        public static HtmlString IsActive(this  IHtmlHelper  htmlHelper, string controller)
        {
            var routeData = htmlHelper.ViewContext.RouteData;
            var routeAction = (string)routeData.Values["action"];
            var routeController = (string)routeData.Values["controller"];
            var isActive = string.Equals(controller, routeController, StringComparison.InvariantCultureIgnoreCase)
                           /*&& string.Equals(action, routeAction, StringComparison.InvariantCultureIgnoreCase)*/;
            return  new HtmlString( isActive ? "active" : string.Empty);
        }
        public static HtmlString IsActive(this IHtmlHelper htmlHelper,  string controller, string parametherName,string paramethers)
        {
            var routeData = htmlHelper.ViewContext.RouteData;
            var routeAction = (string)routeData.Values["action"];
            var routeController = (string)routeData.Values["controller"];
            var routeparamether = htmlHelper.ViewContext.HttpContext.Request.Query[parametherName];
            var isActive = string.Equals(controller, routeController, StringComparison.InvariantCultureIgnoreCase)
                           //&& string.Equals(action, routeAction, StringComparison.InvariantCultureIgnoreCase)
                           && string.Equals(paramethers, routeparamether, StringComparison.InvariantCultureIgnoreCase);
            return new HtmlString( isActive ? "active" : string.Empty);
        }


        public static HtmlString IsActive(this IHtmlHelper htmlHelper, string action, string controller)
        {
            var routeData = htmlHelper.ViewContext.RouteData;
            var routeAction = (string)routeData.Values["action"];
            var routeController = (string)routeData.Values["controller"];
            var isActive = string.Equals(controller, routeController, StringComparison.InvariantCultureIgnoreCase)
                           && string.Equals(action, routeAction, StringComparison.InvariantCultureIgnoreCase);
            return  new HtmlString( isActive ? "active" : string.Empty);
        }
        public static HtmlString IsActive(this IHtmlHelper htmlHelper, string action, string controller, string parametherName, string paramethers)
        {
            var routeData = htmlHelper.ViewContext.RouteData;
            var routeAction = (string)routeData.Values["action"];
            var routeController = (string)routeData.Values["controller"];
            var routeparamether = htmlHelper.ViewContext.HttpContext.Request.Query[parametherName];
            var isActive = string.Equals(controller, routeController, StringComparison.InvariantCultureIgnoreCase)
                           && string.Equals(action, routeAction, StringComparison.InvariantCultureIgnoreCase)
                           && string.Equals(paramethers, routeparamether, StringComparison.InvariantCultureIgnoreCase);
            return  new HtmlString( isActive ? "active" : string.Empty);
        }

    }
}
