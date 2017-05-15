using System.Collections.Generic;
using MvvmCross.Core.ViewModels;
using GetAllLinks.Core.Infrastructure.Services;

namespace GetAllLinks.Core.ViewModels
{
	public class GetAllLinksViewModel : MvxViewModel
	{
		private readonly IDownloadManager _downloadManager;

		public GetAllLinksViewModel(IDownloadManager downloadManager)
		{
			_downloadManager = downloadManager;
			SettingsCommand = new MvxCommand(SettingsAction);
		}

		public IMvxCommand SettingsCommand { get; }
		private async void SettingsAction()
		{
			var downloadableItems = await _downloadManager.GetDownloadItems();
			await _downloadManager.DownloadAll();
		}
	}
}
