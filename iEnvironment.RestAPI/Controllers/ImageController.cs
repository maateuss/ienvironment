﻿using iEnvironment.Domain.Models;
using iEnvironment.RestAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class ImageController : ControllerBase
    {
        private ImageService imageService;
        public ImageController()
        {
            imageService = new ImageService();
        }


        [HttpPost]
        [Route("UploadPhoto")]
        [AllowAnonymous]
        public async Task<ActionResult<Image>> UploadImage([FromForm] IFormFile file)
        {
            if(file == null)
            {
                return BadRequest();
            }

            var result = await imageService.UploadPhoto(file);
            if(result == null)
            {
                return new UnprocessableEntityResult();
            }

            return new OkObjectResult(result);
        }

        [HttpGet]
        [Authorize]
        public async Task<Image> Get(string ImageID)
        {
            return await imageService.FindByID(ImageID);
        }

        [HttpGet]
        [Route("GetAll")]
        [AllowAnonymous]
        public async Task<IEnumerable<Image>> GetAll()
        {
            return await imageService.FindAll();
        }

        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<ActionResult> Delete([FromRoute] string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var current = await imageService.FindByID(id);
                if (current == null)
                {
                    return new NotFoundObjectResult("Image not found");
                }

                var deleted = await imageService.DeleteObjectNonVersionedBucketAsync(current);

                if (deleted)
                {
                    return new OkObjectResult("Image deleted");
                }
                return new NotFoundObjectResult("Image not found");
            }

            return new BadRequestObjectResult("Id invalido");
        }

    }
}
