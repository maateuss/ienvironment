using Amazon.S3;
using Amazon.S3.Model;
using iEnvironment.Domain.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace iEnvironment.RestAPI.Services
{
    public class ImageService : BaseService<Image>
    {
        private AmazonS3Client AwsClient;
        public ImageService() : base("images")
        {
            AwsClient = new AmazonS3Client(Settings.AccessKey, Settings.AccessSecret, Amazon.RegionEndpoint.SAEast1);
        }


        public async Task<Image> UploadPhoto(IFormFile file)
        {

            byte[] fileBytes = new byte[file.Length];
            file.OpenReadStream().Read(fileBytes, 0, int.Parse(file.Length.ToString()));
            var image = new Image();

            var fileName = DateTime.Now.ToLongTimeString().Replace(".", "").Replace(":", "").Replace(" ", "") + file.FileName;
            image.FileName = fileName;
            image.Size = Convert.ToInt32(file.Length);


            PutObjectResponse response = null;

            using (var stream = new MemoryStream(fileBytes))
            {
                var request = new PutObjectRequest
                {
                    BucketName = Settings.Bucket,
                    Key = fileName,
                    InputStream = stream,
                    ContentType = file.ContentType,
                    CannedACL = S3CannedACL.PublicRead
                };

                response = await AwsClient.PutObjectAsync(request);
            };

            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception("Erro na imagem");
            }



            return image;
        }

    }
}
