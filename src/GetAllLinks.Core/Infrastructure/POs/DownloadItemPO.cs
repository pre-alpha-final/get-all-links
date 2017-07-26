using MvvmCross.Core.ViewModels;

namespace GetAllLinks.Core.Infrastructure.POs
{
	public class DownloadItemPO : MvxNotifyPropertyChanged, IDownloadable
	{
		private string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				RaisePropertyChanged();
			}
		}

		private bool _inProgress;
		public bool InProgress
		{
			get { return _inProgress; }
			set
			{
				_inProgress = value;
				RaisePropertyChanged();
			}
		}

		private string _url;
		public string Url
		{
			get { return _url; }
			set
			{
				_url = value;
				RaiseAllPropertiesChanged();
			}
		}

		private string _downloadSpeed;
		public string DownloadSpeed
		{
			get { return _downloadSpeed; }
			set
			{
				_downloadSpeed = value;
				RaisePropertyChanged();
			}
		}

		private string _completion;
		public string Completion
		{
			get { return _completion; }
			set
			{
				_completion = value;
				RaisePropertyChanged();
			}
		}

		public void UpdateProgress(double completion, int speed, string status = "")
		{
			Completion = $"{100 * completion:0.00} %";
			var speedKbytesSecond = (double)speed * 1000 / 1024;
			var speedMbytesSecond = speedKbytesSecond / 1024;
			DownloadSpeed = $"{speedKbytesSecond:0.00} Kb/s";
			if (speedMbytesSecond > 1)
				DownloadSpeed = $"{speedMbytesSecond:0.00} Mb/s";
			if (string.IsNullOrWhiteSpace(status) == false)
				Completion = status;
		}
	}
}
