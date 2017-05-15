using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GetAllLinks.Core.Infrastructure.POs;
using MvvmCross.Platform;

namespace GetAllLinks.Core.Infrastructure.Services.Implementations
{
	public class DownloadManager : IDownloadManager
	{
		private List<DownloadItemPO> _downloadableItems;
		private static readonly object _itemAcquisitionLock = new object();
		private int _index;

		public Task<List<DownloadItemPO>> GetDownloadItems()
		{
			_downloadableItems = new List<DownloadItemPO>
			{
				new DownloadItemPO { Url = "http://www.clker.com/cliparts/w/o/d/I/G/A/smily-hi.png" },
				new DownloadItemPO { Url = "https://s-media-cache-ak0.pinimg.com/736x/f6/95/ac/f695acb40886b0a8bc92509b45fd21bc.jpg" },
				new DownloadItemPO { Url = "http://www.clipartbest.com/cliparts/dc7/6eR/dc76eRdoi.png" },

			};
			return Task.FromResult(_downloadableItems);
		}

		public Task DownloadAll()
		{
			for (int i = 0; i < 2; i++)
			{
				Task.Run(() =>
				{
					while (true)
					{
						var downloadable = GetNextDownloadable();
						if (downloadable == null)
							return;
						Mvx.Resolve<IDownloader>().Download(downloadable);
					}
				});
			}
			return Task.FromResult(0);
		}

		private IDownloadable GetNextDownloadable()
		{
			lock (_itemAcquisitionLock)
			{
				if (_index >= _downloadableItems.Count)
					return null;

				return _downloadableItems[_index++];
			}
		}
	}
}
