using System;
using System.Threading.Tasks;
using GetAllLinks.Core.Infrastructure.POs;
using GetAllLinks.Core.Infrastructure.Services;

namespace GetAllLinks.Droid.Services.Implementations
{
	class Downloader : IDownloader
	{
		public Task Download(IDownloadable downloadable)
		{
			return Task.FromResult(0);
		}
	}
}