using GetAllLinks.Core.Infrastructure.POs;
using System.Threading.Tasks;

namespace GetAllLinks.Core.Infrastructure.Services
{
	public interface IDownloader
	{
		Task Download(IDownloadable downloadable);
		Task<string> DownloadList(string url);
		Task<string> GetDefaultDownloadDir();
	}
}
