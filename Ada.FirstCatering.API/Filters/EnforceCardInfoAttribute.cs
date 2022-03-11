using System.Net;
using Ada.FirstCatering.API.Models.Responses;
using Ada.FirstCatering.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace Ada.FirstCatering.API.Filters;

public class EnforceCardInfoAttribute : ActionFilterAttribute
{
    private readonly EEnforceCardInfoFlags _infoFlags;

    public EnforceCardInfoAttribute(EEnforceCardInfoFlags infoFlags)
    {
        _infoFlags = infoFlags;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var firstCateringContext = context.HttpContext.RequestServices.GetRequiredService<FirstCateringContext>();
        var cardId = context.ActionArguments["cardId"] as string;

        if (string.IsNullOrEmpty(cardId))
        {
            context.Result = new ObjectResult(new BaseResponse(ResponseStatus.Error, "Card ID is required")
            {
                StatusCode = HttpStatusCode.BadRequest
            });
            return;
        }

        var card = firstCateringContext.Cards.Include(x => x.Employee).SingleOrDefault(x => x.Id == cardId);

        if (_infoFlags.HasFlag(EEnforceCardInfoFlags.CardDontExist))
        {
            if (card != null)
            {
                context.Result = new ObjectResult(
                    new BaseResponse(ResponseStatus.Error, $"Card was found with id {cardId}")
                    {
                        StatusCode = HttpStatusCode.BadRequest
                    });
                return;
            }
        }

        if (_infoFlags.HasFlag(EEnforceCardInfoFlags.CardExist))
        {
            if (card == null)
            {
                context.Result = new ObjectResult(
                    new BaseResponse(ResponseStatus.Error, $"No card with id '{cardId}' found")
                    {
                        StatusCode = HttpStatusCode.NotFound
                    });
                return;
            }
        }

        if (_infoFlags.HasFlag(EEnforceCardInfoFlags.CardHasOwner))
        {
            if (card?.Employee == null)
            {
                context.Result = new ObjectResult(new BaseResponse<object>(ResponseStatus.Error,
                    $"Card was found, but it has no employee linked.")
                {
                    Data = new
                    {
                        Information =
                            $"Please create a new employee and linking this card by sending a request to /Employee/Create/{cardId}"
                    },
                    StatusCode = HttpStatusCode.NotFound
                });
                return;
            }
        }

        if (_infoFlags.HasFlag(EEnforceCardInfoFlags.CardHasNoOwner))
        {
            if (card?.Employee != null)
            {
                context.Result = new ObjectResult(new BaseResponse(ResponseStatus.Error,
                    $"Card was found, but it has an employee linked.")
                {
                    StatusCode = HttpStatusCode.BadRequest
                });
                return;
            }
        }

        if (_infoFlags.HasFlag(EEnforceCardInfoFlags.PinCorrect))
        {
            if (card?.Pin != (int)context.ActionArguments["pin"]!)
            {
                context.Result = new ObjectResult(new BaseResponse(ResponseStatus.Error, $"Pin is incorrect.")
                {
                    StatusCode = HttpStatusCode.BadRequest
                });
                return;
            }
        }

        if (_infoFlags.HasFlag(EEnforceCardInfoFlags.HasActiveSession))
        {
            var cardSessionService = context.HttpContext.RequestServices.GetRequiredService<CardSessionService>();
            var cardSession = cardSessionService.GetCardSession(cardId);
            if (cardSession == null)
            {
                context.Result = new ObjectResult(
                    new BaseResponse(ResponseStatus.Error,
                        $"No active session found for card with id '{cardId}'. Your session may have expired or you have not created a session yet")
                    {
                        StatusCode = HttpStatusCode.BadRequest
                    });
                return;
            }
        }

        if (_infoFlags.HasFlag(EEnforceCardInfoFlags.HasNoActiveSession))
        {
            var cardSessionService = context.HttpContext.RequestServices.GetRequiredService<CardSessionService>();
            var cardSession = cardSessionService.GetCardSession(cardId);
            if (cardSession != null)
            {
                context.Result = new ObjectResult(
                    new BaseResponse<object>(ResponseStatus.Error,
                        $"Session found for card with id '{cardId}' when no session was supposed to be active")
                    {
                        Data = new
                        {
                            CardSession = cardSession
                        },
                        StatusCode = HttpStatusCode.BadRequest
                    });
                return;
            }
        }
    }

    [Flags]
    public enum EEnforceCardInfoFlags
    {
        CardDontExist = 1,
        CardExist = 2,
        CardHasOwner = 4,
        CardHasNoOwner = 8,
        PinCorrect = 16,
        HasActiveSession = 32,
        HasNoActiveSession = 64
    }
}