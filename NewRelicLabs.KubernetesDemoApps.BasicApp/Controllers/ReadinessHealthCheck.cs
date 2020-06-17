using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NewRelicLabs.KubernetesDemoApps.BasicApp.Controllers
{
    public class ReadinessHealthCheck : IHealthCheck
    {
        private IMemoryCache memoryCache;

        public ReadinessHealthCheck(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            if (memoryCache.Get<string>("readiness")== "healthy")
            {
                return Task.FromResult(HealthCheckResult.Healthy(""));
            } 
            else
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("failed"));
            }
        }
    }
}
