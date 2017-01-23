using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Newtonsoft.Json;

using Shadow.Lang;

namespace Shadow
{
	public class UserDetailView : ContentView
	{
		public ExtendedEntry _txtFirstName;
		public ExtendedEntry _txtLastName;
		public ExtendedEntry _txtCellPhone;
		public CustomDatePicker _birthDate;
		public ExtendedEntry _txtMedicalAid;
		public ExtendedEntry _txtMedicalAidNum;
		public ExtendedEntry _txtSecurityCompany;
		public ExtendedEntry _txtSecurityCompanyNum;

		public UserDetailView()
		{

			ScrollView scrollView = new ScrollView
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand
			};


			_txtFirstName = new ExtendedEntry
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				HasBorder = false,
				TextColor = UIConst.DefaultText,
				PlaceholderTextColor = UIConst.DefaultText,
				BackgroundColor = UIConst.BgColor
			};

			_txtLastName = new ExtendedEntry
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				HasBorder = false,
				TextColor = UIConst.DefaultText,
				PlaceholderTextColor = UIConst.DefaultText,
				BackgroundColor = UIConst.BgColor
			};

			_birthDate = new CustomDatePicker
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = UIConst.BgColor,
				TextColor = UIConst.DefaultText,
				Format = "yyyy-MM-dd",
				NullText = "Birth Date",
				NullableDate = null
			};

			_txtCellPhone = new ExtendedEntry
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				HasBorder = false,
				TextColor = UIConst.DefaultText,
				PlaceholderTextColor = UIConst.DefaultText,
				BackgroundColor = UIConst.BgColor,
				Keyboard = Keyboard.Telephone
			};

			_txtMedicalAid = new ExtendedEntry
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				HasBorder = false,
				TextColor = UIConst.DefaultText,
				PlaceholderTextColor = UIConst.DefaultText,
				BackgroundColor = UIConst.BgColor
			};

			_txtMedicalAidNum = new ExtendedEntry
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				HasBorder = false,
				TextColor = UIConst.DefaultText,
				PlaceholderTextColor = UIConst.DefaultText,
				BackgroundColor = UIConst.BgColor
			};

			_txtSecurityCompany = new ExtendedEntry
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				HasBorder = false,
				TextColor = UIConst.DefaultText,
				PlaceholderTextColor = UIConst.DefaultText,
				BackgroundColor = UIConst.BgColor
			};

			_txtSecurityCompanyNum = new ExtendedEntry
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				HasBorder = false,
				TextColor = UIConst.DefaultText,
				PlaceholderTextColor = UIConst.DefaultText,
				BackgroundColor = UIConst.BgColor
			};

			StackLayout content = new StackLayout
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Orientation = StackOrientation.Vertical,
				Padding = 20,
				Children = {
					new Label {
						TextColor = Xamarin.Forms.Color.Silver,
						FontSize = 10,
						FontAttributes = FontAttributes.Bold,
						Text = "First Name"
					},
					new StackLayout
					{
						Orientation = StackOrientation.Horizontal,
						HorizontalOptions = LayoutOptions.FillAndExpand,
						Children = {
							new Image {
								HorizontalOptions = LayoutOptions.Start,
								WidthRequest = 20,
								HeightRequest = 20,
								Source = ImageSource.FromFile("avatar.png")
							},
							_txtFirstName
						}
					},
					new StackLayout { HeightRequest = 10 },
					new Label {
						TextColor = Xamarin.Forms.Color.Silver,
						FontSize = 10,
						FontAttributes = FontAttributes.Bold,
						Text = "Last Name"
					},
					new StackLayout
					{
						Orientation = StackOrientation.Horizontal,
						HorizontalOptions = LayoutOptions.FillAndExpand,
						Children = {
							new Image {
								HorizontalOptions = LayoutOptions.Start,
								WidthRequest = 20,
								HeightRequest = 20,
								Source = ImageSource.FromFile("avatar.png")
							},
							_txtLastName
						}
					},
					new StackLayout { HeightRequest = 10 },
					new Label {
						TextColor = Xamarin.Forms.Color.Silver,
						FontSize = 10,
						FontAttributes = FontAttributes.Bold,
						Text = "Birth Date"
					},
					new StackLayout
					{
						Orientation = StackOrientation.Horizontal,
						HorizontalOptions = LayoutOptions.FillAndExpand,
						Children = {
							new Image {
								HorizontalOptions = LayoutOptions.Start,
								WidthRequest = 20,
								HeightRequest = 20,
								Source = ImageSource.FromFile("birthdate.png")
							},
							_birthDate
						}
					},
					new StackLayout { HeightRequest = 10 },
					new Label {
						TextColor = Xamarin.Forms.Color.Silver,
						FontSize = 10,
						FontAttributes = FontAttributes.Bold,
						Text = "Cellphone Number"
					},
					new StackLayout
					{
						Orientation = StackOrientation.Horizontal,
						HorizontalOptions = LayoutOptions.FillAndExpand,
						Children = {
							new Image {
								HorizontalOptions = LayoutOptions.Start,
								WidthRequest = 20,
								HeightRequest = 20,
								Source = ImageSource.FromFile("phone.png")
							},
							_txtCellPhone
						}
					},
					new StackLayout { HeightRequest = 10 },
					new Label {
						TextColor = Xamarin.Forms.Color.Silver,
						FontSize = 10,
						FontAttributes = FontAttributes.Bold,
						Text = "Medical Aid"
					},
					new StackLayout
					{
						Orientation = StackOrientation.Horizontal,
						HorizontalOptions = LayoutOptions.FillAndExpand,
						Children = {
							new Image {
								HorizontalOptions = LayoutOptions.Start,
								WidthRequest = 20,
								HeightRequest = 20,
								Source = ImageSource.FromFile("medical.png")
							},
							_txtMedicalAid
						}
					},
					new StackLayout { HeightRequest = 10 },
					new Label {
						TextColor = Xamarin.Forms.Color.Silver,
						FontSize = 10,
						FontAttributes = FontAttributes.Bold,
						Text = "Medical Aid Number"
					},
					new StackLayout
					{
						Orientation = StackOrientation.Horizontal,
						HorizontalOptions = LayoutOptions.FillAndExpand,
						Children = {
							new Image {
								HorizontalOptions = LayoutOptions.Start,
								WidthRequest = 20,
								HeightRequest = 20,
								Source = ImageSource.FromFile("medical.png")
							},
							_txtMedicalAidNum
						}
					},
					new StackLayout { HeightRequest = 10 },
					new Label {
						TextColor = Xamarin.Forms.Color.Silver,
						FontSize = 10,
						FontAttributes = FontAttributes.Bold,
						Text = "Security Company"
					},
					new StackLayout
					{
						Orientation = StackOrientation.Horizontal,
						HorizontalOptions = LayoutOptions.FillAndExpand,
						Children = {
							new Image {
								HorizontalOptions = LayoutOptions.Start,
								WidthRequest = 20,
								HeightRequest = 20,
								Source = ImageSource.FromFile("alarm.png")
							},
							_txtSecurityCompany
						}
					},
					new StackLayout { HeightRequest = 10 },
					new Label {
						TextColor = Xamarin.Forms.Color.Silver,
						FontSize = 10,
						FontAttributes = FontAttributes.Bold,
						Text = "Security Company Number"
					},
					new StackLayout
					{
						Orientation = StackOrientation.Horizontal,
						HorizontalOptions = LayoutOptions.FillAndExpand,
						Children = {
							new Image {
								HorizontalOptions = LayoutOptions.Start,
								WidthRequest = 20,
								HeightRequest = 20,
								Source = ImageSource.FromFile("alarm.png")
							},
							_txtSecurityCompanyNum
						}
					}
				}
			};

			scrollView.Content = content;

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
						}
					},
					scrollView
				}

			};

		}


	}
}
