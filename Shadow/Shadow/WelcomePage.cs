using System;

using Xamarin.Forms;
using Shadow.Lang;
using Shadow.Model;
using Microsoft.WindowsAzure.MobileServices;
using Acr.UserDialogs;
using Newtonsoft.Json;

namespace Shadow
{
	public class WelcomePage : BaseContentPage
	{
		ExtendedEntry _txtEmail;
		ExtendedEntry _txtPassword;
		Button _btnLogin;
		Button _btnCreateAccount;
		Image _btnCart;

		public WelcomePage ()
		{
			_txtEmail = new ExtendedEntry {
				HasBorder = false,
				TextColor = UIConst.DefaultText,
				PlaceholderTextColor = UIConst.DefaultText,
				Placeholder = AppResources.phWelcome_EnterEmailAddress,
				BackgroundColor = UIConst.BgColor
			};

			_txtPassword = new ExtendedEntry {
				HasBorder = true,
				TextColor = UIConst.DefaultText,
				PlaceholderTextColor = UIConst.DefaultText,
				Placeholder = AppResources.phWelcome_CreatePassword,
				BackgroundColor = UIConst.BgColor,
				IsPassword = true
			};

			_txtEmail.Text = "erhard@ernic.co.za";
			_txtPassword.Text = "Passw0rd123";

			if (Shadow.IO.LocalStorage.FileExists("root", Shadow.Data.Const.ACCOUNT).Result)
			{
				string json = Shadow.IO.LocalStorage.ReadTextFile("root", Shadow.Data.Const.ACCOUNT).Result;
				Shadow.Data.Runtime.RemLogin = JsonConvert.DeserializeObject<Shadow.Data.RemLogin>(json);
				if (Shadow.Data.Runtime.RemLogin != null)
				{
					_txtEmail.Text = Shadow.Data.Runtime.RemLogin.Email;
					_txtPassword.Text = Shadow.Data.Runtime.RemLogin.Password;
				}
			}

			_btnLogin = new Button
			{
				BackgroundColor = UIConst.OrangeColor,
				TextColor = Xamarin.Forms.Color.White,
				BorderRadius = UIConst.DefaultButtonBorderRadius,
				HeightRequest = UIConst.DefaultButtonHeight,
				Text = "Login"
			};
			_btnLogin.Clicked += (sender, e) => { LoginClick(); };

			_btnCreateAccount = new Button {
				BackgroundColor = UIConst.OrangeColor,
				TextColor = Xamarin.Forms.Color.White,
				BorderRadius = UIConst.DefaultButtonBorderRadius,
				HeightRequest = UIConst.DefaultButtonHeight,
				Text = AppResources.btnWelcome_CreateAccount
			};
			_btnCreateAccount.Clicked += (sender, e) => { CreateAccountClick(); };

		
			_btnCart = new Image
			{
				Source = "cart.png",
				WidthRequest = 26,
				HeightRequest = 26,			
			};
			var tapCart = new TapGestureRecognizer();
			tapCart.Tapped += (s, e) =>
			{
				CartClick();
			};
			_btnCart.GestureRecognizers.Add(tapCart);

			Button btnLog = new Button
			{
				Text = "View Log (Temp)"
			};

			btnLog.Clicked += (sender, e) =>
			{
				Navigation.PushModalAsync(new ViewLog());
			};

			ExtendedLabel lblTCs = new ExtendedLabel
			{
				TextColor = UIConst.DefaultText,
				Text = AppResources.lblWelcome_TCAgree,
				FontSize = UIConst.DefaultFontSize,
				HorizontalOptions = LayoutOptions.Center,
				IsUnderline = true
			};
			var tapTCs = new TapGestureRecognizer();
			tapTCs.Tapped += (s, e) =>
			{
				Device.OpenUri(new Uri("http://www.safewithshadow.com"));
			};
			lblTCs.GestureRecognizers.Add(tapTCs);

			Label lblNoShadow = new Label
			{
				VerticalOptions = LayoutOptions.Center,
				TextColor = UIConst.DefaultText,
				FontSize = UIConst.DefaultFontSize,
				Text = AppResources.lblWelcome_NoShadowDevice + " " 
			};

			ExtendedLabel lblBuyNow = new ExtendedLabel
			{
				VerticalOptions = LayoutOptions.Center,
				TextColor = UIConst.DefaultText,
				FontSize = UIConst.DefaultFontSize,
				Text = AppResources.lblWelcome_BuyNow,
				IsUnderline = true
			};
			var tapBuyNow = new TapGestureRecognizer();
			tapBuyNow.Tapped += (s, e) =>
			{
				Device.OpenUri(new Uri("http://www.safewithshadow.com"));
			};
			lblBuyNow.GestureRecognizers.Add(tapBuyNow);


			Content = new StackLayout { 
				Padding = 20,
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.Center,
				Children = {
					new Image {
						HeightRequest = 130,
						Source = "logo.png"
					},
					new Label {
						HorizontalOptions = LayoutOptions.Center,
						TextColor = UIConst.DefaultText,
						Text = AppResources.lblWelcome_Tagline,
						FontSize = UIConst.DefaultFontSize,
						FontAttributes = FontAttributes.Bold
					},
					new StackLayout {
						HeightRequest = 10
					},
					_txtEmail,
					_txtPassword,
					new StackLayout { HeightRequest = 10 },
					lblTCs,
					new StackLayout { HeightRequest = 10 },
					_btnLogin,
					new StackLayout { HeightRequest = 5 },
					_btnCreateAccount,
					new StackLayout { HeightRequest = 10 },
					/*new StackLayout { HeightRequest = 10 },
					new Label { 
						TextColor = UIConst.DefaultText,
						Text = AppResources.lblWelcome_SignIn,
						FontSize = UIConst.DefaultFontSize,
						HorizontalOptions = LayoutOptions.Center,
					},*/
					new StackLayout { HeightRequest = 10 },
					new StackLayout { 
						Orientation = StackOrientation.Horizontal,
						HorizontalOptions = LayoutOptions.Center,
						Children = {
							_btnCart,
							new StackLayout {
								WidthRequest = 5
							},
							lblNoShadow,
							lblBuyNow,
						}
					},
					new StackLayout { HeightRequest = 5 },
					new Label {
						Text = "v.1.0.11",
						TextColor = UIConst.DefaultText
					},
					//btnLog
				}
			};
		}

