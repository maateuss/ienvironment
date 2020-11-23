using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace iEnvironment.RestAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
 

        [HttpGet]
        public IEnumerable<String> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => index.ToString())
            .ToArray();
        }
    }
}
