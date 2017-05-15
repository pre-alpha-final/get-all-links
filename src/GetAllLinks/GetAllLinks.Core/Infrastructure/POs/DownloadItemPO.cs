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

		private double _completion;
		public double Completion
		{
			get { return _completion; }
			set
			{
				_completion = value;
				RaiseAllPropertiesChanged();
			}
		}
		public void UpdateProgress(double completion)
		{
			Completion = completion;
		}
	}
}
