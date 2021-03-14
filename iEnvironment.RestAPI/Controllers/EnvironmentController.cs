using iEnvironment.Domain.Models;
using iEnvironment.RestAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iEnvironment.RestAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EnvironmentController : ControllerBase
    {
        private EnvironmentService environmentService;
        public EnvironmentController()
        {
            environmentService = new EnvironmentService();
        }
        [HttpGet]
        public async Task<Environments> Get(string EnvironmentID)
        {
            return await environmentService.FindByID(EnvironmentID);
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<IEnumerable<Environments>> GetAll()
        {
            return await environmentService.FindAll();
        }



        [HttpPost]
        [Route("create")]
        public async Task<ActionResult> Create(Environments environment)
        {
            var created = await environmentService.CreateNew(environment);
            if (created)
            {
                return new OkResult();
            }
            return new BadRequestResult();
        }

        

        [HttpPut]
        [Route("edit/{id}")]
        public async Task<ActionResult> EditEnvironment([FromRoute] string id, [FromBody] Environments environment)
        {
            var edited = await environmentService.EditEnvironment(id, environment);
            if (edited)
            {
                return Ok(environment);
            }

            return new BadRequestResult();
        }

    }
}
