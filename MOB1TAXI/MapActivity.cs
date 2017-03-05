
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Text;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Newtonsoft.Json;

namespace MOB1TAXI
{
	[Activity(Label = "MapActivity")]
	public class MapActivity : FragmentActivity, IOnMapReadyCallback
	{
		Button menu_but, pick_location_but, search_but, destination_but;
		AutoCompleteTextView search_txt;
		TextView result_txt;
		RelativeLayout contactus_layout;

#region by Jun
		const string strAutoCompleteGoogleApi = "https://maps.googleapis.com/maps/api/place/autocomplete/json?input=";
		const string strGoogleApiKey = "AIzaSyAiBwRUm_KZDv_sp3eI7F8hxkePqDTvY20";
		const string strGeoCodingUrl = "https://maps.googleapis.com/maps/api/geocode/json";

		GoogleMap map;
		ArrayAdapter adapter = null;
		GoogleMapPlaceClass objMapClass;
		GeoCodeJSONClass objGeoCodeJSONClass;
		string autoCompleteOptions;
		string[] strPredictiveText;
		int index = 0;
#endregion

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.activity_map);
			Window.SetSoftInputMode(SoftInput.StateAlwaysHidden);   //keyboard disabled

			search_txt = FindViewById<AutoCompleteTextView>(Resource.Id.search_txt);
			result_txt = FindViewById<TextView>(Resource.Id.result_txt);

			var menu = FindViewById<FlyOutContainer>(Resource.Id.FlyOutContainer);
			menu_but = FindViewById<Button>(Resource.Id.menu_but);
			menu_but.Click += (sender, e) =>
			{
				menu.AnimatedOpened = !menu.AnimatedOpened;
			};
			pick_location_but = FindViewById<Button>(Resource.Id.pick_location_but);
			pick_location_but.Click += delegate
			{
				StartActivity(new Intent(Application.Context, typeof(PickActivity)));
			};
			search_but = FindViewById<Button>(Resource.Id.search_but);
			search_but.Click += delegate
			{
				if (search_txt.Text.Length != 0)
				{
					result_txt.Text = search_txt.Text;
				}
			};
			destination_but = FindViewById<Button>(Resource.Id.destination_but);
			contactus_layout = FindViewById<RelativeLayout>(Resource.Id.contactus_layout);
			contactus_layout.Click += delegate
			{
				Finish();
				StartActivity(new Intent(Application.Context, typeof(MyRideActivity)));
			};

			var mapFragment =
				(SupportMapFragment)SupportFragmentManager.FindFragmentById(Resource.Id.map);
			mapFragment.GetMapAsync(this);

			#region by Jun
			search_txt.ItemClick += SelectedAutoCompleteList;
			search_txt.TextChanged += ChangedSearchText;
			#endregion
		}

#region
		private CameraPositionlHandler _cameraPositionHandler;
