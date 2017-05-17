using System;
using Android.App;
using Android.Content;
using GetAllLinks.Core;
using MvvmCross.Core.ViewModels;
using MvvmCross.Droid.Platform;
using MvvmCross.Platform.Plugins;
using MvvmCross.Plugins.DownloadCache.Droid;
using MvvmCross.Droid.Views;
using MvvmCross.Droid.Shared.Presenter;
using MvvmCross.Platform;
using GetAllLinks.Droid.Services.Implementations;
using GetAllLinks.Core.Infrastructure.Services;

namespace GetAllLinks.Droid
{
	public class Setup : MvxAndroidSetup
	{
		public Setup(Context applicationContext)
			: base(applicationContext)
		{
		}

		protected override IMvxApplication CreateApp()
		{
			Mvx.RegisterType<IDownloader, Downloader>();

			return new App();
		}

		protected override void AddPluginsLoaders(MvxLoaderPluginRegistry registry)
		{
			registry.Register<MvvmCross.Plugins.DownloadCache.PluginLoader, MvvmCross.Plugins.DownloadCache.Droid.Plugin>();
			registry.Register<MvvmCross.Plugins.File.PluginLoader, MvvmCross.Plugins.File.Droid.Plugin>();
			base.AddPluginsLoaders(registry);
		}

		protected override IMvxAndroidViewPresenter CreateViewPresenter()
		{
			return new MvxFragmentsPresenter(new[] { typeof(Setup).Assembly });
		}

		protected override void InitializeLastChance()
		{
			base.InitializeLastChance();
			MvvmCross.Plugins.File.PluginLoader.Instance.EnsureLoaded();
			MvvmCross.Plugins.Json.PluginLoader.Instance.EnsureLoaded();
			MvvmCross.Plugins.DownloadCache.PluginLoader.Instance.EnsureLoaded();
		}

		protected override IMvxPluginConfiguration GetPluginConfiguration(Type plugin)
		{
			if (plugin == typeof(MvvmCross.Plugins.DownloadCache.Droid.Plugin))
			{
				var am = ApplicationContext.GetSystemService(Context.ActivityService) as ActivityManager;
				var memoryInfo = new ActivityManager.MemoryInfo();
				if (am == null)
					return base.GetPluginConfiguration(plugin);

				am.GetMemoryInfo(memoryInfo);
				var mem = Convert.ToInt32(memoryInfo.TotalMem / 100);

				return new MvxDownloadCacheConfiguration
				{
					MaxFileAge = TimeSpan.MaxValue,
					MaxFiles = 1000,
					MaxInMemoryFiles = 20,
					MaxInMemoryBytes = mem,
					DisposeOnRemoveFromCache = true,
					MaxConcurrentDownloads = 10,
					CacheFolderPath = MvxDownloadCacheConfiguration.Default.CacheFolderPath,
					CacheName = MvxDownloadCacheConfiguration.Default.CacheName
				};
			}
			return base.GetPluginConfiguration(plugin);
		}
	}
}