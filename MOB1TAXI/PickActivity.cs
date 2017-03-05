
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

namespace MOB1TAXI
{
	[Activity(Label = "PickActivity")]
	public class PickActivity : FragmentActivity, IOnMapReadyCallback
	{
		Button back_but, fare_estimate_but, promo_code_but, request_ride_but, plus_but, destination_but, remove_but;
		TextView result_txt, time_txt, cost_txt;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.activity_pick);

			result_txt = FindViewById<TextView>(Resource.Id.result_txt);
			time_txt = FindViewById<TextView>(Resource.Id.time_txt);
			cost_txt = FindViewById<TextView>(Resource.Id.cost_txt);

			back_but = FindViewById<Button>(Resource.Id.back_but);
			back_but.Click += delegate
			{
				Finish();
			};
			fare_estimate_but = FindViewById<Button>(Resource.Id.fare_estimate_but);
			fare_estimate_but.Click += delegate
			{
				Toast.MakeText(this, "fare estimate button clicked!", ToastLength.Long).Show();
			};
			promo_code_but = FindViewById<Button>(Resource.Id.promo_code_but);
			promo_code_but.Click += delegate
			{
				Toast.MakeText(this, "promo code button clicked!", ToastLength.Long).Show();
			};
			request_ride_but = FindViewById<Button>(Resource.Id.request_ride_but);
			request_ride_but.Click += delegate
			{
				Toast.MakeText(this, "request ride button clicked!", ToastLength.Long).Show();
			};
			plus_but = FindViewById<Button>(Resource.Id.plus_but);
			plus_but.Click += delegate
			{
				Toast.MakeText(this, "picked this location!", ToastLength.Long).Show();
			};
			remove_but = FindViewById<Button>(Resource.Id.remove_but);
			remove_but.Click += delegate
			{
				Toast.MakeText(this, "remove button clicked!", ToastLength.Long).Show();
			};
			destination_but = FindViewById<Button>(Resource.Id.destination_but);
			destination_but.Click += delegate
			{
				Toast.MakeText(this, "destination button clicked!", ToastLength.Long).Show();
			};
			var mapFragment =
				(SupportMapFragment)SupportFragmentManager.FindFragmentById(Resource.Id.map);
			mapFragment.GetMapAsync(this);
		}

		public void OnMapReady(GoogleMap googleMap)
		{
			googleMap.UiSettings.ZoomControlsEnabled = false;
			googleMap.MyLocationEnabled = true;

			LoginActivity activity = LoginActivity.loginActivity;
			if (activity._currentLocation != null)
			{
				LatLng curLatLng = new LatLng(activity._currentLocation.Latitude, activity._currentLocation.Longitude);
				googleMap.AddMarker(new MarkerOptions()
									.SetPosition(curLatLng)
									.SetTitle(activity._strAddress));
				googleMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(curLatLng, 12));
			}
		}
	}
}
