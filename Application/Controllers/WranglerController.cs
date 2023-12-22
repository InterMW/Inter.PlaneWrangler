using Common;
using Domain;
using DomainService;
using Microsoft.AspNetCore.Mvc;

namespace Application;

[ApiController]
[Route("[controller]")]
public class WranglerController
{
    private readonly long _offset;
    private readonly IAccessDomainService _service;
    public WranglerController(
        IAccessDomainService service,
        TimingsOptions timingsOptions)
    {
        _service = service;
        _offset = timingsOptions.CompilationOffsetSecs +
            timingsOptions.CompilationDurationPredictionSecs;
    }

    [HttpGet]
    [Route("frame")]
    [ProducesResponseType(typeof(PlaneFrame),200)]
    public async Task<IActionResult> GetFrameAsync([FromQuery] long? time)
    {
        time = time.HasValue ? time.Value : MaxTime;
        
        if(time > MaxTime)
        {
            return new BadRequestResult();
        }

        var results = await _service.RetrieveRecentPlaneFrame(time.Value);

        return new OkObjectResult(results);
    }
    
    private long MaxTime =>
        (long) (DateTime.UtcNow
            .Subtract(DateTime.UnixEpoch).TotalSeconds - _offset);

//425 too early
}