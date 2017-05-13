using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Util;
using Android.Views;
using MvvmCross.Platform;
using MvvmCross.Platform.Core;
using MvvmCross.Platform.Platform;
using System.Threading;
using System.Threading.Tasks;

namespace GetAllLinks.Droid.Controls
{
	public class FillWidthUrlImageButton : FillWidthButtonBase, View.IOnTouchListener
	{
		private ManualResetEvent _standardUrlAvailable;

		private string _standardUrl;
		public string StandardUrl
		{
			get { return _standardUrl; }
			set
			{
				_standardUrl = value;
				_standardUrlAvailable.Set();
			}
		}

		public string ClickedUrl { get; set; }

		public FillWidthUrlImageButton(Context context, IAttributeSet attrs) : base(context, attrs)
		{
			SetOnTouchListener(this);

			_standardUrlAvailable = new ManualResetEvent(false);
		}

		private string ImageUrl
		{
			get { return ImageHelper?.ImageUrl; }
			set
			{
				if (ImageHelper == null)
					return;
				ImageHelper.ImageUrl = value;
			}
		}

		private IMvxImageHelper<Bitmap> _imageHelper;
		private IMvxImageHelper<Bitmap> ImageHelper
		{
			get
			{
				if (_imageHelper == null)
				{
					if (!Mvx.TryResolve(out _imageHelper))
						MvxTrace.Error(
							"No IMvxImageHelper registered - you must provide an image helper before you can use a MvxImageView");
					else
						_imageHelper.ImageChanged += ImageHelperOnImageChanged;
				}
				return _imageHelper;
			}
		}

		protected override async void OnFinishInflate()
		{
			base.OnFinishInflate();

			Gravity = GravityFlags.Center;
			await Task.Run(() => _standardUrlAvailable.WaitOne(5000));
			UpdateButtonDrawable(false);
		}

		private void UpdateButtonDrawable(bool clicked)
		{
			ImageUrl = clicked ? ClickedUrl : StandardUrl;
		}

		private void ImageHelperOnImageChanged(object sender, MvxValueEventArgs<Bitmap> mvxValueEventArgs)
		{
			using (var h = new Handler(Looper.MainLooper))
				h.Post(() => Background = new BitmapDrawable(Resources, mvxValueEventArgs.Value));
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_imageHelper != null)
				{
					_imageHelper.ImageChanged -= ImageHelperOnImageChanged;
					_imageHelper.Dispose();
					if (Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
						SetOnTouchListener(null);
				}
			}

			base.Dispose(disposing);
		}

		private bool _clicked;
		public bool OnTouch(View v, MotionEvent e)
		{
			Task.Run(async () =>
			{
				switch (e.Action)
				{
					case MotionEventActions.Down:
						_clicked = true;
						break;
					case MotionEventActions.Cancel:
						_clicked = false;
						break;
					case MotionEventActions.Up:
						if (_clicked)
						{
							await Task.Delay(250);
							_clicked = false;
						}
						break;
				}
				UpdateButtonDrawable(_clicked);
			});
			return false;
		}
	}
}