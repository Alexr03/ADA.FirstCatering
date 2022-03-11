using Ada.FirstCatering.API.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Ada.FirstCatering.API.Filters;

public class BaseResponseFilter : ResultFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Result is ObjectResult e)
        {
            var memberInfo = GetReturnType(e.Value!.GetType());
            if ((memberInfo == typeof(BaseResponse) || memberInfo.IsGenericType && memberInfo.GetGenericTypeDefinition() == typeof(BaseResponse<>)))
            {
                var response = (BaseResponse)e.Value!;
                if (response.StatusCode.HasValue)
                {
                    context.HttpContext.Response.StatusCode = (int)response.StatusCode;
                }
            }
        }
        base.OnResultExecuting(context);
    }
    
    private static Type GetReturnType(Type returnType)
    {
        if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
        {
            var args = returnType.GenericTypeArguments;
            return !args.Any() ? returnType : args[0];
        }
        
        return returnType;
    }
}