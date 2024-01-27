using System.Diagnostics;
using MelbergFramework.Infrastructure.Rabbit.Metrics;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Application.Filters;
    public interface IStopwatch
    {
        long ElapsedMilliseconds { get; }
        void Start();
        void Stop();
        void Reset();
        void Report(long ms);
    }
    [AttributeUsage(validOn: AttributeTargets.Class | AttributeTargets.Method)]
    public class ResponseTimeFilter : Attribute, IActionFilter
    {
        private IActionResponseTimeStopwatch GetStopwatch(HttpContext context)
        {
            return context.RequestServices.GetService<IActionResponseTimeStopwatch>();
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            IStopwatch watch = GetStopwatch(context.HttpContext);
            watch.Reset();
            watch.Start();
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            IStopwatch watch = GetStopwatch(context.HttpContext);
            watch.Stop();
            string value = string.Format("{0}ms", watch.ElapsedMilliseconds);       
            context.HttpContext.Response.Headers["X-Action-Response-Time"] = value;
            watch.Report(watch.ElapsedMilliseconds);
        }
    }

    public interface IActionResponseTimeStopwatch : IStopwatch
    {
    }

    public class ActionResponseTimeStopwatch : Stopwatch, IActionResponseTimeStopwatch
    {
        private readonly IMetricPublisher _publisher;
        public ActionResponseTimeStopwatch(IMetricPublisher publisher) : base()
        {
            _publisher = publisher;
        }

    public void Report(long ms)
    {
        _publisher.SendMetric("api_response",ms,DateTime.UtcNow);
    }
}