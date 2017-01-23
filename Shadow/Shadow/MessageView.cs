using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Newtonsoft.Json;

using Shadow.Lang;
using Plugin.Geolocator;
using Acr.UserDialogs;

namespace Shadow
{
	public class MessageView : ContentView
	{
		public Editor _txtMessageLine1;
		Button _btnPreview;
		Button _btnDefault;
		public bool IsSignupWizard = false;

		public MessageView ()
		{

			_btnDefault = new Button
			{
				Text = AppResources.btnEmergency_Default,
				TextColor = UIConst.DefaultText,
				BackgroundColor = UIConst.OrangeColor,
				HorizontalOptions = LayoutOptions.FillAndExpand
			};
			_btnDefault.Clicked += (sender, e) =>
			{
				_txtMessageLine1.Text = Shadow.Lang.AppResources.DefaultMessage1;
				//_txtMessageLine2.Text = Shadow.Lang.AppResources.DefaultMessage2;
				Shadow.Data.Runtime.RemLogin.Message1 = _txtMessageLine1.Text;
				//Shadow.Data.Runtime.RemLogin.Message2 = _txtMessageLine2.Text;
				Shadow.Data.Runtime.SaveRemLogin();
			};

			_btnPreview = new Button
			{
				Text = AppResources.btnEmergency_Preview,
				TextColor = UIConst.DefaultText,
				BackgroundColor = UIConst.OrangeColor,
				HorizontalOptions = LayoutOptions.FillAndExpand
			};

			_btnPreview.Clicked += Preview;

			_txtMessageLine1 = new Editor
			{
				TextColor = UIConst.DefaultText,
				Text = Shadow.Data.Runtime.MessageLine1,
				BackgroundColor = Xamarin.Forms.Color.Transparent,
				HeightRequest = 100
			};

			_txtMessageLine1.TextChanged += (sender, e) =>
			{
				if (_txtMessageLine1.Text.Length > 140)
					_txtMessageLine1.Text = e.OldTextValue;
			};

			_txtMessageLine1.Unfocused += (sender, e) =>
			{
				//Shadow.Data.Runtime.RemLogin.Message1 = _txtMessageLine1.Text;
				Shadow.Data.Runtime.SaveRemLogin();
			};

			_txtMessageLine1.Text = Shadow.Lang.AppResources.DefaultMessage1;

			if (Shadow.Data.Runtime.RemLogin != null)
			{
				if (!string.IsNullOrEmpty(Shadow.Data.Runtime.RemLogin.Message1))
					_txtMessageLine1.Text = Shadow.Data.Runtime.RemLogin.Message1;
			}

			Content = new ScrollView {
				Content = 
				new StackLayout
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
									new Label {
										Text = AppResources.lblEmergency_ChooseMessage,
										FontSize = UIConst.DefaultFontSize+2,
										TextColor = Xamarin.Forms.Color.White
									},
									new StackLayout { HeightRequest = 5 },
									new Label {
											Text = AppResources.lblEmergency_MessageTitle,
											FontSize = UIConst.DefaultFontSize+2,
											TextColor = Xamarin.Forms.Color.White
										},
									new StackLayout {
										Padding = 10,
										Orientation = StackOrientation.Vertical,
										BackgroundColor = UIConst.OrangeColor,
										Opacity = 0.7,
										Children = {
											_txtMessageLine1,
										}
									},
									new StackLayout {
										Orientation = StackOrientation.Horizontal,
										HorizontalOptions = LayoutOptions.FillAndExpand,
										Children = {
											_btnPreview,
											new StackLayout { WidthRequest = 2 },
											_btnDefault
										}
									}
								}
							},
						}
					}

					}
				}

			};


		}

		private void Preview(object sender, EventArgs a)
		{
			string msg = _txtMessageLine1.Text;

			GetLocation(msg);
		}

		private  async void GetLocation(string msg)
		{
			var locator = CrossGeolocator.Current;
			locator.DesiredAccuracy = 50;
			string url = "";

			using (var dlg = UserDialogs.Instance.Loading(Shadow.Lang.AppResources.lblGettingLocation, null, "", true))
			{
				var position = await locator.GetPositionAsync(10000);
				url = "http://maps.google.com/maps?q=" + position.Latitude.ToString() + "," + position.Longitude.ToString();
			};


			if (IsSignupWizard)
				SignupPageWizard.DisplayMsgPreview(msg + " " + url);
			else
				MainPage.DisplayMsgPreview(msg + " " + url);
		}
	
	}
}


