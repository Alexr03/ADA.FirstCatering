using Microsoft.AspNetCore.Mvc;

namespace Ada.FirstCatering.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FirstCateringController : ControllerBase
{
    private readonly ILogger<FirstCateringController> _logger;
    private readonly FirstCateringContext _firstCateringContext;

    public FirstCateringController(ILogger<FirstCateringController> logger, FirstCateringContext firstCateringContext)
    {
        _logger = logger;
        _firstCateringContext = firstCateringContext;
        firstCateringContext.Database.EnsureCreated();
    }
}