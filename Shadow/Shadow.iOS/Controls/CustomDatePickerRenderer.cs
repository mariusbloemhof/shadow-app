using System;
using System.ComponentModel;
using System.Threading.Tasks;
using CoreGraphics;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Shadow;

namespace Shadow
{
	public class CustomDatePickerRenderer : DatePickerRenderer
	{
		/// <summary>
		/// Wird gefeuert wenn das Element sich ändert
		/// </summary>
		/// <param name="e">Event Argumente</param>
		protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
		{
			base.OnElementChanged(e);

			if (this.Control == null) return;

			var customDatePicker = e.NewElement as CustomDatePicker;

			if (customDatePicker != null)
			{
				this.SetValue(customDatePicker);
			}
		}
		/// <summary>
		/// Wird gefeuert wenn sich eine Property des Elements ändert
		/// </summary>
		/// <param name="sender">Der Sender</param>
		/// <param name="e">Event Argumente</param>
		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (this.Control == null) return;

			var customDatePicker = this.Element as CustomDatePicker;

			if (customDatePicker != null)
			{
				switch (e.PropertyName)
				{
					case CustomDatePicker.NullableDatePropertyName:
						this.SetValue(customDatePicker);
						break;
					case CustomDatePicker.NullTextPropertyName:
						this.SetValue(customDatePicker);
						break;
				}
			}
		}
		/// <summary>
		/// Setzt den Datumswert oder den NullText
		/// </summary>
		/// <param name="customDatePicker">Das PCL Control</param>
		private void SetValue(CustomDatePicker customDatePicker)
		{
			if (customDatePicker.NullableDate.HasValue)
			{
				this.Control.Text = customDatePicker.NullableDate.Value.ToString(customDatePicker.Format);
			}
			else
			{
				this.Control.Text = customDatePicker.NullText ?? string.Empty;
			}
		}
	}
}
