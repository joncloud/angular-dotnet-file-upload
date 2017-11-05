using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace angular_dotnet_file_upload.Models
{
    public class Attachment : IDisposable
    {
        readonly Stream _data;
        readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
        public Guid Id { get; }
        public string Name { get; }
        public string ContentType { get; }

        Attachment(Guid id, string name, string contentType, Stream data)
        {
            Id = id;
            Name = name;
            ContentType = contentType;
            _data = data;
        }

        public static async Task<Attachment> CopyFromAsync(Guid id, string name, string contentType, Stream source)
        {
            MemoryStream target = new MemoryStream();
            await source.CopyToAsync(target);

            target.Position = 0;
            return new Attachment(id, name, contentType, target);
        }

        public async Task CopyToAsync(Stream target)
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                await _data.CopyToAsync(target);
                _data.Position = 0;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public void Dispose()
            => _data.Dispose();
    }
}
