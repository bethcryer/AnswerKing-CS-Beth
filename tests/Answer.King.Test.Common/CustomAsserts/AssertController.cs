using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Xunit;

namespace Answer.King.Test.Common.CustomAsserts;

public static class AssertController
{
    public static void HasRouteAttribute<TController>(string route)
        where TController : ControllerBase
    {
        var attr = typeof(TController).GetCustomAttributes(typeof(RouteAttribute), false).ToList();

        attr.AssertAttributeCount<RouteAttribute>();

        Assert.Equal(route, ((RouteAttribute)attr[0]).Template, true);
    }

    public static void MethodHasVerb<TController, TVerbAttribute>(string methodName)
        where TController : ControllerBase
        where TVerbAttribute : HttpMethodAttribute
    {
        var method = typeof(TController).GetMethod(methodName);

        var attr = method?.GetCustomAttributes(typeof(TVerbAttribute), false).ToList();

        attr?.AssertAttributeCount<TVerbAttribute>();
    }

    public static void MethodHasVerb<TController, TVerbAttribute>(string methodName, string template)
        where TController : ControllerBase
        where TVerbAttribute : HttpMethodAttribute
    {
        var method = typeof(TController).GetMethod(methodName);

        var attr = method?.GetCustomAttributes(typeof(TVerbAttribute), false).ToList();

        if (attr == null)
        {
            throw new Exception("No custom attributes found.");
        }

        attr.AssertAttributeCount<TVerbAttribute>();

        var verb = (HttpMethodAttribute)attr[0];

        Assert.Equal(template, verb.Template);
    }

    public static void MethodHasRoute<TController>(string methodName, string template)
        where TController : ControllerBase
    {
        var method = typeof(TController).GetMethod(methodName);

        var attr = method?.GetCustomAttributes(typeof(RouteAttribute), false).ToList();

        if (attr == null)
        {
            throw new Exception("No custom attributes found.");
        }

        attr.AssertAttributeCount<RouteAttribute>();

        Assert.Equal(template, ((RouteAttribute)attr[0]).Template, true);
    }
}
