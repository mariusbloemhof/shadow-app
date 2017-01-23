using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Newtonsoft.Json;
//using BluetoothLE.Core;
//using BluetoothLE.Core.Events;

using Shadow.Lang;

namespace Shadow
{
	public class DeviceView : ContentView
	{
		ImageButton _btnScan;
		ImageButton _btnTest;
		private const int SCAN_TIMEOUT = 6000;

		public static ObservableCollection<string> items { get; set; }
		public static DeviceView DeviceViewInstance;

		public static ListView _lstView;

		public DeviceView ()
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

			_btnTest = new ImageButton
			{
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				BackgroundColor = UIConst.GreenColor,
				TextColor = Xamarin.Forms.Color.White,
				ImageOrientation = ImageOrientation.ImageToLeft,
				Text = "Test Device",
				Image = "check.png",
				ImageHeightRequest = 30,
				ImageWidthRequest = 30,
				WidthRequest = 200,
				ImageTextPadding = 10,
				FontSize = 14
			};
			_btnTest.Clicked += (sender, e) => { TestDevice(); };

			_lstView = new ListView
			{
				SeparatorVisibility = SeparatorVisibility.Default,
				SeparatorColor = UIConst.BgColor,
				VerticalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = UIConst.LightGrey,
				//RowHeight = 35
			};
			_lstView.ItemSelected += OnSelection;
			_lstView.ItemTapped += OnTap;
			_lstView.ItemsSource = Shadow.Data.Runtime.DevicesFound;

			var temp = new DataTemplate(typeof(deviceTextViewCell));
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
									new StackLayout {
										Orientation = StackOrientation.Horizontal,
										Children = {											
											new Label {
												VerticalOptions = LayoutOptions.End,
												Text = "Your connected Shadow devices",
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
						Padding = 10,
						VerticalOptions = LayoutOptions.FillAndExpand,
						Children = {
							_lstView,
							new StackLayout { HeightRequest = 5 },
							_btnScan,
							new StackLayout { HeightRequest = 5 },
							_btnTest,
							new StackLayout { HeightRequest = 5 }
						}
					}
				}

			};

			DeviceViewInstance = this;
					
		}
			

		void OnTap(object sender, ItemTappedEventArgs e)
		{			
			//_lstView.ItemsSource = Shadow.Data.Runtime.DevicesFound;
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
			Shadow.Data.Runtime.BLEDevice.StartScanning();
			_lstView.ItemsSource = Shadow.Data.Runtime.DevicesFound;

			Device.StartTimer(new TimeSpan(0, 0, 0, 0, SCAN_TIMEOUT), () =>
			{
				Shadow.Data.Runtime.BLEDevice.StopScanning();
				_btnScan.IsEnabled = true;
				return false;
			});
		}


		private async void TestDevice()
		{
			if (_btnTest.Text == "Test Device")
			{
				_btnTest.BackgroundColor = Xamarin.Forms.Color.Red;
				_btnTest.Text = "Test Mode";
				Shadow.Data.Runtime.TestMode = true;
				Shadow.Data.Runtime.TestModeOk = false;

				await Shadow.Data.Runtime.MainDisplayInstance.DisplayAlert("Test Mode", "Please press the button to test the device. If no signal is detected the test mode will exit in 10 seconds.", "OK");
				Device.StartTimer(new TimeSpan(0, 0, 0, 10, 0), () =>
				{
					if (!Shadow.Data.Runtime.TestModeOk)
					{
						_btnTest.BackgroundColor = UIConst.GreenColor;
						_btnTest.Text = "Test Device";
						Shadow.Data.Runtime.TestMode = false;
						Shadow.Data.Runtime.MainDisplayInstance.DisplayAlert("Test Mode", "Your device was not detected. Please have it checked out.", "OK");
					}
					else
					{
						_btnTest.BackgroundColor = UIConst.GreenColor;
						_btnTest.Text = "Test Device";
						Shadow.Data.Runtime.TestMode = false;
						Shadow.Data.Runtime.MainDisplayInstance.DisplayAlert("Test Mode", "Your device was detected successfully.", "OK");
					}
					return false;
				});
			}
			else
			{
				_btnTest.BackgroundColor = UIConst.GreenColor;
				_btnTest.Text = "Test Device";
				Shadow.Data.Runtime.TestMode = false;
			}
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

	public class deviceTextViewCell : ViewCell
	{
		public deviceTextViewCell()
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

			View = container;
		}

		void OnPair(object sender, EventArgs e)
		{
			var item = (MenuItem)sender;
			DeviceAction.WaitForDevicePair(item.CommandParameter.ToString());
			DeviceView.DeviceViewInstance.WaitForPair();
		}

		void OnDelete(object sender, EventArgs e)
		{
			var item = (MenuItem)sender;
			DeviceAction.UnpairDevice(item.CommandParameter.ToString());
            DeviceView.RefreshList();
		}
	}
}


