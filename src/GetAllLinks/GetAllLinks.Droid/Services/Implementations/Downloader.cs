using System;
using System.Net;
using System.Threading.Tasks;
using GetAllLinks.Core.Infrastructure.POs;
using GetAllLinks.Core.Infrastructure.Services;

namespace GetAllLinks.Droid.Services.Implementations
{
	class Downloader : IDownloader
	{
		private const int ChunkSize = 4096;
		private const int MeasureSpan = 1000;

		public async Task Download(IDownloadable downloadable)
		{
			var receivedBytes = 0;
			var client = new WebClient();

			using (var stream = await client.OpenReadTaskAsync(downloadable.Url))
			{
				var buffer = new byte[ChunkSize];
				var totalBytes = int.Parse(client.ResponseHeaders[HttpResponseHeader.ContentLength]);

				var speed = 0;
				var lastUpdate = DateTime.Now;
				var lastReceivedBytes = 0;
				for (;;)
				{
					var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
					if (bytesRead == 0)
					{
						await Task.Yield();
						break;
					}
					receivedBytes += bytesRead;
					
					if (DateTime.Now > lastUpdate + TimeSpan.FromMilliseconds(MeasureSpan))
					{
						speed = (receivedBytes - lastReceivedBytes) / MeasureSpan;
						downloadable.UpdateProgress((double)receivedBytes / totalBytes, speed);
						lastUpdate = DateTime.Now;
						lastReceivedBytes = receivedBytes;
					}
				}
				downloadable.UpdateProgress((double)receivedBytes / totalBytes, speed);
			}
		}
	}
}