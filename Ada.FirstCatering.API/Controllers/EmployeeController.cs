using System.Net;
using Ada.FirstCatering.API.Filters;
using Ada.FirstCatering.API.Models;
using Ada.FirstCatering.API.Models.Entities;
using Ada.FirstCatering.API.Models.Requests;
using Ada.FirstCatering.API.Models.Responses;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ada.FirstCatering.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly ILogger<EmployeeController> _logger;
    private readonly FirstCateringContext _firstCateringContext;
    private readonly IMapper _mapper;

    public EmployeeController(ILogger<EmployeeController> logger, FirstCateringContext firstCateringContext, IMapper mapper)
    {
        _logger = logger;
        _firstCateringContext = firstCateringContext;
        _mapper = mapper;
    }

    [HttpGet("List")]
    public BaseResponse<IEnumerable<Employee>> GetEmployees()
    {
        return new BaseResponse<IEnumerable<Employee>>(_firstCateringContext.Employees
            .Include(x => x.Card).OrderBy(x => x.Id));
    }

    [HttpGet("{id:int}")]
    public BaseResponse<Employee?> GetEmployee(int id)
    {
        return new BaseResponse<Employee?>(_firstCateringContext.Employees
            .Include(x => x.Card)
            .SingleOrDefault(x => x.Id == id));
    }

    [HttpGet("FromCard/{cardId}")]
    [EnforceCardInfo(EnforceCardInfoAttribute.EEnforceCardInfoFlags.CardExist | EnforceCardInfoAttribute.EEnforceCardInfoFlags.CardHasOwner)]
    public BaseResponse<Employee?> GetEmployeeFromCard(string cardId)
    {
        var employee = _firstCateringContext.Cards.Include(x => x.Employee)
            .SingleOrDefault(x => x.Id == cardId)?.Employee;
        return new BaseResponse<Employee?>(employee);
    }

    [HttpPost("Create/{cardId}")]
    [EnforceCardInfo(EnforceCardInfoAttribute.EEnforceCardInfoFlags.CardExist | EnforceCardInfoAttribute.EEnforceCardInfoFlags.CardHasNoOwner)]
    public BaseResponse<Employee?> CreateEmployee(string cardId, [FromBody] CreateEmployeeModel employeeModel)
    {
        if (ModelState.IsValid)
        {
            var employee = _mapper.Map<Employee>(employeeModel);
            employee.CardId = cardId;
            _firstCateringContext.Employees.Add(employee);
            _firstCateringContext.SaveChanges();
            
            var card = _firstCateringContext.Cards.Find(cardId)!;
            card.EmployeeId = employee.Id;
            _firstCateringContext.SaveChanges();
            return new BaseResponse<Employee?>(employee)
            {
                StatusCode = HttpStatusCode.Created
            };
        }

        var message = string.Join(" | ", ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage));
        return new BaseResponse<Employee?>(ResponseStatus.Error, null)
        {
            Message =
                "Invalid request - " + message,
            StatusCode = HttpStatusCode.BadRequest
        };
    }
}