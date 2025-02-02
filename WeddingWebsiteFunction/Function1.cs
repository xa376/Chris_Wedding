using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;

namespace TestingSAS
{
    public static class Function1
    {
        [FunctionName("GetUploadBlobSAS")]
        public static IActionResult GetUploadBlobSAS(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string connectionString = System.Environment.GetEnvironmentVariable("PhotoStore_ConnString");

            string containerName = "weddingphotoscontainer";

            string blobName = System.Guid.NewGuid().ToString(); // Generate a new unique file name

            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            BlobSasBuilder sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerClient.Name,
                BlobName = blobClient.Name,
                Resource = "b"
            };
            sasBuilder.StartsOn = DateTimeOffset.UtcNow;
            sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddHours(1);
            sasBuilder.SetPermissions(BlobSasPermissions.Write);

            Uri sasUri = blobClient.GenerateSasUri(sasBuilder);

            return new OkObjectResult(sasUri);
        }
        [FunctionName("GetBrowseContainerSAS")]
        public static IActionResult GetBrowseContainerSAS(
                [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
                ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string connectionString = System.Environment.GetEnvironmentVariable("PhotoStore_ConnString");

            string containerName = "weddingphotoscontainer";

            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            BlobSasBuilder sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerClient.Name,
                Resource = "c"
            };
            sasBuilder.StartsOn = DateTimeOffset.UtcNow;
            sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddHours(1);
            sasBuilder.SetPermissions(BlobSasPermissions.List | BlobSasPermissions.Read);

            Uri sasUri = containerClient.GenerateSasUri(sasBuilder);

            return new OkObjectResult(sasUri);
        }

    }
}
