﻿using iEnvironment.Domain.Models;
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
    public class MCUControllers : ControllerBase
    {
        private MCUService microControllerService;
        public MCUControllers()
        {
            microControllerService = new MCUService();
        }

        [HttpGet]
        [Route("getAll")]
        [Authorize]

        public async Task<IEnumerable<MicroController>> GetAllMCU()
        {
            return await microControllerService.FindAll();
        }


        [HttpPost]
        [Authorize(Roles="adm")]
        [Route("create")]
        public async Task<ActionResult> Create([FromBody] MicroController device)
        {
            var result = await microControllerService.CreateNew(device);
            if (result)
            {
                return Ok();
            }

            return BadRequest();
        }

        [HttpGet]
        [Authorize]
        [Route("getById/{id}")]
        public async Task<ActionResult<MicroController>> GetById([FromRoute] string id)
        {
            var result = await microControllerService.FindByID(id);
            if(result != null)
            {
                return Ok(result);
            }
            return NotFound();
        }

        [HttpPut]
        [Authorize(Roles="adm")]
        [Route("edit/{id}")]
        public async Task<ActionResult> Edit([FromRoute] string id, [FromBody] MicroController device)
        {
            var result = await microControllerService.Update(id, device);
            if (result)
            {
                return Ok();
            }
            return BadRequest();
        }


        [HttpDelete]
        [Authorize(Roles ="adm")]
        [Route("delete/{id}")]
        public async Task<ActionResult> Delete([FromRoute]string id)
        {
            var result = await microControllerService.Delete(id);
            if (result)
            {
                return Ok();
            }
            return BadRequest();
        }

    }
}