		private async void LoginClick()
		{
			if ((string.IsNullOrEmpty(_txtEmail.Text)) || (string.IsNullOrEmpty(_txtPassword.Text)))
				await DisplayAlert("Error", "Please enter your email address or password.", "OK");
			else
			{
				try
				{
					Account shadowUser = null;

					using (var dlg = UserDialogs.Instance.Loading(Shadow.Lang.AppResources.lblAuthenticating, null, "", true))
					{
						shadowUser = await ShadowService.AuthenticateUser(_txtEmail.Text, _txtPassword.Text);
					};

					if (shadowUser == null)
						await DisplayAlert("Login Error", "Invalid email address or password.", "OK");
					else
					{
						if (string.IsNullOrEmpty(shadowUser.firstName))
						{
							await Navigation.PushModalAsync(new SignupPageWizard());

							return;
						}
						else if (Shadow.Data.Runtime.Contacts.Count == 0)
						{
							foreach (Contact contact in shadowUser.ContactList)
							{
								Shadow.Data.Contact shadowcontact = new Data.Contact();
								shadowcontact.Name = contact.firstName + " " + contact.lastName;
								shadowcontact.Mobile = contact.phoneNo;
								Shadow.Data.Runtime.Contacts.Add(shadowcontact);
							}
							var json = JsonConvert.SerializeObject(Shadow.Data.Runtime.Contacts);
							bool result = Shadow.IO.LocalStorage.WriteTextFile("root", Shadow.Data.Const.CONTACTS, json).Result;
						}

						ShadowService.SaveLoggedinUser();
						if (Shadow.Data.Runtime.RemLogin == null)
							Shadow.Data.Runtime.RemLogin = new Shadow.Data.RemLogin();
						Shadow.Data.Runtime.RemLogin.Email = _txtEmail.Text;
						Shadow.Data.Runtime.RemLogin.Password = _txtPassword.Text;
						//Shadow.Data.Runtime.RemLogin.AuthToken = shadowUser.salt;
						Shadow.Data.Runtime.SaveRemLogin();

						await Navigation.PushModalAsync(new MainPage());
					}
				}
				catch (MobileServiceInvalidOperationException ex)
				{
					await DisplayAlert("Error", ex.Message, "OK");
				}
				catch (Exception ex)
				{
					await DisplayAlert("Error", ex.Message, "OK");
				}
			}
		}

		private void CartClick()
		{
			Device.OpenUri(new Uri("http://www.google.com"));
		}

		private async void CreateAccountClick()
		{
			//await Navigation.PushModalAsync(new SignupPageWizard());
			//return;


			if ((string.IsNullOrEmpty(_txtEmail.Text)) || (string.IsNullOrEmpty(_txtPassword.Text)))
				await DisplayAlert("Error", "Please enter your email address or password.", "OK");
			else
			{
				try
				{
					Account shadowUser = null;

					using (var dlg = UserDialogs.Instance.Loading(Shadow.Lang.AppResources.lblValidating, null, "", true))
					{
						shadowUser = await ShadowService.RegisterAccount(_txtEmail.Text, _txtPassword.Text);
					};

					if (shadowUser != null)
					{
						// Remove previous contact files and paired devices
						bool deleted = Shadow.IO.LocalStorage.DeleteFile("root", Shadow.Data.Const.CONTACTS).Result;
						deleted = Shadow.IO.LocalStorage.DeleteFile("root", Shadow.Data.Const.PAIREDDEVICES).Result;

						await Navigation.PushModalAsync(new SignupPageWizard());
					}
				}
				catch (MobileServiceInvalidOperationException ex)
				{
					await DisplayAlert("Error", ex.Message, "OK");
				}
				catch (Exception ex)
				{
					await DisplayAlert("Error", ex.Message, "OK");
				}
			}
		}
	}
}


