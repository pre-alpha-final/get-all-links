using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chance.MvvmCross.Plugins.UserInteraction;
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

		public int CurrentItemCount { get; set; }

		public DownloadManager(IDownloader downloader)
		{
			_downloader = downloader;
			_downloadableItems = new List<DownloadItemPO>();
		}

		public async Task<List<DownloadItemPO>> GetDownloadItems()
		{
			if (_downloadableItems.Count > 0)
				return _downloadableItems;
				
			try
			{
				var html = await _downloader.DownloadList(Settings.ListUrl);
				var htmlLinks = LinkFinderHelper.FindLinks(html).Where(e => e.Href != null).ToList();

				_downloadableItems = htmlLinks.Select(e => new DownloadItemPO
				{
					Name = e.Text,
					Url = e.Href,
				}).ToList();
				CurrentItemCount = _downloadableItems.Count;
			}
			catch (Exception e)
			{
				await Mvx.Resolve<IUserInteraction>().AlertAsync(e.Message);
			}
			return _downloadableItems;
		}

		public Task DownloadAll()
		{
			_index = 0;
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
						catch (Exception e)
						{
							downloadable.UpdateProgress(0, 0, "error: download error");
						}
						finally
						{
							downloadable.InProgress = false;
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
				while (true)
				{
					if (_index >= _downloadableItems.Count)
						return null;

					var item = _downloadableItems[_index++];
					if (item.InProgress)
						continue;
					item.InProgress = true;

					return item;
				}
			}
		}
	}
}
