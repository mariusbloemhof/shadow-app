using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Newtonsoft.Json;
using Shadow.Lang;
using Plugin.Messaging;


namespace Shadow
{
	public class MainPage : ContentPage
	{
		StackLayout _btnContacts;
		StackLayout _btnMessage;
		StackLayout _btnDevice;
		StackLayout _btnFAQ;

		Label _lblContacts;
		Label _lblMessage;
		Label _lblDevice;
		Label _lblFAQ;

		StackLayout _titleBar = UIHelper.CreateHomeTitleBar("Contacts");
		StackLayout _container;

		DeviceView _deviceView;
		ContactsView _contactsView;
		MessageView _messageView;
		FaqView _faqView;

		Label _connectedDevices;

		public Label BatteryLabel;

		public MainPage()
		{
            Shadow.Data.Runtime.BLEDevice.onBatteryLevelRead += batteryLevelRead;
            BatteryLabel = new Label
			{
				HorizontalOptions = LayoutOptions.End,
				TextColor = Xamarin.Forms.Color.White,
				FontSize = 12,
				Text = "0%"
			};

			_lblContacts = new Label
			{
				Text = "Contacts",
				TextColor = UIConst.DefaultText,
				FontSize = 11,
				HorizontalTextAlignment = TextAlignment.Center
			};

			_btnContacts = new StackLayout()
			{
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Children = {
					new Image {
						Source = "contacts.png",
						HeightRequest = 25,
						WidthRequest = 25
					},
					_lblContacts
				}
			};
			var tapbtnContacts = new TapGestureRecognizer();
			tapbtnContacts.Tapped += (s, e) =>
			{
				SelectView(1);
			};
			_btnContacts.GestureRecognizers.Add(tapbtnContacts);


			_lblMessage = new Label
			{
				Text = "Message",
				TextColor = UIConst.DefaultText,
				FontSize = 11,
				HorizontalTextAlignment = TextAlignment.Center
			};
			_btnMessage = new StackLayout()
			{
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Children = {
					new Image {
						Source = "message.png",
						HeightRequest = 25,
						WidthRequest = 25
					},
					_lblMessage
				}
			};
			var tapbtnMessage = new TapGestureRecognizer();
			tapbtnMessage.Tapped += (s, e) =>
			{
				SelectView(2);
			};
			_btnMessage.GestureRecognizers.Add(tapbtnMessage);

			_lblDevice = new Label
			{
				Text = "Device",
				TextColor = UIConst.DefaultText,
				FontSize = 11,
				HorizontalTextAlignment = TextAlignment.Center
			};
			_btnDevice =  new StackLayout()
			{
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Children = {
					new Image {
						Source = "device.png",
						HeightRequest = 25,
						WidthRequest = 25
					},
					_lblDevice
				}
			};
			var tapbtnDevice = new TapGestureRecognizer();
			tapbtnDevice.Tapped += (s, e) =>
			{
				SelectView(3);
			};
			_btnDevice.GestureRecognizers.Add(tapbtnDevice);

			_lblFAQ = new Label
			{
				Text = "FAQ",
				TextColor = UIConst.DefaultText,
				FontSize = 11,
				HorizontalTextAlignment = TextAlignment.Center
			};
			_btnFAQ = new StackLayout()
			{
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Children = {
					new Image {
						Source = "faq.png",
						HeightRequest = 25,
						WidthRequest = 25
					},
					_lblFAQ
				}
			};
			var tapbtnFAQ = new TapGestureRecognizer();
			tapbtnFAQ.Tapped += (s, e) =>
			{
				SelectView(4);
			};
			_btnFAQ.GestureRecognizers.Add(tapbtnFAQ);

			_connectedDevices = new Label
			{
				Text = "No Device Connected",
				TextColor = Xamarin.Forms.Color.White,
				FontSize = 8
			};

			int toolbarSize = 40;
			//if (Device.OS == TargetPlatform.iOS)
			//	toolbarSize = 40;

			_container = new StackLayout
			{
				VerticalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = UIConst.BgColor,
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
                            UIHelper.TitleSpacer(),
                                    _titleBar,
                        }
                    },
                    new StackLayout {
                        Orientation = StackOrientation.Horizontal,
                        BackgroundColor = UIConst.LightGrey,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        Padding = 10,
                        Children = {
                            new Image {
                                Source = "devicephoto.png",
                                HeightRequest = 45,
                                HorizontalOptions = LayoutOptions.Start
                            },
                            new StackLayout {
                                HorizontalOptions = LayoutOptions.FillAndExpand,
                                Orientation = StackOrientation.Vertical,
                                Children = {
                                    new Label {
                                        FontSize = 15,
                                        FontAttributes = FontAttributes.Bold,
                                        Text = "Shadow Device",
                                        TextColor = UIConst.OrangeColor,
                                    },
                                    _connectedDevices,
                                }
                            },
                            BatteryLabel
                        }
                    },
                    new StackLayout {
                        BackgroundColor = Xamarin.Forms.Color.White,
                        HeightRequest = 0.5
                    },
                    _container
                    ,
                    new StackLayout {
                        BackgroundColor = Xamarin.Forms.Color.White,
                        HeightRequest = 0.5
                    },
                    new StackLayout {
                        Padding = 10,
                        Spacing = 0,
                        BackgroundColor = UIConst.ToolbarBgColor,
                        HeightRequest = toolbarSize,
                        Orientation = StackOrientation.Horizontal,
                        VerticalOptions = LayoutOptions.End,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        Children = {
                            _btnContacts,
                            _btnMessage,
                            _btnDevice,
                            _btnFAQ
                        }
                    }
                }
            };

			UIHelper.SetHomeTitle(_titleBar, "Contacts");

			_deviceView = new DeviceView();
			_contactsView = new ContactsView();
			_messageView = new MessageView();
			_faqView = new FaqView();

			_container.Children.Add(_contactsView);
        }

		private void SelectView(int view)
		{
			_container.Children.Clear();

			_lblMessage.TextColor = UIConst.DefaultText;
			_lblFAQ.TextColor = UIConst.DefaultText;
			_lblDevice.TextColor = UIConst.DefaultText;
			_lblContacts.TextColor = UIConst.DefaultText;

			switch (view)
			{
				case 1: UIHelper.SetHomeTitle(_titleBar, "Contacts"); _lblContacts.TextColor = UIConst.OrangeColor; _container.Children.Add(_contactsView); break;
				case 2: UIHelper.SetHomeTitle(_titleBar, "Message"); _lblMessage.TextColor = UIConst.OrangeColor; _container.Children.Add(_messageView); break;
				case 3: UIHelper.SetHomeTitle(_titleBar, "Device");_lblDevice.TextColor = UIConst.OrangeColor; _container.Children.Add(_deviceView); break;
				case 4: UIHelper.SetHomeTitle(_titleBar, "FAQ");_lblFAQ.TextColor = UIConst.OrangeColor; _container.Children.Add(_faqView); break;
			};

			Title = "MainPage";
			Shadow.Data.Runtime.MainDisplayInstance = this;
		}

		public static async void DisplayMsgPreview(string msg)
		{			
			await Shadow.Data.Runtime.MainDisplayInstance.DisplayAlert("Alert Preview", msg, "OK");
			return;

			var smsMessenger = MessagingPlugin.SmsMessenger;
			if (smsMessenger.CanSendSms)
				smsMessenger.SendSms("", msg);
		}

		protected override void OnAppearing()
		{
			if (_connectedDevices != null)
			{
				UpdatedConnectedDevicesLabel();
			}

			base.OnAppearing();
		}

		public void UpdatedConnectedDevicesLabel()
		{
			if (Shadow.Data.Runtime.ConnectedDevices.Count > 0)
            {
                _connectedDevices.Text = "Connected";
                Device.BeginInvokeOnMainThread(() => { UpdateBatteryLevel(); });
            }
				
			else
				_connectedDevices.Text = "No Device Connected";
		}

		public void UpdateBatteryLevel()
		{
            //read battery. Result will return in event for onBatteryLevelRead
            Shadow.Data.Runtime.BLEDevice.BatteryLevel(Shadow.Data.Runtime.ConnectedDevices[0].Device).ConfigureAwait(false);
        }

        public void batteryLevelRead(object sender, int level)
        {
            Device.BeginInvokeOnMainThread(() => 
            {
                BatteryLabel.Text = level.ToString() + "%";
            });
        }

    }
}


