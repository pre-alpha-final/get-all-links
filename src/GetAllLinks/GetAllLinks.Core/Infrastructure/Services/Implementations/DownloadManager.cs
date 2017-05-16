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
				new DownloadItemPO { Url = "https://r1---sn-f5f7ln7e.googlevideo.com/videoplayback?sparams=clen%2Cdur%2Cei%2Cgir%2Cid%2Cinitcwndbps%2Cip%2Cipbits%2Citag%2Ckeepalive%2Clmt%2Cmime%2Cmm%2Cmn%2Cms%2Cmv%2Cpl%2Crequiressl%2Csource%2Cupn%2Cexpire&clen=135763898&source=youtube&upn=eF6_1sw0bEc&keepalive=yes&itag=133&key=yt6&lmt=1476097092991541&mv=m&mime=video%2Fmp4&id=o-AKPbSPvydkt6Qs4f_rL5oSt5QXt8GzbFXsrGBgH0W9VS&pl=19&mm=31&mn=sn-f5f7ln7e&ipbits=0&ip=217.153.215.58&requiressl=yes&ms=au&mt=1494943585&gir=yes&dur=4419.147&ei=vwcbWeDBLciKd72GqYgJ&initcwndbps=1561250&expire=1494965279&signature=D72DB58DCB82A49B8E360636C7EBEDD3C72B08B4.A3291AD679BC52D72E8832B50FC062EFF3605658&ratebypass=yes&cmbypass=yes" },
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
