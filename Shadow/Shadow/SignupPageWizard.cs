using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Newtonsoft.Json;

using Shadow.Lang;
using Shadow.Model;
using Microsoft.WindowsAzure.MobileServices;
using Acr.UserDialogs;
using Plugin.Messaging;


namespace Shadow
{
	public class SignupPageWizard : BaseContentPage
	{
		Button _btnContinue;
		Button _btnBack;
		Image _btnCurrentLocation;
		StackLayout _subViewContainer;
		StackLayout _stackButtonsContainer;
		StackLayout _titleBar = UIHelper.CreateTitleBar("");
		int _currentStep = 0;

		DeviceSignupView _deviceView;
		ContactsSignupView _contactsView;
		UserDetailView _userDetailView;
		MessageView _messageView;
		LocationView _locationView;

		public SignupPageWizard ()
		{

			_btnBack = new Button {
				HorizontalOptions = LayoutOptions.StartAndExpand,
				VerticalOptions = LayoutOptions.End,
				BorderColor = Xamarin.Forms.Color.White,
				BorderWidth = 2,
				BorderRadius = UIConst.DefaultButtonBorderRadius,
				TextColor = Xamarin.Forms.Color.White,
				Text = AppResources.btnEmergencyMessage_Back,
				WidthRequest = 100
			};
			_btnBack.Clicked += btnBack_Clicked;

			_btnContinue = new Button {
				HorizontalOptions = LayoutOptions.EndAndExpand,
				VerticalOptions = LayoutOptions.End,
				BorderColor = Xamarin.Forms.Color.White,
				BorderWidth = 2,
				BorderRadius = UIConst.DefaultButtonBorderRadius,
				TextColor = Xamarin.Forms.Color.White,
				Text = AppResources.btnEmergencyMessage_Continue,
				WidthRequest = 100
			};
			_btnContinue.Clicked += _btnContinue_Clicked;

			_btnCurrentLocation = new Image
			{
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				Source = "pin.png",
				WidthRequest = 35,
				HeightRequest = 35,
			};
			var imgCurrentLocationTap = new TapGestureRecognizer();
			_btnCurrentLocation.GestureRecognizers.Add(imgCurrentLocationTap);
			imgCurrentLocationTap.Tapped += (sender, e) => { _locationView.GetCurrentLocation(); };

			_subViewContainer = new StackLayout
			{
				VerticalOptions = LayoutOptions.FillAndExpand
			};

			_stackButtonsContainer = new StackLayout
			{
				Padding = 10,
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.End,
				Children = {
					_btnBack,
					_btnCurrentLocation,
					_btnContinue
				}
			};
		
			Content = new StackLayout { 
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
							UIHelper.TitleSpacer(),
							_titleBar,			
						}
					},
					_subViewContainer,
					_stackButtonsContainer,
				}

			};

			UIHelper.SetTitle(_titleBar, AppResources.lblContactsPage_Contacts);

			_deviceView = new DeviceSignupView();
			_contactsView = new ContactsSignupView();
			_userDetailView = new UserDetailView();
			_messageView = new MessageView();
			_messageView.IsSignupWizard = true;
			_locationView = new LocationView();

			_currentStep = 0; // Temporary

			SetCurrentStep();

			Title = "SignupPage";
			Shadow.Data.Runtime.MainDisplayInstance = this;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
		}

		private void btnBack_Clicked(object sender, EventArgs e)
		{
			if (_currentStep == 0)
				Navigation.PopModalAsync();
			else
				_currentStep--;

			SetCurrentStep();
		}

		private void _btnContinue_Clicked(object sender, EventArgs e)
		{
			if (_btnContinue.Text == "Continue")
			{

				if (_currentStep == 0)
				{

					if (string.IsNullOrEmpty(_userDetailView._txtFirstName.Text))
					{
						DisplayAlert("Validation Error", "Please enter your first name.", "OK");
						return;
					}
					else if (string.IsNullOrEmpty(_userDetailView._txtLastName.Text))
					{
						DisplayAlert("Validation Error", "Please enter your last name.", "OK");
						return;
					}
					else if (_userDetailView._birthDate.NullableDate == null)
					{
						DisplayAlert("Validation Error", "Please enter your birth date.", "OK");
						return;
					}
					else if (string.IsNullOrEmpty(_userDetailView._txtCellPhone.Text))
					{
						DisplayAlert("Validation Error", "Please enter your cell phone number.", "OK");
						return;
					}
				};

				if (_currentStep == 1)
				{

					if (string.IsNullOrEmpty(_locationView.address))
					{
						DisplayAlert("Validation Error", "Please enter your address, or click on the map to select your location.", "OK");
						return;
					}
				
				};

				_currentStep++;
				SetCurrentStep();

				if (!_locationView.createdView)
				{
					_locationView.CreateView();
				}
			}
			else if (_btnContinue.Text == "Finish")
			{
				RegisterUser( _userDetailView._txtFirstName.Text,
				             _userDetailView._txtLastName.Text, _userDetailView._txtCellPhone.Text, _userDetailView._birthDate.Date, _userDetailView._txtMedicalAid.Text,
						 _userDetailView._txtMedicalAidNum.Text, _userDetailView._txtSecurityCompany.Text, _userDetailView._txtSecurityCompanyNum.Text);

				Shadow.Data.Runtime.MessageLine1 = _messageView._txtMessageLine1.Text;
				//Shadow.Data.Runtime.MessageLine2 = _messageView._txtMessageLine2.Text;
			}
		}


		private async void RegisterUser(string firstName, string lastName, string phoneNo, DateTime dateOfBirth, 
		                                string medicalProvider, string medicalproviderPhoneno, string securityProvider,
		                               string securityproviderPhoneno)
		{
			try
			{
				if (ShadowService.CurrentUser != null)
				{
					ShadowService.CurrentUser.firstName = firstName;
					ShadowService.CurrentUser.lastName = lastName;
					ShadowService.CurrentUser.phoneNo = phoneNo;
					ShadowService.CurrentUser.medicalProvider = medicalProvider;
					ShadowService.CurrentUser.medicalproviderPhoneno = medicalproviderPhoneno;
					ShadowService.CurrentUser.securityProvider = securityProvider;
					ShadowService.CurrentUser.securityproviderPhoneno = securityproviderPhoneno;
					ShadowService.CurrentUser.Address = _locationView.address;
					ShadowService.CurrentUser.Lat = _locationView.position.Latitude;
					ShadowService.CurrentUser.Long = _locationView.position.Longitude;
					ShadowService.CurrentUser.DateOfBirth = dateOfBirth;

					foreach (Shadow.Data.Contact shadowcontact in Shadow.Data.Runtime.Contacts)
					{
						Model.Contact svcContact = new Contact();
						svcContact.firstName = shadowcontact.Name;
						svcContact.phoneNo = shadowcontact.Mobile;
						ShadowService.CurrentUser.addEmergencyContact(svcContact);
					}

					using (var dlg = UserDialogs.Instance.Loading(Shadow.Lang.AppResources.lblRegistering, null, "", true))
					{
						await ShadowService.SaveCurrentUser();
					};

					Shadow.Data.Runtime.RemLogin.Message1 = _messageView._txtMessageLine1.Text;
					//Shadow.Data.Runtime.RemLogin.Message2 = _messageView._txtMessageLine2.Text;
					Shadow.Data.Runtime.SaveRemLogin();

					await DisplayAlert("Success", "Registered new account: " + ShadowService.CurrentUser.email, "OK");
					await Navigation.PopModalAsync();
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

		private void SetCurrentStep()
		{
			if (_subViewContainer.Children.Count > 0)
				_subViewContainer.Children.Clear();
			_btnCurrentLocation.IsVisible = false;
			switch (_currentStep)
			{
				case 0:
					{
						_subViewContainer.Children.Add(_userDetailView);
						UIHelper.SetTitle(_titleBar, "Sign up/Registration");
						break;
					}
				case 1: { 
						_subViewContainer.Children.Add(_locationView); 
						UIHelper.SetTitle(_titleBar, "Sign up/Primary Location");
						_btnCurrentLocation.IsVisible = true;
						break; 
					} 
				case 2: { 
						_subViewContainer.Children.Add(_deviceView); 
						UIHelper.SetTitle(_titleBar, "Sign up/Set up Shadow Device");
						break; 
						}
				case 3:
					{
						_subViewContainer.Children.Add(_messageView);
						UIHelper.SetTitle(_titleBar, "Sign up/Emergency Message");
						break;
					}
				case 4:
					{
						_subViewContainer.Children.Add(_contactsView);
						UIHelper.SetTitle(_titleBar, "Sign up/Add Emergency Contacts");
						break;
					}
			}

			if (_currentStep == 4)
			{
				_btnContinue.Text = "Finish";
			}
			else
			{
				_btnContinue.Text = "Continue";
			}
		}

		public static async void DisplayMsgPreview(string Msg)
		{			
			await Shadow.Data.Runtime.MainDisplayInstance.DisplayAlert("Alert Preview", Msg, "OK");
		}

	}
}


