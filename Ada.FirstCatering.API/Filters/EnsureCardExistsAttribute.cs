using System.Net;
using Ada.FirstCatering.API.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace Ada.FirstCatering.API.Filters;

public class EnsureCardExistsAttribute : ActionFilterAttribute
{
    private readonly EEnsureCardFlags _flags;

    public EnsureCardExistsAttribute(EEnsureCardFlags flags = EEnsureCardFlags.CardExists | EEnsureCardFlags.CardHasOwner)
    {
        _flags = flags;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var firstCateringContext = context.HttpContext.RequestServices.GetRequiredService<FirstCateringContext>();
        var cardId = context.ActionArguments["id"] as string;
        var card = firstCateringContext.Cards.Include(x => x.Employee).SingleOrDefault(x => x.Id == cardId);

        if (_flags.HasFlag(EEnsureCardFlags.CardExists))
        {
            if (card == null)
            {
                context.Result = new ObjectResult(
                    new BaseResponse(ResponseStatus.Error, $"No card with id '{cardId}' found")
                    {
                        StatusCode = HttpStatusCode.NotFound
                    });
            }
        }

        if (_flags.HasFlag(EEnsureCardFlags.CardHasOwner))
        {
            if (card?.Employee == null)
            {
                context.Result = new ObjectResult(
                    new BaseResponse<object>(ResponseStatus.Error,
                        $"Card was found, but it has no employee linked.")
                    {
                        Data = new
                        {
                            Information = "Please create a new employee and linking this card by sending a request to /Employee/Create"
                        },
                        StatusCode = HttpStatusCode.NotFound
                    });
            }
        }
    }

    [Flags]
    public enum EEnsureCardFlags
    {
        CardExists,
        CardHasOwner
    }
}