using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
    public class EventController : ControllerBase
    {
        private EventService _eventService;
        private EnvironmentService _environmentService;
        private ActuatorService _actuatorService;
        private SensorService _sensorService;
        private MqttService _mqttService;
        public EventController()
        {
            _eventService = new EventService();
            _environmentService = new EnvironmentService();
            _sensorService = new SensorService();
            _actuatorService = new ActuatorService();
            _mqttService = new MqttService();
        }

        [HttpGet]
        [Route("getAll")]
        [Authorize]

        public async Task<IEnumerable<Event>> GetAll()
        {
            return await _eventService.FindAll();
        }

        [HttpPost]
        [Route("create")]
        [Authorize]
        public async Task<ActionResult> CreateNew([FromBody] Event ed)
        {
            ed.CreatedAt = DateTime.Now;
            if (ed.EnvironmentID == null) return new BadRequestObjectResult("Id do ambiente obrigatório");

            if (ed.TimeBased)
            {
                if (!(ed.RunningDays?.Any() ?? false))
                {
                    return new BadRequestObjectResult("Necessário assinalar quais dias da semana o evento programado irá ocorrer");
                }

                if(!Regex.IsMatch(ed.StartTime ?? "" , "^(?:[01]?\\d|2[0-3])(?::[0-5]\\d){1,2}$"))
                {
                    return new BadRequestObjectResult("StartTime não está respeitando o regex ^(?:[01]?\\d|2[0-3])(?::[0-5]\\d){1,2}$ ");
                }

                if (!Regex.IsMatch(ed.EndTime ?? "", "^(?:[01]?\\d|2[0-3])(?::[0-5]\\d){1,2}$"))
                {
                    return new BadRequestObjectResult("EndTime não está respeitando o regex ^(?:[01]?\\d|2[0-3])(?::[0-5]\\d){1,2}$ ");
                }
            }

            if (ed.IsManual && (ed.WhenExecute?.Any() ?? false))
            {
                return new BadRequestObjectResult("Eventos manuais não podem ter condições");
            }

            if(!ed.IsManual && !ed.TimeBased && !(ed.WhenExecute?.Any() ?? false)) {
                return new BadRequestObjectResult("Eventos automáticos devem possuir ao menos 1 condição");
            }

            if (!(ed.WhatExecute?.Any() ?? false))
            {
                return new BadRequestObjectResult("Eventos precisam possuir ao menos 1 atuador para ser ativado");
            }

            foreach (var item in ed.WhatExecute)
            {
                var atuador = await _actuatorService.FindByID(item.ActuatorId);
                if(atuador == null)
                {
                    return new BadRequestObjectResult($"Atuador {item} não encontrado");
                }

            }

            var environment = await _environmentService.FindByID(ed.EnvironmentID);
            if(environment == null)
            {
                return new BadRequestObjectResult($"Ambiente {ed.EnvironmentID} não encontrado");
            }




            var valid = await _eventService.Create(ed);

          

            if (valid != null)
            {
                await _environmentService.AddEventReference(valid.EnvironmentID, valid.Id);

                return Ok(valid);
            }

            return UnprocessableEntity();

            
        }


        [HttpGet]
        [Route("GetByEnvironmentId/{id}")]
        [Authorize]
        public async Task<ActionResult> GetByEnvironmentId([FromRoute] string id)
        {
            var events = await _eventService.FindAll();
            var filtered = events.Where(x => x.EnvironmentID == id);
            return Ok(filtered);
        }

        [HttpGet]
        [Route("RaiseManualEvent/{id}")]
        [Authorize]
        public async Task<ActionResult> RaiseEvent([FromRoute] string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return new NotFoundObjectResult("Id Obrigatório");
            var currentEvent = await _eventService.FindByID(id);

            if (!currentEvent.IsManual)
            {
                return new BadRequestObjectResult("Apenas eventos manuais podem ser acionados dessa forma");
            }

            currentEvent.UpdatedAt = DateTime.Now;

            var listOfTopics = new List<(string topic, string payload)>();
            foreach (var item in currentEvent.WhatExecute)
            {
                var atuador = await _actuatorService.FindByID(item.ActuatorId);
                listOfTopics.Add((atuador.Topic, item.Value));
            }

            var response = await _mqttService.SendMessages(listOfTopics);

            if (response.result)
            {
                await _eventService.Update(currentEvent);

                return Ok();
            }

            else return new BadRequestObjectResult($"Não foi possivel acionar o evento manual \n {response.message}");




        }


        [HttpPut]
        [Route("Update")]
        [Authorize]
        public async Task<ActionResult> Update([FromBody] Event ed)
        {
            var valid = await _eventService.Update(ed);
            if (valid)
            {
                return Ok();
            }

            return BadRequest();
        }

        [HttpDelete]
        [Route("Delete/{id}")]
        [Authorize]
        public async Task<ActionResult> Delete([FromRoute] string id)
        {

            var currentEvent = await _eventService.FindByID(id);

            if (currentEvent == null) return new NotFoundObjectResult("evento não encontrado");

            var valid = await _eventService.Delete(id);
            
            if (valid)
            {
                await _environmentService.RemoveEventReference(currentEvent.EnvironmentID, id);
                return Ok();
            }

            return BadRequest();
        }


    }
}
