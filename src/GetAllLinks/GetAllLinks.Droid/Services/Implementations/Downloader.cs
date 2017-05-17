using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Android;
using Android.Content.PM;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using GetAllLinks.Core.Infrastructure.POs;
using GetAllLinks.Core.Infrastructure.Services;
using MvvmCross.Platform;
using MvvmCross.Platform.Droid.Platform;

namespace GetAllLinks.Droid.Services.Implementations
{
	class Downloader : IDownloader
	{
		private const int ChunkSize = 4096;
		private const int MeasureSpan = 1000;

		public async Task Download(IDownloadable downloadable)
		{
			GetPermission(Manifest.Permission.ReadExternalStorage);
			GetPermission(Manifest.Permission.WriteExternalStorage);

			var activity = Mvx.Resolve<IMvxAndroidCurrentTopActivity>().Activity;
			var mediaDirs = activity.GetExternalMediaDirs();
			var targetDirectory = "/storage/697B-091B/Android/media/com.getalllinks";

			var receivedBytes = 0;
			var client = new WebClient();
			using (var netStream = await client.OpenReadTaskAsync(downloadable.Url))
			{
				using (var fileStream = File.Create($"{targetDirectory}/{downloadable.Name}"))
				{
					var buffer = new byte[ChunkSize];
					var totalBytes = int.Parse(client.ResponseHeaders[HttpResponseHeader.ContentLength]);

					var speed = 0;
					var lastUpdate = DateTime.Now;
					var lastReceivedBytes = 0;
					for (;;)
					{
						var bytesRead = await netStream.ReadAsync(buffer, 0, buffer.Length);
						if (bytesRead == 0)
						{
							await Task.Yield();
							break;
						}
						receivedBytes += bytesRead;
						await fileStream.WriteAsync(buffer, 0, buffer.Length);

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

		public async Task<string> DownloadList(string url)
		{
			var client = new WebClient();
			using (var netStream = await client.OpenReadTaskAsync(url))
			{
				using (var streamReader = new StreamReader(netStream))
				{
					return await streamReader.ReadToEndAsync();
				}
			}
		}

		private void GetPermission(string requestedPermission)
		{
			var activity = Mvx.Resolve<IMvxAndroidCurrentTopActivity>().Activity;
			if (ContextCompat.CheckSelfPermission(activity,
				requestedPermission) != Permission.Granted)
			{
				ActivityCompat.RequestPermissions(activity,
				new[] { requestedPermission }, 0);
			}
		}
	}
}