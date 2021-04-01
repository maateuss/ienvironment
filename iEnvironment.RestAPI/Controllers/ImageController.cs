using iEnvironment.Domain.Models;
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
    public class ImageController : ControllerBase
    {
        private ImageService imageService;
        public ImageController()
        {
            imageService = new ImageService();
        }


        [HttpPost]
        [Route("UploadPhoto")]
        [Authorize]
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

    }
}
