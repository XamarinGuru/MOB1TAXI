
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
	[Activity(Label = "LoginActivity")]
	public class LoginActivity : Activity, ILocationListener
	{
		Button login_but, register_but, facebook_login_but, forgotpassword_but;

		public Location _currentLocation;
		LocationManager _locationManager;

		public string _locationProvider;
		public string _strAddress;

		public static LoginActivity loginActivity;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.activity_login);

			Window.SetSoftInputMode(SoftInput.StateAlwaysHidden);   //keyboard disabled

			register_but = FindViewById<Button>(Resource.Id.register_but);
			register_but.Click += delegate {
				Finish();
				StartActivity(new Intent(Application.Context, typeof(RegisterActivity)));
			};
			login_but = FindViewById<Button>(Resource.Id.app_login_but);
			login_but.Click += delegate
			{
				StartActivity(new Intent(Application.Context, typeof(MapActivity)));
			};
			facebook_login_but = FindViewById<Button>(Resource.Id.facebook_login_but);
			facebook_login_but.Click += delegate
			{
				//StartActivity(new Intent(Application.Context, typeof(MyRideActivity)));
			};
			forgotpassword_but = FindViewById<Button>(Resource.Id.forgotpassword_but);
			forgotpassword_but.Click += delegate
			{
				Toast.MakeText(this, "forgot password button clicked!", ToastLength.Long).Show();
			};

			loginActivity = this;

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
