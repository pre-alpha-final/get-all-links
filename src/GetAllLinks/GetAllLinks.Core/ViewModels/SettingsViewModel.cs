using GetAllLinks.Core.Helpers;
using GetAllLinks.Core.Infrastructure.Interfaces;
using GetAllLinks.Core.Infrastructure.Services;
using MvvmCross.Core.ViewModels;

namespace GetAllLinks.Core.ViewModels
{
	public class SettingsViewModel : MvxViewModel, ICloseable
	{
		private readonly IDownloader _downloader;

		public string ListUrl
		{
			get { return Settings.ListUrl; }
			set
			{
				Settings.ListUrl = value;
				RaisePropertyChanged();
			}
		}

		public int SimultaneousDownloads
		{
			get { return Settings.SimultaneousDownloads; }
			set
			{
				Settings.SimultaneousDownloads = value;
				RaisePropertyChanged();
			}
		}

		public string DestinationDirectory
		{
			get
			{
				if (string.IsNullOrWhiteSpace(Settings.DestinationDirectory))
					Settings.DestinationDirectory = _downloader.GetDefaultDownloadDir().Result;
				return Settings.DestinationDirectory;
			}
			set
			{
				Settings.DestinationDirectory = value;
				RaisePropertyChanged();
			}
		}

		public SettingsViewModel(IDownloader downloader)
		{
			_downloader = downloader;
		}

		public void OnClose()
		{
		}
	}
}
