using System;
using Xamarin.Forms;

namespace Shadow
{
	public static class UIHelper
	{
		public static Size ScreenSize;

		public static StackLayout TitleSpacer() 
		{
			StackLayout titleSpacer = new StackLayout {
				HeightRequest = 20,
				Padding = 0,
				Spacing = 0,
				BackgroundColor = UIConst.OrangeColor
			};
			return titleSpacer;
		}

		public static StackLayout CreateTitleBar(string title)
		{
			StackLayout titleBar = new StackLayout
			{
				Padding = 0,
				Spacing = 0,
				BackgroundColor = UIConst.OrangeColor,
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.Fill,
				Children = {
					new StackLayout { HeightRequest = 10 },
					new Label { 						
						FontAttributes = FontAttributes.Bold,
						HorizontalOptions = LayoutOptions.Center,
						VerticalOptions = LayoutOptions.Center,
						TextColor = Xamarin.Forms.Color.White,
						FontSize = UIConst.HeadingFontSize,
						Text = title
					},
					new StackLayout { HeightRequest = 10 }
				}
			};
			return titleBar;
		}

		public static void SetTitle(StackLayout titleBar, string title)
		{
			((Label)titleBar.Children[1]).Text = title;
		}

		public static StackLayout CreateHomeTitleBar(string title)
		{
			StackLayout titleBar = new StackLayout
			{
				Padding = 0,
				Spacing = 0,
				BackgroundColor = UIConst.OrangeColor,
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.Fill,
				Children = {
					new StackLayout { HeightRequest = 10 },
					new StackLayout { 
						Orientation = StackOrientation.Horizontal,
						HorizontalOptions = LayoutOptions.FillAndExpand,
						Padding = 5,
						Children = {
							new Label {
								FontAttributes = FontAttributes.Bold,
								HorizontalOptions = LayoutOptions.StartAndExpand,
								VerticalOptions = LayoutOptions.Center,
								TextColor = Xamarin.Forms.Color.White,
								FontSize = UIConst.HeadingFontSize + 2,
								Text = title
							},
							new Image {
								HorizontalOptions = LayoutOptions.End,
								Source = "shadowiconwhite",
								WidthRequest = 35
							}
						}
					},
					new StackLayout { HeightRequest = 10 }
				}
			};
			return titleBar;
		}

		public static void SetHomeTitle(StackLayout titleBar, string title)
		{
			StackLayout stack = ((StackLayout)titleBar.Children[1]);
			((Label)stack.Children[0]).Text = title;
		}

	}
}

