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
    public class ActuatorController
    {
        private ActuatorService actuatorService;

        private MCUService microControllerService;
        private EnvironmentService environmentService;

        public ActuatorController()
        {
            actuatorService = new ActuatorService();
            microControllerService = new MCUService();
            environmentService = new EnvironmentService();
        }

        [HttpGet]
        [Route("GetAll")]
        [Authorize(Roles = "adm,user,broker,guest")]
        public async Task<IEnumerable<Actuator>> GetAll()
        {
            return await actuatorService.FindAll();
        }


        [HttpPost]
        [Route("Create")]
        [Authorize(Roles = "adm,user,broker,guest")]
        public async Task<ActionResult> Create([FromBody] Actuator actuator)
        {
            if (actuator == null)
            {
                return new UnprocessableEntityResult();
            }
            var Mcu = await microControllerService.FindByID(actuator.MicrocontrollerID);

            if (Mcu == null)
            {
                return new BadRequestResult();
            }

            var inserted = await actuatorService.Create(actuator);

            if (inserted == null)
            {
                return new BadRequestResult();
            }

            return new OkObjectResult(inserted);
        }


        [HttpPut]
        [Route("Update")]
        [Authorize(Roles = "adm")]
        public async Task<ActionResult> EditActuator(string id, [FromBody] Actuator actuator)
        {
            if(actuator == null || String.IsNullOrWhiteSpace(id))
            {
                return new NotFoundResult();
            }

            var Mcu = await microControllerService.FindByID(actuator.MicrocontrollerID);

            if (Mcu == null)
            {
                return new BadRequestResult();
            }

            var currentEqp = await actuatorService.FindByID(id);

            var valid = await actuatorService.Update(id, actuator);

            if (valid)
            {
                if (currentEqp.MicrocontrollerID != actuator.MicrocontrollerID)
                {
                    await microControllerService.UpdateEquipmentReference(oldId: currentEqp.MicrocontrollerID, newId: actuator.MicrocontrollerID, eqpId: id);
                }


                return new OkResult();
            }

            return new BadRequestResult();
            

        }


        [HttpDelete]
        [Route("Delete")]
        [Authorize(Roles="adm")]
        public async Task<ActionResult> Delete(string id)
        {
            var valid = await actuatorService.Delete(id);

            if (valid)
            {
                RemoveReferences(id);
                return new OkResult();
            }

            return new BadRequestResult();
        }

        private void RemoveReferences(string id)
        {
            microControllerService.RemoveEquipmentReference(id);
            environmentService.RemoveEquipmentReference(id);
        }


    }
}
