using System;

using Xamarin.Forms;
using Shadow;
using Xamarin.Contacts;
using System.Collections.Generic;
using System.Linq;
using Shadow.Lang;
using Newtonsoft.Json;

namespace Shadow
{
	public class ContactPickerPage : BaseContentPage
	{
		Button _btnContinue;
		Button _btnBack;
		private Xamarin.Contacts.AddressBook _book;
		private IEnumerable<Shadow.Data.Contact> _contacts;
		private ListView _lstView;

		public ContactPickerPage()
		{
			_lstView = new ListView
			{
				SeparatorVisibility = SeparatorVisibility.Default,
				SeparatorColor = UIConst.BgColor,
				VerticalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = UIConst.LightGrey,
				//RowHeight = 35
			};

			_btnBack = new Button
			{
				HorizontalOptions = LayoutOptions.StartAndExpand,
				VerticalOptions = LayoutOptions.End,
				BorderColor = Xamarin.Forms.Color.White,
				BorderWidth = 2,
				BorderRadius = UIConst.DefaultButtonBorderRadius,
				TextColor = Xamarin.Forms.Color.White,
				Text = "Cancel",
				WidthRequest = 100
			};
			_btnBack.Clicked += btnBack_Clicked;

			_btnContinue = new Button
			{
				HorizontalOptions = LayoutOptions.EndAndExpand,
				VerticalOptions = LayoutOptions.End,
				BorderColor = Xamarin.Forms.Color.White,
				BorderWidth = 2,
				BorderRadius = UIConst.DefaultButtonBorderRadius,
				TextColor = Xamarin.Forms.Color.White,
				Text = "OK",
				WidthRequest = 100
			};
			_btnContinue.Clicked += _btnContinue_Clicked;
		
			var temp = new DataTemplate(typeof(addressBookContactTextViewCell));
			_lstView.ItemTemplate = temp;
			_lstView.ItemSelected += OnSelection;

			ReadContacts();

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
								UIHelper.TitleSpacer(),
								UIHelper.CreateHomeTitleBar("Adding Contacts"),
								new StackLayout { HeightRequest = 10 },
							}
						},
						new StackLayout {
							VerticalOptions = LayoutOptions.FillAndExpand,
							Padding = 10,
							Children = {
								_lstView,
							}
						},
						new StackLayout { HeightRequest = 5 },
						new StackLayout {
							Padding = 10,
							Orientation = StackOrientation.Horizontal,
							HorizontalOptions = LayoutOptions.FillAndExpand,
							Children = {
								_btnBack,
								_btnContinue
							}
						},
						new StackLayout { HeightRequest = 5 },
					}

			};
		}

		protected void btnBack_Clicked(object sender, EventArgs e)
		{
			Navigation.PopModalAsync();
		}

		private bool ContactAlreadyAdded(Shadow.Data.Contact newcontact)
		{
			foreach (Shadow.Data.Contact contact in Shadow.Data.Runtime.Contacts)
			{
				if ((newcontact.Name == contact.Name) && (newcontact.Mobile == contact.Mobile))
					return true;
			}
			return false;
		}

		private void SaveContacts()
		{
			var json = JsonConvert.SerializeObject(Shadow.Data.Runtime.Contacts);
			bool result = Shadow.IO.LocalStorage.WriteTextFile("root", Shadow.Data.Const.CONTACTS, json).Result;
		}

		protected void _btnContinue_Clicked(object sender, EventArgs e)
		{
			Shadow.Data.Runtime.Contacts.Clear();
			foreach (Shadow.Data.Contact contact in _contacts)
			{
				if (contact.Selected)
				{	
					Shadow.Data.Runtime.Contacts.Add(contact);
				}
			}
			SaveContacts();
			Navigation.PopModalAsync();
		}

		private async void ReadContacts()
		{
#if __IOS__
			_book = new Xamarin.Contacts.AddressBook();
#else
			_book = new Xamarin.Contacts.AddressBook(Android.App.Application.Context);
			#endif


			if (_contacts != null) return;

			var contacts = new List<Shadow.Data.Contact>();
			await _book.RequestPermission().ContinueWith(t =>
			{
				if (!t.Result)
				{
					Console.WriteLine("Sorry ! Permission was denied by user or manifest !");
					return;
				}

				List<Contact> phonebookcontacts = _book.ToList();

				foreach (var contact in phonebookcontacts.Where(c => c.Phones.Any()))
				{
					var firstOrDefault = contact.Phones.FirstOrDefault();
					if (firstOrDefault != null)
					{

						Shadow.Data.Contact shadowcontact = new Shadow.Data.Contact
						{
							Name = contact.DisplayName,
							Mobile = firstOrDefault.Number
						};

						if (ContactAlreadyAdded(shadowcontact))
						   shadowcontact.Selected = true;

						contacts.Add(shadowcontact);						
					}
				}
			});

			_contacts = (from c in contacts orderby c.Name select c).ToList();
			_lstView.ItemsSource = _contacts;
			return;
		}

		void OnSelection(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem == null)
			{
				return; //ItemSelected is called on deselection, which results in SelectedItem being set to null
			}
			//DisplayAlert("Item Selected", e.SelectedItem.ToString(), "Ok");
			//comment out if you want to keep selections
			ListView lst = (ListView)sender;
			lst.SelectedItem = null;
		}
	}

	public class addressBookContactTextViewCell : ViewCell
	{
		public addressBookContactTextViewCell()
		{
			StackLayout container = new StackLayout
			{
				Spacing = 1,
				Padding = 5,
				//BackgroundColor = UIConst.LightGrey,
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.Center,
			};

			Image imgContact = new Image
			{
				Source = "contactplaceholder.png",
				WidthRequest = 30
			};
			container.Children.Add(imgContact);

			container.Children.Add(new StackLayout { WidthRequest = 10 });

			Label lblHeading = new Label
			{
				TextColor = UIConst.DefaultText,
				HorizontalOptions = LayoutOptions.StartAndExpand,
				VerticalOptions = LayoutOptions.Center,
			};
			lblHeading.SetBinding(Label.TextProperty, "Name");
			container.Children.Add(lblHeading);

			ImageButton btnCheck = new ImageButton
			{
				ImageOrientation = ImageOrientation.NoText,
				HorizontalOptions = LayoutOptions.End,
				WidthRequest = 20,
				Image = "tickoff.png"
			};

			Switch cbCheck = new Switch
			{

			};
			cbCheck.SetBinding(Switch.IsToggledProperty, "Selected");
			container.Children.Add(cbCheck);

		/*	btnCheck.Clicked += (sender, e) =>
			{
				ImageButton btnThis = (ImageButton)sender;
				if ((btnThis.Source == null) || (((FileImageSource)btnThis.Source).File == "tickoff.png"))
					btnThis.Source = "tick.png";
				else
					btnThis.Source = "tickoff.png";
			};*/






			/*this.SeparatorColor = Xamarin.Forms.Color.White;
			this.ShowSeparator = true;
			this.ShowDisclousure = false;*/

			View = container;
		}
	}


}


