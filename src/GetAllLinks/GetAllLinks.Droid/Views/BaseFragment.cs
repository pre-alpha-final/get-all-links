using System;
using Android.OS;
using Android.Views;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Core.ViewModels;
using MvvmCross.Droid.Shared.Attributes;
using MvvmCross.Droid.Support.V4;
using System.Collections.Generic;

namespace GetAllLinks.Droid.Views
{
	public abstract class BaseFragment<T> : MvxFragment<T> where T : class, IMvxViewModel
	{
		protected MvxFragmentAttribute Attribute
		{
			get { return (MvxFragmentAttribute)System.Attribute.GetCustomAttribute(GetType(), typeof(MvxFragmentAttribute)); }
		}

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			RetainInstance = true;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			this.EnsureBindingContextIsSet(inflater);
			return this.BindingInflate(Attribute.FragmentContentId, container, false);
		}
	}
}