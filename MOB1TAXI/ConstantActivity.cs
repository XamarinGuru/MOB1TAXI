
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace MOB1TAXI
{
	[Activity(Label = "ConstantActivity")]
	public class ConstantActivity : Activity, ILocationListener
	{
		public Location _currentLocation;
		LocationManager _locationManager;

		public string _locationProvider;
		public string _strAddress;

		public static ConstantActivity constantActivity;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			constantActivity = this;

			InitializeLocationManager();
		}

		void InitializeLocationManager()
		{
			_locationManager = (LocationManager)GetSystemService(LocationService);
			Criteria criteriaForLocationService = new Criteria
			{
				Accuracy = Accuracy.Fine
			};
			IList<string> acceptableLocationProviders = _locationManager.GetProviders(criteriaForLocationService, true);

			if (acceptableLocationProviders.Any())
			{
				_locationProvider = acceptableLocationProviders.First();
			}
			else
			{
				_locationProvider = string.Empty;
			}
		}

		protected override void OnResume()
		{
			base.OnResume();
			_locationManager.RequestLocationUpdates(_locationProvider, 0, 0, this);
		}

		protected override void OnPause()
		{
			base.OnPause();
			_locationManager.RemoveUpdates(this);
		}

		public void OnLocationChanged(Location location)
		{
			try
			{
				_currentLocation = location;

				if (_currentLocation == null)
					Log.Debug("current location = ", "Unable to determine your location. Try again in a short while.");
				else
				{
					Log.Debug("current location = ", string.Format("{0:f6},{1:f6}", _currentLocation.Latitude, _currentLocation.Longitude));

					Geocoder geocoder = new Geocoder(this);

					//The Geocoder class retrieves a list of address from Google over the internet
					IList<Address> addressList = geocoder.GetFromLocation(_currentLocation.Latitude, _currentLocation.Longitude, 10);

					Address address = addressList.FirstOrDefault();

					if (address != null)
					{
						StringBuilder deviceAddress = new StringBuilder();

						for (int i = 0; i < address.MaxAddressLineIndex; i++)
							deviceAddress.Append(address.GetAddressLine(i))
								.AppendLine(",");

						_strAddress = deviceAddress.ToString();
					}
					else
						_strAddress = "Unable to determine the address.";
				}
			}
			catch
			{
				_strAddress = "Unable to determine the address.";
			}
		}

		public void OnProviderDisabled(string provider)
		{

		}

		public void OnProviderEnabled(string provider)
		{

		}

		public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
		{

		}
	}
}
