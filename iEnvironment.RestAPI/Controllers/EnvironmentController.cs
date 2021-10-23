using iEnvironment.Domain.Models;
using iEnvironment.RestAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iEnvironment.RestAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class EnvironmentController : ControllerBase
    {
        private EnvironmentService environmentService;
        private EventService eventService;
        private SensorService sensorService;
        public EnvironmentController()
        {
            environmentService = new EnvironmentService();
            eventService = new EventService();
            sensorService = new SensorService();
        }
        [HttpGet]
        [Authorize]
        public async Task<Environments> Get(string EnvironmentID)
        {
            return await environmentService.FindByID(EnvironmentID);
        }

        [HttpGet]
        [Route("GetAll")]
        [Authorize]
        public async Task<IEnumerable<Environments>> GetAll()
        {
            return await environmentService.FindAll();
        }

        [HttpGet]
        [Route("Dashboard")]
        [Authorize]
        public async Task<ActionResult> GetDashboard()
        {
            var envs = await environmentService.FindAll();
            var viewModels = new List<EnvironmentViewModel>();
            foreach (var item in envs)
            {
                var sensorList = new List<Sensor>();
                var eventList = new List<Event>();
                foreach (var sens in item.Equipments)
                {
                    var sensor = await sensorService.FindByID(sens);
                    if (sensor != null) sensorList.Add(sensor);
                }

                foreach (var evt in item.Events)
                {
                    var fullEvent = await eventService.FindByID(evt);
                    if (fullEvent != null) eventList.Add(fullEvent);
                }

                viewModels.Add(new EnvironmentViewModel(item, sensorList, eventList));
            }


            return new OkObjectResult(viewModels);
        }

        [HttpDelete]
        [Authorize(Roles = "admin")]
        [Route("Delete/{id}")]
        public async Task<ActionResult> Delete([FromRoute] string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var current = await environmentService.FindByID(id);
                if (current == null)
                {
                    return new NotFoundObjectResult("ambiente não encontrado");
                }
                var deleted = await environmentService.Delete(id);
                if (deleted)
                {
                    return new OkObjectResult("ambiente deletado");
                }
                return new NotFoundObjectResult("ambiente não encontrado");
            }

            return new BadRequestObjectResult("Id invalido");

        }


        [HttpPost]
        [Route("create")]
        [Authorize(Roles = "admin")]
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
        [Authorize(Roles = "admin")]

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
