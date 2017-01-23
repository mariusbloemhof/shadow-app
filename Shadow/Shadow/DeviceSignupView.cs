using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Newtonsoft.Json;
//using BluetoothLE.Core;
//using BluetoothLE.Core.Events;

using Shadow.Lang;

namespace Shadow
{
	public class DeviceSignupView : ContentView
	{
		ImageButton _btnScan;
		private const int SCAN_TIMEOUT = 6000;

		public static ObservableCollection<string> items { get; set; }
		public static DeviceSignupView DeviceViewInstance;

		public static ListView _lstView;

		public DeviceSignupView ()
		{

			_btnScan = new ImageButton
			{
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				BackgroundColor = UIConst.OrangeColor,
				TextColor = Xamarin.Forms.Color.White,
				ImageOrientation = ImageOrientation.ImageToLeft,
				Text = "Scan for Shadow",
				Image = "refresh.png",
				ImageHeightRequest = 30,
				ImageWidthRequest = 30,
				WidthRequest = 200,
				ImageTextPadding = 10,
				FontSize = 14
			};
			_btnScan.Clicked += (sender, e) => { StartScanningDevices(); };

			_lstView = new ListView
			{
				SeparatorVisibility = SeparatorVisibility.None,
				BackgroundColor = UIConst.BgColor,
				VerticalOptions = LayoutOptions.FillAndExpand
			};
			_lstView.ItemSelected += OnSelection;
			_lstView.ItemTapped += OnTap;

			_lstView.ItemsSource = Shadow.Data.Runtime.DevicesFound;
			var temp = new DataTemplate(typeof(deviceSignupTextViewCell));
			_lstView.ItemTemplate = temp;
		
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
							new StackLayout { HeightRequest = 10 },	
							new StackLayout {
								Padding = 10,
								Children = {
									new Label {
										HorizontalOptions = LayoutOptions.FillAndExpand,
										VerticalOptions = LayoutOptions.FillAndExpand,
										Text = "You will now pair your Shadow Device with this Smart phone. Click on the Button below to find your device",
										FontSize = UIConst.DefaultFontSize+2,
										TextColor = Xamarin.Forms.Color.White
									}
								}
							}
						}
					},
					new StackLayout {
						Padding = 20,
						VerticalOptions = LayoutOptions.FillAndExpand,
						Children = {
							_lstView,
							new StackLayout { HeightRequest = 5 },
							_btnScan,
							new StackLayout { HeightRequest = 20 }
						}
					}
				}

			};

			DeviceViewInstance = this;
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

		private void StartScanningDevices()
		{
			_btnScan.IsEnabled = false;
			//App.BluetoothAdapter.StartScanningForDevices();
			Shadow.Data.Runtime.BLEDevice.StartScanning();
			_lstView.ItemsSource = Shadow.Data.Runtime.DevicesFound;

			Device.StartTimer(new TimeSpan(0, 0, 0, 0, SCAN_TIMEOUT), () =>
			{
				Shadow.Data.Runtime.BLEDevice.StopScanning();
				_btnScan.IsEnabled = true;
				return false;
			});
		}

		public void WaitForPair()
		{
			Device.StartTimer(new TimeSpan(0, 0, 0, 0, 5000), () =>
			{
				_lstView.ItemsSource = null;
				_lstView.ItemsSource = Shadow.Data.Runtime.DevicesFound;

				if (DeviceAction.Pairing)
					return true;
				else
					return false;
			});
		}

		public static void RefreshList()
		{
			_lstView.ItemsSource = null;
			_lstView.ItemsSource = Shadow.Data.Runtime.DevicesFound;
		}
	}

	public class deviceSignupTextViewCell : ExtendedViewCell
	{
		public deviceSignupTextViewCell()
		{
			StackLayout container = new StackLayout
			{
				BackgroundColor = UIConst.LightGrey,
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.Center,
				Padding = 3
			};
			//container.SetBinding(Layout.BackgroundColorProperty, new Binding("BackgroundColor"));

			Image imgPairedIcon = new Image
			{
				//TextColor = UIConst.DefaultText,
				HorizontalOptions = LayoutOptions.Start,
				VerticalOptions = LayoutOptions.Center,
				WidthRequest = 20,
				HeightRequest = 20,
				//BackgroundColor = Xamarin.Forms.Color.Red
			};
			imgPairedIcon.SetBinding(Image.SourceProperty, "PairedIcon");
			//imgPairedIcon.SetBinding(Image.ImageProperty, "PairedIcon");
			container.Children.Add(imgPairedIcon);

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

			lblSubHeading.SetBinding(Label.TextProperty, "Id");
			container.Children.Add(lblSubHeading);

			var pairAction = new MenuItem { Text = "Pair" };
			pairAction.SetBinding(MenuItem.CommandParameterProperty, new Binding("Id"));
			pairAction.Clicked += OnPair;
			this.ContextActions.Add(pairAction);

			var deleteAction = new MenuItem { Text = "Unpair", IsDestructive = true }; // red background
			deleteAction.SetBinding(MenuItem.CommandParameterProperty, new Binding("Id"));
			deleteAction.Clicked += OnDelete;

			this.ContextActions.Add(deleteAction);

			this.SeparatorColor = UIConst.BgColor;
			this.ShowSeparator = true;
			this.ShowDisclousure = false;

			View = container;
		}

		void OnPair(object sender, EventArgs e)
		{
			var item = (MenuItem)sender;
			DeviceAction.WaitForDevicePair(item.CommandParameter.ToString());
			DeviceSignupView.DeviceViewInstance.WaitForPair();
		}

		void OnDelete(object sender, EventArgs e)
		{
			var item = (MenuItem)sender;
			DeviceAction.UnpairDevice(item.CommandParameter.ToString());
			DeviceSignupView.RefreshList();
		}
	}
}


