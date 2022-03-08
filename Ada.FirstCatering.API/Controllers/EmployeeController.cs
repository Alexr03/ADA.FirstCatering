using System.Net;
using Ada.FirstCatering.API.Filters;
using Ada.FirstCatering.API.Models;
using Ada.FirstCatering.API.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ada.FirstCatering.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly ILogger<EmployeeController> _logger;
    private readonly FirstCateringContext _firstCateringContext;

    public EmployeeController(ILogger<EmployeeController> logger, FirstCateringContext firstCateringContext)
    {
        _logger = logger;
        _firstCateringContext = firstCateringContext;
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

    [HttpGet("FromCard/{id}")]
    [EnsureCardExists]
    public BaseResponse<Employee?> GetEmployeeFromCard(string id)
    {
        var employee = _firstCateringContext.Cards.Include(x => x.Employee)
            .SingleOrDefault(x => x.Id == id)?.Employee;
        return new BaseResponse<Employee?>(employee);
    }

    [HttpGet("{id:int}/Card")]
    public BaseResponse<Card?> GetEmployeeCard(int id)
    {
        var employeeCardLink = _firstCateringContext.Employees.Include(x => x.Card)
            .SingleOrDefault(x => x.Id == id);
        return new BaseResponse<Card?>(employeeCardLink?.Card);
    }

    [HttpPost("Create")]
    public BaseResponse<Employee?> CreateEmployee([FromBody] Employee employee)
    {
        if (ModelState.IsValid)
        {
            _firstCateringContext.Employees.Add(employee);
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