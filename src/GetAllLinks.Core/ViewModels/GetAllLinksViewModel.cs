using System.Collections.Generic;
using GetAllLinks.Core.Infrastructure.POs;
using MvvmCross.Core.ViewModels;
using GetAllLinks.Core.Infrastructure.Services;

namespace GetAllLinks.Core.ViewModels
{
	public class GetAllLinksViewModel : MvxViewModel
	{
		private readonly IDownloadManager _downloadManager;

		private List<DownloadItemPO> _downloadableItems;
		public List<DownloadItemPO> DownloadableItems
		{
			get { return _downloadableItems; }
			set
			{
				_downloadableItems = value;
				RaisePropertyChanged();
			}
		}

		public GetAllLinksViewModel(IDownloadManager downloadManager)
		{
			_downloadManager = downloadManager;
			SettingsCommand = new MvxCommand(SettingsAction);
			GoCommand = new MvxCommand(GoAction);
		}

		public IMvxCommand SettingsCommand { get; }
		private void SettingsAction()
		{
			ShowViewModel<SettingsViewModel>();
		}

		public IMvxCommand GoCommand { get; }
		private async void GoAction()
		{
			DownloadableItems = await _downloadManager.GetDownloadItems();
			await _downloadManager.DownloadAll();
		}
	}
}