#endregion
		public void OnMapReady(GoogleMap googleMap)
		{
			#region by Jun
			map = googleMap;
			map.CameraChange += OnCameraChanged;
			_cameraPositionHandler = new CameraPositionlHandler(map, this);
			#endregion

			googleMap.UiSettings.ZoomControlsEnabled = false;
			googleMap.MyLocationEnabled = false;

			LoginActivity activity = LoginActivity.loginActivity;
			if (activity._currentLocation != null)
			{
				LatLng curLatLng = new LatLng(activity._currentLocation.Latitude, activity._currentLocation.Longitude);
				//googleMap.AddMarker(new MarkerOptions()
				//					.SetPosition(curLatLng)
				//					.SetTitle(activity._strAddress));
				googleMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(curLatLng, 12));
			}
		}




		#region by Jun

		void OnCameraChanged(object sender, GoogleMap.CameraChangeEventArgs e)
		{
			_cameraPositionHandler.RemoveMessages(1);
			_cameraPositionHandler.RemoveMessages(2);
			_cameraPositionHandler.SendEmptyMessageDelayed(1, 300);
			_cameraPositionHandler.SendEmptyMessageDelayed(2, 600);
		}
		public async void DragMapPinProcess(CameraPosition cameraPos)
		{
			try
			{
				var location = cameraPos.Target;
				var geo = new Geocoder(this);

				var addresses = await geo.GetFromLocationAsync(location.Latitude, location.Longitude, 1);

				Address addressList = addresses[0];

				string formatedAddress = "";

				for (var i = 0; i < addressList.MaxAddressLineIndex; i++)
				{
					formatedAddress += addressList.GetAddressLine(i) + " ";
				}
				search_txt.Text = formatedAddress;
			}
			catch
			{
			}
		}

		private class CameraPositionlHandler : Handler
		{
			private CameraPosition _lastCameraPosition;
			private GoogleMap _googleMap;
			private MapActivity rootVC;

			public CameraPositionlHandler(GoogleMap googleMap, MapActivity rootVC)
			{
				_googleMap = googleMap;
				this.rootVC = rootVC;
			}

			public override void HandleMessage(Message msg)
			{
				if (_googleMap != null)
				{
					if (msg.What == 1)
					{
						_lastCameraPosition = _googleMap.CameraPosition;
					}
					else if (msg.What == 2)
					{
						if (_lastCameraPosition.Equals(_googleMap.CameraPosition))
						{
							rootVC.DragMapPinProcess(_lastCameraPosition);
						}
					}
				}
			}
		}

		async void ChangedSearchText(object sender, TextChangedEventArgs e)
		{
			try
			{
				autoCompleteOptions = await fnDownloadString(strAutoCompleteGoogleApi + search_txt.Text + "&key=" + strGoogleApiKey);
				if (autoCompleteOptions == "Exception")
				{
					Toast.MakeText(this, "Unable to connect to server!!!", ToastLength.Short).Show();
					return;
				}
				objMapClass = JsonConvert.DeserializeObject<GoogleMapPlaceClass>(autoCompleteOptions);
				Console.WriteLine(objMapClass.status);

				strPredictiveText = new string[objMapClass.predictions.Count];
				index = 0;
				foreach (Prediction objPred in objMapClass.predictions)
				{
					strPredictiveText[index] = objPred.description;
					index++;
				}
				adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleDropDownItem1Line, strPredictiveText);
				search_txt.Adapter = adapter;
			}
			catch
			{
				Toast.MakeText(this, "Unable to process at this moment!!!", ToastLength.Short).Show();
			}
		}

		async void SelectedAutoCompleteList(object sender, AdapterView.ItemClickEventArgs e)
		{
			//to soft keyboard hide
			InputMethodManager inputManager = (InputMethodManager)this.GetSystemService(Context.InputMethodService);
			inputManager.HideSoftInputFromWindow(search_txt.WindowToken, HideSoftInputFlags.NotAlways);
			map.Clear();
			if (search_txt.Text != string.Empty)
			{
				var sb = new StringBuilder();
				sb.Append(strGeoCodingUrl);
				sb.Append("?address=").Append(search_txt.Text);
				string strResult = await fnDownloadString(sb.ToString());
				if (strResult == "Exception")
				{
					Toast.MakeText(this, "Unable to connect to server!!!", ToastLength.Short).Show();

				}
				//below used single quote to avoid html interpretation
				objGeoCodeJSONClass = JsonConvert.DeserializeObject<GeoCodeJSONClass>(strResult);
				LatLng Position = new LatLng(objGeoCodeJSONClass.results[0].geometry.location.lat, objGeoCodeJSONClass.results[0].geometry.location.lng);
				updateCameraPosition(Position);
				//MarkOnMap("MyLocation", Position);
			}
		}

		void MarkOnMap(string title, LatLng pos)
		{
			RunOnUiThread(() =>
		  {
			  var marker = new MarkerOptions();
			  marker.SetTitle(title);
			  marker.SetPosition(pos);
			  map.AddMarker(marker);
		  });
		}
		void updateCameraPosition(LatLng pos)
		{
			CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
			builder.Target(pos);
			builder.Zoom(14);
			builder.Bearing(45);
			builder.Tilt(90);
			CameraPosition cameraPosition = builder.Build();
			CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
			map.AnimateCamera(cameraUpdate);
		}
		async Task<string> fnDownloadString(string strUri)
		{
			WebClient webclient = new WebClient();
			string strResultData;
			try
			{
				strResultData = await webclient.DownloadStringTaskAsync(new Uri(strUri));
				Console.WriteLine(strResultData);
			}
			catch
			{
				strResultData = "Exception";
				RunOnUiThread(() =>
			  {
				  Toast.MakeText(this, "Unable to connect to server!!!", ToastLength.Short).Show();
			  });
			}
			finally
			{
				webclient.Dispose();
				webclient = null;
			}

			return strResultData;
		}




		//GeoCodeJsonClass
		public class AddressComponent
		{
			public string long_name { get; set; }
			public string short_name { get; set; }
			public List<string> types { get; set; }
		}

		public class Northeast
		{
			public double lat { get; set; }
			public double lng { get; set; }
		}

		public class Southwest
		{
			public double lat { get; set; }
			public double lng { get; set; }
		}

		public class Bounds
		{
			public Northeast northeast { get; set; }
			public Southwest southwest { get; set; }
		}

		public class Location
		{
			public double lat { get; set; }
			public double lng { get; set; }
		}

		public class Northeast2
		{
			public double lat { get; set; }
			public double lng { get; set; }
		}

		public class Southwest2
		{
			public double lat { get; set; }
			public double lng { get; set; }
		}

		public class Viewport
		{
			public Northeast2 northeast { get; set; }
			public Southwest2 southwest { get; set; }
		}

		public class Geometry
		{
			public Bounds bounds { get; set; }
			public Location location { get; set; }
			public string location_type { get; set; }
			public Viewport viewport { get; set; }
		}

		public class Result
		{
			public List<AddressComponent> address_components { get; set; }
			public string formatted_address { get; set; }
			public Geometry geometry { get; set; }
			public string place_id { get; set; }
			public List<string> types { get; set; }
		}

		public class GeoCodeJSONClass
		{
			public List<Result> results { get; set; }
			public string status { get; set; }
		}

		//GoogleMapPlaceClass
		public class Prediction
		{
			public string description { get; set; }
			public string id { get; set; }
			public List<MatchedSubstring> matched_substrings { get; set; }
			public string place_id { get; set; }
			public string reference { get; set; }
			public List<Term> terms { get; set; }
			public List<string> types { get; set; }
		}

		public class GoogleMapPlaceClass
		{
			public List<Prediction> predictions { get; set; }
			public string status { get; set; }
		}
		public class MatchedSubstring
		{
			public int length { get; set; }
			public int offset { get; set; }
		}

		public class Term
		{
			public int offset { get; set; }
			public string value { get; set; }
		}
		#endregion
	}
}
