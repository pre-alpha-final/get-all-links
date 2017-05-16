using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using GetAllLinks.Core.Infrastructure.POs;
using GetAllLinks.Core.Infrastructure.Services;

namespace GetAllLinks.Droid.Services.Implementations
{
	class Downloader : IDownloader
	{
		private const int ChunkSize = 4096;
		private const int NumberOfChunks = 100;

		public async Task Download(IDownloadable downloadable)
		{
			int receivedBytes = 0;
			var client = new WebClient();

			using (var stream = await client.OpenReadTaskAsync(downloadable.Url))
			{
				var buffer = new byte[ChunkSize];
				var totalBytes = int.Parse(client.ResponseHeaders[HttpResponseHeader.ContentLength]);

				int speed = 0;
				Stopwatch sw = new Stopwatch();
				sw.Start();
				for (int i=1;; i++)
				{
					var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
					if (bytesRead == 0)
					{
						await Task.Yield();
						break;
					}
					receivedBytes += bytesRead;

					if (i % NumberOfChunks == 0)
					{
						sw.Stop();
						speed = ChunkSize * NumberOfChunks / sw.Elapsed.Milliseconds; // bytes / milliseconds
						sw.Start();
					}
					downloadable.UpdateProgress((double)receivedBytes / totalBytes, speed);
				}
			}
		}
	}
}