using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NLog;

namespace NewRelicLabs.KubernetesDemoApps.BasicApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProbeController : ControllerBase
    {
        private IMemoryCache memoryCache;

        public ProbeController(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        [HttpGet]
        public IActionResult Set([FromQuery]string type, [FromQuery]bool disabled=true)
        {
            var value = disabled ? "unhealthy" : "healthy";
            LogManager.GetCurrentClassLogger().Info($"set {type} probe to {value}");
            memoryCache.Set(type, value);
            return Ok();
        }
    }
}
