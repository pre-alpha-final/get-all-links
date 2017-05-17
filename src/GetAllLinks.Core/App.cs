using GetAllLinks.Core.Infrastructure.Services;
using GetAllLinks.Core.Infrastructure.Services.Implementations;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;

namespace GetAllLinks.Core
{
	public class App : MvxApplication
	{
		public App()
		{
		}

		public override void Initialize()
		{
			Mvx.LazyConstructAndRegisterSingleton<IDownloadManager, DownloadManager>();

			base.Initialize();
		}
	}
}
