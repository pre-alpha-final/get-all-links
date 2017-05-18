using System;
using System.IO;
using System.Linq;
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
using GetAllLinks.Core.Helpers;

namespace GetAllLinks.Droid.Services.Implementations
{
	class Downloader : IDownloader
	{
		private const int ChunkSize = 4096;
		private const int MeasureSpan = 500;
		private const int MeasureCount = 10;
		private CircularBuffer _sentData;


		public async Task Download(IDownloadable downloadable)
		{
			GetPermission(Manifest.Permission.ReadExternalStorage);
			GetPermission(Manifest.Permission.WriteExternalStorage);

			var activity = Mvx.Resolve<IMvxAndroidCurrentTopActivity>().Activity;
			activity.GetExternalMediaDirs();
			var targetDirectory = Settings.DestinationDirectory;

			if (Uri.IsWellFormedUriString(downloadable.Url, UriKind.RelativeOrAbsolute) == false ||
				downloadable.Url.StartsWith("&"))
			{
				downloadable.UpdateProgress(0, 0, "error: incorrect url");
				return;
			}

			var receivedBytes = 0;
			var client = new WebClient();
			using (var netStream = await client.OpenReadTaskAsync(downloadable.Url))
			{
				var fileName = $"{targetDirectory}/{downloadable.Name}";
				if (File.Exists(fileName))
				{
					downloadable.UpdateProgress(0, 0, "error: file exists");
					return;
				}

				using (var fileStream = File.Create(fileName))
				{
					var buffer = new byte[ChunkSize];
					var totalBytes = int.Parse(client.ResponseHeaders[HttpResponseHeader.ContentLength]);

					var speed = 0;
					var lastUpdate = DateTime.Now;
					var lastReceivedBytes = 0;
					_sentData = new CircularBuffer(MeasureCount);
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
							_sentData.Add(receivedBytes - lastReceivedBytes);
							speed = _sentData.GetAverage() / MeasureSpan;
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

		public Task<string> GetDefaultDownloadDir()
		{
			var activity = Mvx.Resolve<IMvxAndroidCurrentTopActivity>().Activity;
			var mediaDirs = activity.GetExternalMediaDirs(); // creates the dirs if not already present
			return Task.FromResult(mediaDirs.Last().AbsolutePath);
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