using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GetAllLinks.Core.Helpers;
using GetAllLinks.Core.Infrastructure.POs;
using MvvmCross.Platform;

namespace GetAllLinks.Core.Infrastructure.Services.Implementations
{
	public class DownloadManager : IDownloadManager
	{
		private readonly IDownloader _downloader;
		private List<DownloadItemPO> _downloadableItems;
		private static readonly object ItemAcquisitionLock = new object();
		private int _index;

		public DownloadManager(IDownloader downloader)
		{
			_downloader = downloader;
		}

		public async Task<List<DownloadItemPO>> GetDownloadItems()
		{
			var html = await _downloader.DownloadList(Settings.ListUrl);
			var htmlLinks = LinkFinderHelper.FindLinks(html).Where(e => e.Href != null).ToList();

			_downloadableItems = htmlLinks.Select(e => new DownloadItemPO
			{
				Name = e.Text,
				Url = e.Href,
			}).ToList();
			return _downloadableItems;
		}

		public Task DownloadAll()
		{
			for (int i = 0; i < Settings.SimultaneousDownloads; i++)
			{
				Task.Run(async () =>
				{
					while (true)
					{
						var downloadable = GetNextDownloadable();
						if (downloadable == null)
							return;
						try
						{
							await Mvx.Resolve<IDownloader>().Download(downloadable);
						}
						catch
						{
							// ignored
							downloadable.UpdateProgress(0, 0, "error: download error");
						}
					}
				});
			}
			return Task.FromResult(0);
		}

		private IDownloadable GetNextDownloadable()
		{
			lock (ItemAcquisitionLock)
			{
				if (_index >= _downloadableItems.Count)
					return null;

				return _downloadableItems[_index++];
			}
		}
	}
}
