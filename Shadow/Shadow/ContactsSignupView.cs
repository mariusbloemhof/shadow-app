using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Newtonsoft.Json;

using Shadow.Lang;

namespace Shadow
{
	public class ContactsSignupView : ContentView
	{
		ImageButton _AddContact;
		//StackLayout _contactsContainer;
		public static ObservableCollection<Shadow.Data.Contact> items { get; set; }
		public static ContactsSignupView ContactsViewInstance;

		public ContactsSignupView ()
		{
			_AddContact = new ImageButton {
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				BackgroundColor = UIConst.OrangeColor,
				TextColor = Xamarin.Forms.Color.White,
				ImageOrientation = ImageOrientation.ImageToLeft,
				Text = "Add Contact",
				Image = "addcontacts.png",
				ImageHeightRequest = 25,
				ImageWidthRequest = 25,
				WidthRequest = 200,
				ImageTextPadding = 10,
				FontSize = 14
			};


			_AddContact.Clicked += (sender, e) =>
			{
				Navigation.PushModalAsync(new ContactPickerPage());
			};


			ListView lstView = new ListView { 
				SeparatorVisibility = SeparatorVisibility.None,
				BackgroundColor = UIConst.BgColor,
				VerticalOptions = LayoutOptions.FillAndExpand,
			};

			lstView.ItemSelected += OnSelection;
			lstView.ItemTapped += OnTap;
			lstView.ItemsSource = Shadow.Data.Runtime.Contacts;

			var temp = new DataTemplate(typeof(contactSignupTextViewCell));
			lstView.ItemTemplate = temp;

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
							new StackLayout { HeightRequest = 5 },
							new StackLayout {
								Padding = 10,
								Children = {
									new Label {
										HorizontalOptions = LayoutOptions.FillAndExpand,
										VerticalOptions = LayoutOptions.FillAndExpand,
										Text = "Add the contacts you want to notify in case of an emergency",
										FontSize = UIConst.DefaultFontSize+2,
										TextColor = Xamarin.Forms.Color.White
									}
								}
							},
						}
					},
					new StackLayout {
						Padding = 20,
						VerticalOptions = LayoutOptions.FillAndExpand,
						Children = {
							lstView
						}
					},
					_AddContact,
					new StackLayout { HeightRequest = 20 }
				}
			};

			ContactsViewInstance = this;
		}

		private void SaveContacts()
		{
			var json = JsonConvert.SerializeObject(Shadow.Data.Runtime.Contacts);
			bool result = Shadow.IO.LocalStorage.WriteTextFile("root", Shadow.Data.Const.CONTACTS, json).Result;
		}

		public void OpenContactPicker()
		{
			Navigation.PushModalAsync(new ContactPickerPage());
		}

		public void RemoveContact(string Name)
		{
			foreach (Shadow.Data.Contact contact in Shadow.Data.Runtime.Contacts)
			{
				if (contact.Name == Name)
				{
					Shadow.Data.Runtime.Contacts.Remove(contact);
					break;
				}
			}
			SaveContacts();
		}

		void OnTap(object sender, ItemTappedEventArgs e)
		{
			//DisplayAlert("Item Tapped", e.Item.ToString(), "Ok");
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

	public class contactSignupTextViewCell : ExtendedViewCell
	{
		public contactSignupTextViewCell()
		{
			StackLayout container = new StackLayout
			{
				BackgroundColor = UIConst.LightGrey,
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.Center,
				Padding = 10
			};

			Label lblHeading = new Label
			{
				TextColor = UIConst.DefaultText,
				HorizontalOptions = LayoutOptions.StartAndExpand,
				VerticalOptions = LayoutOptions.Center,
			};

			Label lblSubHeading = new Label
			{
				TextColor = UIConst.DefaultText,
				HorizontalOptions = LayoutOptions.End,
				VerticalOptions = LayoutOptions.Center,
			};

			lblHeading.SetBinding(Label.TextProperty, "Name");
			container.Children.Add(lblHeading);

			lblSubHeading.SetBinding(Label.TextProperty, "Mobile");
			container.Children.Add(lblSubHeading);

			var moreAction = new MenuItem { Text = "Edit" };
			moreAction.SetBinding(MenuItem.CommandParameterProperty, new Binding("Name"));
			moreAction.Clicked += OnMore;

			var deleteAction = new MenuItem { Text = "Delete", IsDestructive = true }; // red background
			deleteAction.SetBinding(MenuItem.CommandParameterProperty, new Binding("Name"));
			deleteAction.Clicked += OnDelete;

			this.ContextActions.Add(moreAction);
			this.ContextActions.Add(deleteAction);

			this.SeparatorColor = UIConst.BgColor;
			this.ShowSeparator = true;
			this.ShowDisclousure = false;

			View = container;
		}

		void OnMore(object sender, EventArgs e)
		{
			ContactsSignupView.ContactsViewInstance.OpenContactPicker();
		}

		void OnDelete(object sender, EventArgs e)
		{
			var item = (MenuItem)sender;
			ContactsSignupView.ContactsViewInstance.RemoveContact(item.CommandParameter.ToString());
		}
	}
}


