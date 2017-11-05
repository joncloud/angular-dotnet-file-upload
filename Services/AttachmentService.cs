using angular_dotnet_file_upload.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace angular_dotnet_file_upload.Services
{
    public class AttachmentService
    {
        readonly ConcurrentDictionary<Guid, Attachment> _attachments = new ConcurrentDictionary<Guid, Attachment>();

        public bool TryGetAttachment(Guid id, out Attachment attachment)
            => _attachments.TryGetValue(id, out attachment);

        public bool TryDeleteAttachment(Guid id)
        {
            if (_attachments.TryRemove(id, out var attachment))
            {
                attachment.Dispose();
                return true;
            }
            return false;
        }

        public IReadOnlyList<Attachment> List()
            => _attachments.Values.OrderBy(a => a.Name).ToList().AsReadOnly();

        public async Task<Attachment> UploadAsync(string name, string contentType, Stream contents)
        {
            Guid id = Guid.NewGuid();
            var attachment = await Attachment.CopyFromAsync(id, name, contentType, contents);
            _attachments.GetOrAdd(id, attachment);
            return attachment;
        }
    }
}
