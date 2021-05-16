using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using iEnvironment.Domain.Models;
using iEnvironment.RestAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace iEnvironment.RestAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventController : ControllerBase
    {
        private EventService _eventService;

        public EventController()
        {
            _eventService = new EventService();
        }

        [HttpGet]
        [Route("getAll")]
        [Authorize]

        public async Task<IEnumerable<EventDefinition>> GetAll()
        {
            return await _eventService.FindAll();
        }

        [HttpPost]
        [Route("create")]
        [Authorize]
        public async Task<ActionResult> CreateNew([FromBody] EventDefinition ed)
        {
            var valid = await _eventService.Create(ed);

            if (valid != null)
            {
                return Ok(valid);
            }

            return UnprocessableEntity();

            
        }

        [HttpPut]
        [Route("Update")]
        [Authorize]
        public async Task<ActionResult> Update([FromBody] EventDefinition ed)
        {
            var valid = await _eventService.Update(ed);
            if (valid)
            {
                return Ok();
            }

            return BadRequest();
        }

        [HttpDelete]
        [Route("Delete")]
        [Authorize]
        public async Task<ActionResult> Delete([FromBody] EventDefinition ed, string id)
        {
            if(ed.Id == id)
            {
                var valid = await _eventService.Delete(id);

                if (valid)
                {
                    return Ok();
                }
            }

            return BadRequest();
        }


    }
}
