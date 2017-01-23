using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Newtonsoft.Json;

using Shadow.Lang;
using TK.CustomMap;
using TK.CustomMap.Api.Google;
using Xamarin.Forms.Maps;
using System.Collections.Generic;
using Plugin.Geolocator;
using Acr.UserDialogs;

namespace Shadow
{
	public class LocationView : ContentView
	{
		public TKCustomMap mapView;
		public PlacesAutoComplete autoComplete;
		public ImageButton btnCurrentLocation;
		public Position position;
		public string address;
		public bool createdView = false;

		public LocationView ()
		{
			

		}

		public async void CreateView()
		{
			createdView = true;
			var locator = CrossGeolocator.Current;
			locator.DesiredAccuracy = 50;

			using (var dlg = UserDialogs.Instance.Loading(Shadow.Lang.AppResources.lblGettingLocation, null, "", true))
			{
				var gpspos = await locator.GetPositionAsync(15000);
				position = new Position(gpspos.Latitude, gpspos.Longitude);
			};

			if (position == null)
			{
				await Shadow.Data.Runtime.MainDisplayInstance.DisplayAlert("Error", "No GPS location could be located. Please check your GPS settings and try again.", "OK");
				return;
			}

			autoComplete = new PlacesAutoComplete { 
				HorizontalOptions = LayoutOptions.StartAndExpand,
				ApiToUse = PlacesAutoComplete.PlacesApi.Google
			};
			autoComplete.OnSelected += (sender, e) =>
			{
				address = autoComplete.SearchText;
				GetLatLng(autoComplete.SearchText);
			};

			GmsPlace.Init("AIzaSyCIr4L-pEZNMiVx3bKC6cy0L7qVOSfVHRY");

			mapView = new TKCustomMap(MapSpan.FromCenterAndRadius(position, Distance.FromKilometers(0.7)));
			mapView.IsRegionChangeAnimated = true;
			if (Device.OS == TargetPlatform.iOS)
				mapView.HeightRequest = UIHelper.ScreenSize.Height - 200;
			else
				mapView.HeightRequest = UIHelper.ScreenSize.Height - 250;
			mapView.WidthRequest = UIHelper.ScreenSize.Width;
			mapView.MapClicked += (sender, e) =>
			{
				position = e.Value;
				AddCurrentPin(false);
			};		

			StackLayout container = new StackLayout
			{
				Padding = 0,
				Spacing = 0,
				Orientation = StackOrientation.Vertical,
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Children = {
					/*new StackLayout {
						Padding = 0,
						Spacing = 0,
						VerticalOptions = LayoutOptions.FillAndExpand,
						HorizontalOptions = LayoutOptions.FillAndExpand
					},*/
					new StackLayout {
						Orientation = StackOrientation.Horizontal,
						HorizontalOptions = LayoutOptions.FillAndExpand,
						Children = {
							autoComplete
						}
					},
					mapView
				}

			};

			ScrollView scrollView = new ScrollView
			{
				Content = container
			};
			Content = scrollView;

			Device.StartTimer(new TimeSpan(0, 0, 0, 0, 500), () =>
			{
				AddCurrentPin(true);
				return false;
			});

		}

		private void AddCurrentPin(bool reverse)
		{
			if (mapView == null)
				return;
			mapView.Pins.Clear();
			mapView.Pins.Add(new Pin
			{
				Position = position,
				Label = "My Location"
			});
			if (reverse)
				ReverseAddress();
			else
			{
				autoComplete.SearchText = position.Latitude.ToString().Replace(",", ".") + ", " + position.Longitude.ToString().Replace(",", ".");
				address = autoComplete.SearchText;
			}
		}

		public async void GetCurrentLocation()
		{
			var locator = CrossGeolocator.Current;
			locator.DesiredAccuracy = 20;

			using (var dlg = UserDialogs.Instance.Loading(Shadow.Lang.AppResources.lblGettingLocation, null, "", true))
			{
				var gpspos = await locator.GetPositionAsync(15000);
				position = new Position(gpspos.Latitude, gpspos.Longitude);
				AddCurrentPin(true);
			};
		}

		private async void GetLatLng(string address)
		{
			Geocoder geoCoder;
			geoCoder = new Geocoder();
			using (var dlg = UserDialogs.Instance.Loading(Shadow.Lang.AppResources.lblGettingLocation, null, "", true))
			{
				var approximateLocations = await geoCoder.GetPositionsForAddressAsync(address);
				foreach (var gpspos in approximateLocations)
				{
					var currentLocation = new Position(gpspos.Latitude, gpspos.Longitude);
					mapView.MapRegion = MapSpan.FromCenterAndRadius(currentLocation, Distance.FromKilometers(5));
					mapView.Pins.Clear();
					mapView.Pins.Add(new Pin
					{
						Position = currentLocation,
						Label = "My Location"
					});
					break;
				}
			}
		}

		private async void ReverseAddress()
		{
			Geocoder geoCoder = new Geocoder();
			List<string> addresses = new List<string>();

			using (var dlg = UserDialogs.Instance.Loading(Shadow.Lang.AppResources.lblLookingUpAddress, null, "", true))
			{
				var possibleAddresses = await geoCoder.GetAddressesForPositionAsync(position);
				foreach (var possibleaddress in possibleAddresses)
				{
					addresses.Add(possibleaddress);
				}
			}
			if (addresses.Count == 0)
			{
				autoComplete.SearchText = position.Latitude.ToString().Replace(",", ".") + ", " + position.Longitude.ToString().Replace(",", ".");
				address = autoComplete.SearchText;
			}
			else if (addresses.Count == 1)
			{
				autoComplete.SearchText = addresses[0];
				address = autoComplete.SearchText;
				autoComplete.Unfocus();
			}
			else
			{
				var action = await Shadow.Data.Runtime.MainDisplayInstance.DisplayActionSheet("Choose Address", "Cancel", null, addresses.ToArray());
				if ((!string.IsNullOrEmpty(action)) && (action != "Cancel"))
				{
					autoComplete.SearchText = action;
					address = autoComplete.SearchText;
				}
			}
		}
			

	}
}


