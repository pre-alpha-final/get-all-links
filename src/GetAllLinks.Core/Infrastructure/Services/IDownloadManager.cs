using GetAllLinks.Core.Infrastructure.POs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetAllLinks.Core.Infrastructure.Services
{
	public interface IDownloadManager
	{
		int CurrentItemCount { get; set; }

		Task<List<DownloadItemPO>> GetDownloadItems();
		Task DownloadAll();
	}
}
