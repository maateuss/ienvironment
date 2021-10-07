using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iEnvironment.Domain.Models;
using iEnvironment.RestAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iEnvironment.RestAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class ActuatorController : ControllerBase
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
        /// <summary>
        /// Listar todos os Atuadores
        /// </summary>
        [HttpGet]
        [Route("GetAll")]
        [Authorize]
        public async Task<IEnumerable<Actuator>> GetAll()
        {
            return await actuatorService.FindAll();
        }

        /// <summary>
        /// Listar todos os Atuadores que pertencem a esse ambiente
        /// </summary>
        [HttpGet]
        [Route("GetByEnvironmentId/{id}")]
        [Authorize]
        public async Task<ActionResult> GetByEnvironmentId([FromRoute] string id)
        {
            var events = await actuatorService.FindAll();
            var filtered = events.Where(x => x.EnvironmentId == id);
            return Ok(filtered);
        }



        /// <summary>
        /// Cadastro de novo atuador
        /// </summary>
        /// <remarks>
        /// Exemplo de requisição:
        ///
        ///
        ///     POST /Create
        ///     {
        ///         "name": "Nome do atuador",
        ///         "description": "Descrição do atuador",
        ///         "entityType": 0,
        ///         "environmentId": "604bf393b46162214e9f05ca",
        ///         "enabled": true,
        ///         "simulationMode": false,
        ///         "microcontrollerID": "60a3bf68ee5c5fcf3fcded4f",
        ///         "autoDisconnectSeconds": 300
        ///     }
        ///
        /// </remarks>
        /// <param name="actuator"> entity type = String = 0,Int = 1,Boolean = 2,Numeric = 3 esse parametro será o tipo de dado a ser enviado para o atuador.</param>
        /// <returns>O atuador cadastrado</returns>
        [HttpPost]
        [Route("Create")]
        [Authorize(Roles = "admin")]
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

            var Environment = await environmentService.FindByID(actuator.EnvironmentId);

            actuator.CreatedAt = DateTime.Now;
            if (Environment == null)
            {
                return new BadRequestObjectResult("Invalid EnvironmentID");
            }



            var inserted = await actuatorService.Create(actuator);
            if (inserted == null)
            {
                return new BadRequestResult();
            }

            await environmentService.AddEquipmentReference(inserted.EnvironmentId, inserted.Id);
            await microControllerService.AddEquipmentReference(inserted.MicrocontrollerID, inserted.Id);

            return new OkObjectResult(inserted);
        }


        [HttpPut]
        [Route("Update")]
        [AllowAnonymous]
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
        [AllowAnonymous]
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
