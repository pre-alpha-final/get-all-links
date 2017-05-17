using MvvmCross.Core.ViewModels;

namespace GetAllLinks.Core.ViewModels
{
	public class MainActivityViewModel : MvxViewModel
	{
		public override void Start()
		{
			ShowViewModel<GetAllLinksViewModel>();
		}
	}
}
