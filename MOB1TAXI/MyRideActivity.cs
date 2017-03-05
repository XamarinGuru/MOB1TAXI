
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
	[Activity(Label = "MyRideActivity")]
	public class MyRideActivity : FragmentActivity, IOnMapReadyCallback
	{
		Button menu_but, cancel_ride_but, call_but, message_but, destination_but;
		TextView name_txt, address_txt, time_txt;
		RelativeLayout booking_layout;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.activity_myride);

			name_txt = FindViewById<TextView>(Resource.Id.name_txt);
			address_txt = FindViewById<TextView>(Resource.Id.address_txt);
			time_txt = FindViewById<TextView>(Resource.Id.time_txt);

			var menu = FindViewById<FlyOutContainer>(Resource.Id.FlyOutContainer);
			menu_but = FindViewById<Button>(Resource.Id.menu_but);
			menu_but.Click += (sender, e) =>
			{
				menu.AnimatedOpened = !menu.AnimatedOpened;
			};
			cancel_ride_but = FindViewById<Button>(Resource.Id.cancel_ride_but);
			cancel_ride_but.Click += delegate
			{
				Finish();
			};
			call_but = FindViewById<Button>(Resource.Id.call_but);
			call_but.Click += delegate
			{
				Toast.MakeText(this, "call button clicked!", ToastLength.Long).Show();
			};
			message_but = FindViewById<Button>(Resource.Id.message_but);
			message_but.Click += delegate
			{
				Toast.MakeText(this, "message button clicked!", ToastLength.Long).Show();
			};
			destination_but = FindViewById<Button>(Resource.Id.destination_but);
			destination_but.Click += delegate
			{
				Toast.MakeText(this, "destination button clicked!", ToastLength.Long).Show();
			};
			booking_layout = FindViewById<RelativeLayout>(Resource.Id.booking_layout);
			booking_layout.Click += delegate
			{
				Finish();
				StartActivity(new Intent(Application.Context, typeof(MapActivity)));
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
