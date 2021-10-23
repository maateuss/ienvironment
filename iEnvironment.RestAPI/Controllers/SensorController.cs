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
    public class SensorController : ControllerBase
    {
        private SensorService sensorService;
        private MCUService microControllerService;
        private EnvironmentService environmentService;

        public SensorController()
        {
            sensorService = new SensorService();
            microControllerService = new MCUService();
            environmentService = new EnvironmentService();
        }

        [HttpGet]
        [Authorize]
        [Route("GetAll")]
        public async Task<IEnumerable<Sensor>> GetAll()
        {
            return await sensorService.FindAll();
        }

        [HttpGet]
        [Authorize]
        [Route("getById/{id}")]
        public async Task<ActionResult> GetById([FromRoute] string id)
        {
            var result = await sensorService.FindByID(id);
            if (result != null)
            {
                return Ok(result);
            }
            return NotFound();
        }

        [HttpGet]
        [Route("GetByEnvironmentId/{id}")]
        [Authorize]
        public async Task<ActionResult> GetByEnvironmentId([FromRoute] string id)
        {
            var events = await sensorService.FindAll();
            var filtered = events.Where(x => x.EnvironmentId == id);
            return Ok(filtered);
        }



        /// <summary>
        /// Cadastro de novo sensor
        /// </summary>
        /// <remarks>
        /// Exemplo de requisição:
        ///
        ///
        ///     POST /Create
        ///     {
        ///         "measurementUnit": "Nadas por minuto",
        ///         "defaultTriggersActive": false,
        ///         "limitUp": null,
        ///         "limitDown": null,
        ///         "name": "Nome do sensor",
        ///         "description": "Descrição do sensor",
        ///         "entityType": 0,
        ///         "environmentId": "604bf393b46162214e9f05ca",
        ///         "enabled": true,
        ///         "simulationMode": false,
        ///         "microcontrollerID": "60a3bf68ee5c5fcf3fcded4f",
        ///         "autoDisconnectSeconds": 300
        ///     }
        ///
        /// </remarks>
        /// <param name="sensor"> entity type = String = 0,Int = 1,Boolean = 2,Numeric = 3 esse parametro será o tipo a ser considerado nos triggers limitup e limitdown caso aplicavel.</param>
        /// <returns>O sensor cadastrado</returns>
        [HttpPost]
        [Authorize(Roles = "admin")]
        [Route("Create")]
        public async Task<ActionResult> Create([FromBody] Sensor sensor)
        {
            if (sensor == null || string.IsNullOrEmpty(sensor.MicrocontrollerID) || string.IsNullOrEmpty(sensor.EnvironmentId))
            {
                return new UnprocessableEntityResult();
            }

            var Mcu = await microControllerService.FindByID(sensor.MicrocontrollerID);

            if(Mcu == null)
            {
                return new BadRequestResult();
            }

            var Environment = await environmentService.FindByID(sensor.EnvironmentId);

            if(Environment == null)
            {
                return new BadRequestObjectResult("Invalid EnvironmentID");
            }


            sensor.CreatedAt = DateTime.Now;
            var inserted = await sensorService.Create(sensor);

            if (inserted == null)
            {
                return new BadRequestResult();
            }



            await microControllerService.AddEquipmentReference(inserted.MicrocontrollerID, inserted.Id);
            await environmentService.AddEquipmentReference(inserted.EnvironmentId, inserted.Id);
            return new OkObjectResult(inserted);



        }


        [HttpPut]
        [Authorize(Roles = "admin")]
        [Route("Update/{id}")]
        public async Task<ActionResult> EditSensor([FromRoute]string id, [FromBody] Sensor sensor)
        {
            if (sensor == null || String.IsNullOrWhiteSpace(id))
            {
                return new NotFoundResult();
            }

            var Mcu = await microControllerService.FindByID(sensor.MicrocontrollerID);

            if (Mcu == null)
            {
                return new BadRequestResult();
            }
            Sensor currentEqp = new Sensor();

            try
            {
                currentEqp = await sensorService.FindByID(id);
            }
            catch
            {
                return new NotFoundObjectResult("Sensor não encontrado");
            }
            

            var valid = await sensorService.Update(id, sensor);

            if (valid)
            {
                if (currentEqp.MicrocontrollerID != sensor.MicrocontrollerID)
                {
                    await microControllerService.UpdateEquipmentReference(oldId: currentEqp.MicrocontrollerID,newId:  sensor.MicrocontrollerID,eqpId: id);
                }


                return new OkResult();
            }


            return new BadRequestResult();


        }

        [HttpDelete]
        [Authorize(Roles = "admin")]
        [Route("Delete")]
        public async Task<ActionResult> DeleteSensor(string id)
        {
            if (id == null) return new BadRequestResult();

            var valid = await sensorService.Delete(id);
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
