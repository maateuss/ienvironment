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
    public class SensorController
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


        [HttpPost]
        [Authorize(Roles="adm")]
        [Route("Create")]
        public async Task<ActionResult> Create([FromBody] Sensor sensor)
        {
            if (sensor == null || String.IsNullOrEmpty(sensor.MicrocontrollerID) )
            {
                return new UnprocessableEntityResult();
            }

            var Mcu = await microControllerService.FindByID(sensor.MicrocontrollerID);

            if(Mcu == null)
            {
                return new BadRequestResult();
            }


            var inserted = await sensorService.Create(sensor);

            if (inserted == null)
            {
                return new BadRequestResult();
            }



            await microControllerService.AddEquipmentReference(inserted.MicrocontrollerID, inserted.Id);

            return new OkObjectResult(inserted);



        }


        [HttpPut]
        [Authorize(Roles="adm")]
        [Route("Update")]
        public async Task<ActionResult> EditSensor(string id, [FromBody] Sensor sensor)
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

            var currentEqp = await sensorService.FindByID(id);

            

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
        [Authorize(Roles="adm")]
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
