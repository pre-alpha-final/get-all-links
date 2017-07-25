using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
using ModernHttpClient;
using System.Net.Http;
using System.Net.Http.Headers;
using Java.IO;
using File = System.IO.File;

namespace GetAllLinks.Droid.Services.Implementations
{
	class Downloader : IDownloader
	{
		private const int ChunkSize = 4096;
		private const int MeasureSpan = 500;
		private const int MeasureCount = 5;
		private CircularBuffer _sentData;


		public async Task Download(IDownloadable downloadable)
		{
			GetPermission(Manifest.Permission.ReadExternalStorage);
			GetPermission(Manifest.Permission.WriteExternalStorage);

			var activity = Mvx.Resolve<IMvxAndroidCurrentTopActivity>().Activity;
			activity.GetExternalMediaDirs();
			var targetDirectory = Settings.DestinationDirectory;

			var fileName = $"{targetDirectory}/{downloadable.Name}";
			var fileSize = 0L;
			if (File.Exists(fileName))
			{
				var fileInfo = new FileInfo(fileName);
				fileSize = fileInfo.Length;
			}

			var client = new HttpClient(new NativeMessageHandler());
			var request = new HttpRequestMessage(HttpMethod.Get, downloadable.Url);
			if (fileSize > 0)
				request.Headers.Range = new RangeHeaderValue(fileSize, null);
			var response = await client.SendAsync(
				request,
				HttpCompletionOption.ResponseHeadersRead);
			if (response.IsSuccessStatusCode == false)
			{
				downloadable.UpdateProgress(0, 0, $"error: {response.ReasonPhrase}");
				return;
			}

			var receivedBytes = (int) fileSize;
			using (var netStream = await response.Content.ReadAsStreamAsync())
			{
				using (var fileStream = new FileOutputStream(fileName, true))
				{
					var buffer = new byte[ChunkSize];
					var totalBytes = (response.Content.Headers.ContentLength ?? -1) + fileSize;

					var speed = 0;
					var lastUpdate = DateTime.Now;
					var stopWatch = new Stopwatch();
					stopWatch.Start();
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
						await fileStream.WriteAsync(buffer, 0, bytesRead);

						if (DateTime.Now > lastUpdate + TimeSpan.FromMilliseconds(MeasureSpan))
						{
							_sentData.Add(receivedBytes - lastReceivedBytes);
							speed = _sentData.GetAverage() / (int)stopWatch.Elapsed.TotalMilliseconds;
							stopWatch.Restart();
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
			var client = new HttpClient(new NativeMessageHandler());
			using (var netStream = await client.GetStreamAsync(url))
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