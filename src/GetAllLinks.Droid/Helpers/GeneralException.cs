using System;
using System.Threading.Tasks;
using Chance.MvvmCross.Plugins.UserInteraction;
using MvvmCross.Platform;

namespace GetAllLinks.Droid.Helpers
{
	public static class GeneralException
	{
		public static async Task HandleGeneralException(object sender, UnhandledExceptionEventArgs e)
		{
			await Show(e.ToString());
		}

		public static async Task HandleGeneralException(object sender, UnobservedTaskExceptionEventArgs e)
		{
			await Show(e.ToString());
		}

		private static async Task Show(string exception)
		{
			await Mvx.Resolve<IUserInteraction>().AlertAsync(exception, "General exception");
		}
	}
}
