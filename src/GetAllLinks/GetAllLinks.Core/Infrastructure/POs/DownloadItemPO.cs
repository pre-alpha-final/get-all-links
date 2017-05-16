using System;
using MvvmCross.Core.ViewModels;

namespace GetAllLinks.Core.Infrastructure.POs
{
	public class DownloadItemPO : MvxNotifyPropertyChanged, IDownloadable
	{
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

		private double _completion;
		public double Completion
		{
			get { return _completion; }
			set
			{
				_completion = value;
				RaisePropertyChanged();
			}
		}

		public void UpdateProgress(double completion, int speed)
		{
			Completion = completion;
			var speedKbytesSecond = (double)speed * 1000 / 1024;
			var speedMbytesSecond = speedKbytesSecond / 1024;
			DownloadSpeed = $"{speedKbytesSecond:0.##} Kb/s";
			if (speedMbytesSecond > 1)
				DownloadSpeed = $"{speedMbytesSecond:0.##} Mb/s";
			if (Completion == 1)
				DownloadSpeed = "Completed";
		}
	}
}
