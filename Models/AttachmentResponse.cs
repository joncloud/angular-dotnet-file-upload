using System.Collections.Generic;

namespace angular_dotnet_file_upload.Models
{
    public class AttachmentResponse
    {
        public IReadOnlyList<Attachment> Attachments { get; }
        public AttachmentResponse(IReadOnlyList<Attachment> attachments) 
            => Attachments = attachments;
    }
}
