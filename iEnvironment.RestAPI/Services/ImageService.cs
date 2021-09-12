using Amazon.S3;
using Amazon.S3.Model;
using iEnvironment.Domain.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using MongoDB.Driver;

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
            var extension = Path.GetExtension(file.FileName);
            var fileName = DateTime.Now.ToLongTimeString().Replace(".", "").Replace(":", "").Replace(" ", "") + file.FileName;


            StringBuilder sb = new StringBuilder();

            using (HashAlgorithm algorithm = SHA256.Create()) {
                foreach (var item in algorithm.ComputeHash(Encoding.UTF8.GetBytes(fileName)))
                {
                    sb.Append(item.ToString("X2"));
                }
                fileName = sb.ToString() + extension;

            }
            
            image.FileName = fileName;
            image.AltName = file.FileName;
            image.Size = Convert.ToInt32(file.Length);
            image.Url = Settings.S3Prefix + image.FileName;

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
            image.CreatedAt = DateTime.Now;
            image.UpdatedAt = DateTime.Now;
            Collection.InsertOne(image);

            var result = await Collection.Find(x => x.Url == image.Url).FirstOrDefaultAsync();

            return result;
        }

        public async Task<bool> DeleteObjectNonVersionedBucketAsync(Image image)
        {
            try
            {
                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = Settings.Bucket,
                    Key = image.FileName
                };

                await AwsClient.DeleteObjectAsync(deleteObjectRequest);

                var result = base.Delete(image.Id);

                return result.Result;
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when deleting an object", e.Message);
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when deleting an object", e.Message);
                return false;
            }
        }
    }
}
