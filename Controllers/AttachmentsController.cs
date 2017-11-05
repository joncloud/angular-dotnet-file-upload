using angular_dotnet_file_upload.Models;
using angular_dotnet_file_upload.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace angular_dotnet_file_upload.Controllers
{
    [Route("api/attachments")]
    public class AttachmentsController : Controller
    {
        readonly AttachmentService _attachmentService;
        public AttachmentsController(AttachmentService attachmentService)
            => _attachmentService = attachmentService;

        IActionResult ToActionResult(IReadOnlyList<Attachment> attachments)
            => Ok(new AttachmentResponse(attachments));

        [HttpGet]
        public IActionResult Get()
            => ToActionResult(_attachmentService.List());

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
            => _attachmentService.TryDeleteAttachment(id) ? (IActionResult)Ok() : NotFound();

        [HttpGet("{id}")]
        public async Task<IActionResult> DownloadAsync(Guid id)
        {
            if (_attachmentService.TryGetAttachment(id, out var attachment))
            {
                MemoryStream file = new MemoryStream();
                await attachment.CopyToAsync(file);
                file.Position = 0;
                return File(file, attachment.ContentType, attachment.Name);
            }
            return NotFound();
        }

        async Task<Attachment> UploadFormFileAsync(IFormFile file)
        {
            using (var stream = file.OpenReadStream())
            {
                return await _attachmentService.UploadAsync(file.Name, file.ContentType, stream);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadAsync()
        {
            if (Request.HasFormContentType && Request.Form.Files.Any())
            {
                var tasks = Request.Form.Files.Select(UploadFormFileAsync);
                var results = await Task.WhenAll(tasks);
                return ToActionResult(results);
            }

            ModelState.AddModelError("", "At least one file must be uploaded");
            return BadRequest(ModelState);
        }
    }
}
