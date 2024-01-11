using Common;
using DomainService;
using Application.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Application.Responses;
using System.Diagnostics;

namespace Application;

[ApiController]
[Route("[controller]")]
public class WranglerController
{
    private readonly long _offset;
    private readonly IAccessDomainService _service;
    public WranglerController(
        IAccessDomainService service,
        IOptions<TimingsOptions> timingsOptions
        )
    {
        _service = service;
        _offset = timingsOptions.Value.CompilationOffsetSecs +
            timingsOptions.Value.CompilationDurationPredictionSecs;
    }

    [HttpGet]
    [Route("frame")]
    [ProducesResponseType(typeof(PlaneFrameResponse),200)]
    public async Task<IActionResult> GetFrameAsync([FromQuery] long? time)
    {
        time ??= MaxTime;
        
        if(time > MaxTime)
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