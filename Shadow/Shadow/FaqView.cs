using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Newtonsoft.Json;

using Shadow.Lang;

namespace Shadow
{
	public class FaqView : ContentView
	{

		public FaqView ()
		{

			Button btnLog = new Button
			{
				Text = "View Log (Temp)"
			};

			btnLog.Clicked += (sender, e) =>
			{
				Navigation.PushModalAsync(new ViewLog());
			};
		
			Content = new StackLayout
			{
				Padding = 0,
				Spacing = 0,
				Orientation = StackOrientation.Vertical,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Children = {
					new StackLayout {
						Padding = 0,
						Spacing = 0,
						VerticalOptions = LayoutOptions.Start,
						Children = {
							new StackLayout { HeightRequest = 10 },
							new StackLayout {
								Padding = 10,
								Children = {
									new StackLayout {
										Orientation = StackOrientation.Horizontal,
										Children = {
											new Image {
												WidthRequest = 20,
												HeightRequest = 20,
												Source = ImageSource.FromFile("pulllist.png")
											},
											new Label {
												VerticalOptions = LayoutOptions.End,
												Text = "Frequently Asked Questions",
												FontSize = UIConst.DefaultFontSize+2,
												TextColor = Xamarin.Forms.Color.White
											}
										}
									},


								}
							}
						}
					},
					new StackLayout {
						Padding = 20,
						VerticalOptions = LayoutOptions.FillAndExpand,
						Children = {
							btnLog
						}
					}
				}

			};

		}
			

	}
}


