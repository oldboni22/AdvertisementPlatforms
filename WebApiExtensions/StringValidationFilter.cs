using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApiExtensions;

public class StringValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var passed = context.ActionArguments.
            FirstOrDefault(arg => arg.Value is string).
            Value as string;

        if (passed == null)
            context.Result = new BadRequestObjectResult("Provided object is null.");

        if (string.IsNullOrWhiteSpace(passed))
            context.Result = new BadRequestObjectResult("Provided string is empty");
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}