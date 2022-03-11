using Ada.FirstCatering.API.Filters;
using Ada.FirstCatering.API.Models.Responses;
using Ada.FirstCatering.API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ada.FirstCatering.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FirstCateringController : ControllerBase
{
    private readonly ILogger<FirstCateringController> _logger;
    private readonly FirstCateringContext _firstCateringContext;
    private readonly CardSessionService _cardSessionService;
    private readonly IMapper _mapper;

    public FirstCateringController(ILogger<FirstCateringController> logger, FirstCateringContext firstCateringContext,
        CardSessionService cardSessionService, IMapper mapper)
    {
        _logger = logger;
        _firstCateringContext = firstCateringContext;
        _cardSessionService = cardSessionService;
        _mapper = mapper;
    }

    [HttpPost("StartSession")]
    [EnforceCardInfo(EnforceCardInfoAttribute.EEnforceCardInfoFlags.CardExist |
                     EnforceCardInfoAttribute.EEnforceCardInfoFlags.CardHasOwner |
                     EnforceCardInfoAttribute.EEnforceCardInfoFlags.PinCorrect |
                     EnforceCardInfoAttribute.EEnforceCardInfoFlags.HasNoActiveSession)]
    public BaseResponse<CardSession> StartSession(string cardId, int pin)
    {
        var card = _firstCateringContext.Cards.Include(x => x.Employee).FirstOrDefault(x => x.Id == cardId)!;
        var cardSession = _cardSessionService.CreateCardSession(cardId);
        var response = new BaseResponse<CardSession>(ResponseStatus.Success, cardSession)
        {
            Message = $"Welcome {card.Employee!.FullName}!"
        };
        return response;
    }

    [HttpPost("EndSession")]
    [EnforceCardInfo(EnforceCardInfoAttribute.EEnforceCardInfoFlags.CardExist |
                     EnforceCardInfoAttribute.EEnforceCardInfoFlags.CardHasOwner |
                     EnforceCardInfoAttribute.EEnforceCardInfoFlags.PinCorrect |
                     EnforceCardInfoAttribute.EEnforceCardInfoFlags.HasActiveSession)]
    public BaseResponse EndSession(string cardId, int pin)
    {
        _cardSessionService.DeleteCardSession(cardId);
        var response = new BaseResponse(ResponseStatus.Success, "Goodbye");
        return response;
    }

    [HttpPost("Tap")]
    [EnforceCardInfo(EnforceCardInfoAttribute.EEnforceCardInfoFlags.CardExist |
                     EnforceCardInfoAttribute.EEnforceCardInfoFlags.CardHasOwner)]
    public object TapCard(string cardId, int pin)
    {
        var cardSession = _cardSessionService.GetCardSession(cardId);
        if (cardSession == null)
        {
            return StartSession(cardId, pin);
        }

        return EndSession(cardId, pin);
    }

    [HttpPost("TopUp")]
    [EnforceCardInfo(EnforceCardInfoAttribute.EEnforceCardInfoFlags.CardExist |
                     EnforceCardInfoAttribute.EEnforceCardInfoFlags.CardHasOwner |
                     EnforceCardInfoAttribute.EEnforceCardInfoFlags.HasActiveSession |
                     EnforceCardInfoAttribute.EEnforceCardInfoFlags.PinCorrect)]
    public BaseResponse<TopUpResponse> TopUpCard(string cardId, int pin, decimal amount)
    {
        var card = _firstCateringContext.Cards.Find(cardId)!;
        var oldBalance = card.Balance;
        var newBalance = oldBalance + amount;
        card.Balance = newBalance;
        _firstCateringContext.SaveChanges();
        _cardSessionService.UpdateCardSessionExpiration(cardId);
        
        return new BaseResponse<TopUpResponse>(ResponseStatus.Success, new TopUpResponse()
        {
            OldBalance = oldBalance,
            NewBalance = newBalance
        })
        {
            Message = "Top up successful"
        };
    }
}