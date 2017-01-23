using System;

using Xamarin.Forms;

namespace Shadow
{
	public class ViewLog : ContentPage
	{
		public ViewLog ()
		{
			WebView webView = new WebView {
				HorizontalOptions = LayoutOptions.Fill,
				VerticalOptions = LayoutOptions.FillAndExpand,
				HeightRequest = 400
			};

			string log = Shadow.IO.LocalStorage.ReadTextFile ("root", "log1.txt").Result;
			HtmlWebViewSource html = new HtmlWebViewSource ();
			html.Html = log;

			webView.Source = html;

			Button btnClose = new Button {
				Text = "Close"
			};
			btnClose.Clicked += (object sender, EventArgs e) => {
				Navigation.PopModalAsync();
			};

			Content = new StackLayout { 
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.Fill,
				VerticalOptions = LayoutOptions.Fill,
				Children = {
					new StackLayout {
						HeightRequest = 30,
					},
					btnClose,
					webView
				}
			};
		}
	}
}


