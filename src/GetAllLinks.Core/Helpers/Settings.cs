using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace GetAllLinks.Core.Helpers
{
	/// <summary>
	/// This is the Settings static class that can be used in your Core solution or in any
	/// of your client applications. All settings are laid out the same exact way with getters
	/// and setters. 
	/// </summary>
	public static class Settings
	{
		private static ISettings AppSettings => CrossSettings.Current;

		public static string ListUrl
		{
			get { return AppSettings.GetValueOrDefault("ListUrl", string.Empty); }
			set { AppSettings.AddOrUpdateValue("ListUrl", value); }
		}

		public static int SimultaneousDownloads
		{
			get { return AppSettings.GetValueOrDefault("SimultaneousDownloads", 1); }
			set { AppSettings.AddOrUpdateValue("SimultaneousDownloads", value); }
		}

		public static string DestinationDirectory
		{
			get { return AppSettings.GetValueOrDefault("DestinationDirectory", string.Empty); }
			set { AppSettings.AddOrUpdateValue("DestinationDirectory", value); }
		}
	}
}