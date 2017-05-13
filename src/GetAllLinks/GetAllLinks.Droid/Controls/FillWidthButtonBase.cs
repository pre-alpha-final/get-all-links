using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;

namespace GetAllLinks.Droid.Controls
{
	public class FillWidthButtonBase : AppCompatButton
	{
		private bool _hasDrawable;
		private int _width;
		private double _height;

		public FillWidthButtonBase(Context context, IAttributeSet attrs) : base(context, attrs)
		{
		}

		protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
		{
			if (_hasDrawable && _width == MeasureSpec.GetSize(widthMeasureSpec))
			{
				base.OnMeasure(MeasureSpec.MakeMeasureSpec(_width, MeasureSpecMode.Exactly),
					MeasureSpec.MakeMeasureSpec((int)_height, MeasureSpecMode.Exactly));
				return;
			}

			_width = MeasureSpec.GetSize(widthMeasureSpec);
			_height = MeasureSpec.GetSize(widthMeasureSpec);

			var drawable = Background as BitmapDrawable;
			if (drawable?.Bitmap != null && _width > 0)
			{
				_hasDrawable = true;
				var ratio = (double)drawable.Bitmap.Width / _width;
				_height = drawable.Bitmap.Height / ratio;
			}
			else // Recycler resize fix
			{
				_height = 1;
			}

			base.OnMeasure(MeasureSpec.MakeMeasureSpec(_width, MeasureSpecMode.Exactly),
				MeasureSpec.MakeMeasureSpec((int)_height, MeasureSpecMode.Exactly));
		}
	}
}