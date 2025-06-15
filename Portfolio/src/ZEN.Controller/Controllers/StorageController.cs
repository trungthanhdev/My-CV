using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ZEN.Controller.Controllers
{
    // [ApiController]
    // [Route("public-api/storage")]
    // public class StorageController(ISimpleStorage simpleStorage) : ControllerBase
    // {
    //     private readonly ISimpleStorage _simpleStorage = simpleStorage;

    //     [HttpPost("upload/res")]
    //     [Authorize]
    //     public async Task<IActionResult> UploadRes(
    //         [FromForm] IFormFile file,
    //         [FromServices] IConfiguration configuration)
    //     {
    //         if (file == null || file.Length == 0)
    //             return BadRequest("File is not selected");

    //         var objectName = $"{DateTimeOffset.Now.ToUnixTimeSeconds()}-{file.FileName}";
    //         var contentType = file.ContentType;

    //         using var stream = new MemoryStream();
    //         await file.CopyToAsync(stream);
    //         stream.Seek(0, SeekOrigin.Begin);
    //         var bucketName = configuration["SimpleStorage:Bucket0:Name"] ?? "123";
    //         var fileUrl = await _simpleStorage.UploadFileAsync(bucketName, objectName, stream, stream.Length, contentType);
    //         if (string.IsNullOrEmpty(fileUrl))
    //         {
    //             return StatusCode(500, "Error uploading res");
    //         }

    //         return Ok(new { Url = fileUrl });
    //     }

    //     [HttpPost("upload/file")]
    //     [Authorize]
    //     public async Task<IActionResult> UploadFile(
    //         [FromForm] IFormFile file,
    //         [FromServices] IConfiguration configuration)
    //     {
    //         if (file == null || file.Length == 0)
    //             return BadRequest("File is not selected");

    //         var objectName = DateTime.UtcNow.Millisecond+ "-" +file.FileName;
    //         var contentType = file.ContentType;

    //         using var stream = new MemoryStream();
    //         await file.CopyToAsync(stream);
    //         stream.Seek(0, SeekOrigin.Begin);
    //         var bucketName = configuration["SimpleStorage:Bucket1:Name"] ?? "456";
    //         var fileUrl = await _simpleStorage.UploadFileAsync(bucketName, objectName, stream, stream.Length, contentType);
    //         if (string.IsNullOrEmpty(fileUrl))
    //         {
    //             return StatusCode(500, "Error uploading file");
    //         }

    //         return Ok(new { objectName  });
    //     }

     
    //     [HttpGet("download", Name = "Download")]
    //     public async Task<IActionResult> GetFile(
    //         [FromQuery] string objectName,
    //         [FromQuery] long expiration,
    //         [FromQuery] string signature,
    //         [FromServices] IConfiguration configuration)
    //     {
    //         var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    //         if (now > expiration)
    //             return Unauthorized("URL has expired.");

    //         var secret = configuration["Salt"]; // Use the same key as above
    //         var data = $"{objectName}|{expiration}";
    //         var expectedSignature = Convert.ToBase64String(new HMACSHA256(Encoding.UTF8.GetBytes(secret!)).ComputeHash(Encoding.UTF8.GetBytes(data)));

    //         if (signature != expectedSignature)
    //             return Unauthorized("Invalid signature.");

    //         try
    //         {
    //             var bucketName = configuration["SimpleStorage:Bucket1:Name"] ?? "456";
    //             var (stream, contentType) = await _simpleStorage.GetFileAsync(bucketName, objectName);
    //             return File(stream, contentType, objectName);
    //         }
    //         catch (Exception ex)
    //         {
    //             return StatusCode(500, $"Error retrieving file: {ex.Message}");
    //         }
    //     }

    //     [HttpGet("preview")]
    //     [Authorize]
    //     public IActionResult GetPreview([FromQuery] string objectName,
    //     [FromServices] IConfiguration configuration)
    //     {
    //         if (string.IsNullOrEmpty(objectName))
    //             return BadRequest("Object name cannot be null or empty.");

    //         var expiration = DateTimeOffset.UtcNow.AddMinutes(10).ToUnixTimeSeconds();
    //         var secret = configuration["Salt"]; // Use a secure key
    //         var data = $"{objectName}|{expiration}";
    //         var signature = Convert.ToBase64String(new HMACSHA256(Encoding.UTF8.GetBytes(secret!)).ComputeHash(Encoding.UTF8.GetBytes(data)));

    //         var url = Url.RouteUrl("Download", new { objectName, expiration, signature }, Request.Scheme);
    //         var httpsUrl = url!.Replace("http://", "https://");
    //         return Created(httpsUrl, new { Url = httpsUrl });
    //     }

    // }
}