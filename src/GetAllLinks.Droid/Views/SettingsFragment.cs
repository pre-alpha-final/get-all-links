using Android.OS;
using GetAllLinks.Core.ViewModels;
using MvvmCross.Droid.Shared.Attributes;
using Android.Views;

namespace GetAllLinks.Droid.Views
{
	[MvxFragment(typeof(MainActivityViewModel), Resource.Layout.settingsView, ViewModelType = typeof(SettingsViewModel), IsCacheableFragment = false)]
	public class SettingsFragment : BaseFragment<SettingsViewModel>
	{
		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = base.OnCreateView(inflater, container, savedInstanceState);

			return view;
		}
	}
}