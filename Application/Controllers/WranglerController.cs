using Common;
using DomainService;
using Application.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Application.Responses;

namespace Application;

[ApiController]
[Route("[controller]")]
public class WranglerController
{
    private readonly long _offset;
    private readonly long _range;
    private readonly IAccessDomainService _service;
    public WranglerController(
        IAccessDomainService service,
        IOptions<TimingsOptions> timingsOptions
        )
    {
        _service = service;
        _offset = timingsOptions.Value.CompilationOffsetSecs +
            timingsOptions.Value.CompilationDurationPredictionSecs;
        _range = timingsOptions.Value.PlaneDocLifetimesSecs;
    }

    [HttpGet]
    [Route("frame")]
    [ProducesResponseType(typeof(PlaneFrameResponse),200)]
    public async Task<IActionResult> GetFrameAsync([FromQuery] long? time)
    {
        time ??= MaxTime;
        
        //If the time is too high or too low, we won't have the answer
        if(time > MaxTime || time < MaxTime - _range)
        {
            return new BadRequestResult();
        }

        var results = await _service.RetrieveRecentPlaneFrame(time.Value);

        var result = new OkObjectResult(results.ToResponse());
        
        return result;
    }
    
    private long MaxTime =>
        (long) (DateTime.UtcNow
            .Subtract(DateTime.UnixEpoch).TotalSeconds - _offset);

}