using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NLog;

namespace NewRelicLabs.KubernetesDemoApps.BasicApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class InvokeController : ControllerBase
    {
        public string RemoteURL { get; set; }

        public InvokeController(IConfiguration configuration)
        {
            RemoteURL = configuration["REMOTE_URL"];
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            if (string.IsNullOrEmpty(RemoteURL))
            {
                return Ok("ended.");
            }
            else
            {
                var client = new HttpClient();
                var res = await client.GetAsync(RemoteURL);
                var content = await res.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
                LogManager.GetCurrentClassLogger().Info(content);
                return Ok("invoked to next chain");
            }
        }
    }
}
