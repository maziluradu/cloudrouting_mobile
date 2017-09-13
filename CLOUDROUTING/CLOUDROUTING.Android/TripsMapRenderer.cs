using System.Collections.Generic;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using CLOUDROUTING;
using CLOUDROUTING.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;
using System;
using System.ComponentModel;
using Android.Widget;
using Android.Content;
using Android.Graphics;

[assembly: ExportRenderer(typeof(TripsMap), typeof(TripsMapRenderer))]
namespace CLOUDROUTING.Droid
{
    class TripsMapRenderer : MapRenderer, IOnMapReadyCallback, GoogleMap.IInfoWindowAdapter
    {
        GoogleMap map;
        List<Position> routeCoordinates;
        List<TripsPin> customPins = new List<TripsPin>();
        bool isDrawn;
        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Xamarin.Forms.Maps.Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                NativeMap.InfoWindowClick -= OnInfoWindowClick;
            }

            if (e.NewElement != null)
            {
                var formsMap = (TripsMap)e.NewElement;
                routeCoordinates = formsMap.RouteCoordinates;
                customPins = formsMap.TripPins;
                Console.WriteLine("formsMap.TripPins is " + ((formsMap.TripPins == null) ? "null" : "not null"));

                Control.GetMapAsync(this);
            }
        }
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName.Equals("VisibleRegion") && !isDrawn)
            {
                NativeMap.Clear();
                NativeMap.InfoWindowClick += OnInfoWindowClick;
                NativeMap.SetInfoWindowAdapter(this);

                var polylineOptions = new PolylineOptions();
                polylineOptions.InvokeColor(Android.Graphics.Color.Argb(255, 0, 89, 178).ToArgb());
                polylineOptions.InvokeStartCap(new SquareCap());
                

                foreach (var position in routeCoordinates)
                {
                    polylineOptions.Add(new LatLng(position.Latitude, position.Longitude));
                }

                NativeMap.AddPolyline(polylineOptions);

                foreach (var pin in customPins)
                {
                    try
                    {
                        var marker = new MarkerOptions();
                        //marker.
                        marker.SetPosition(new LatLng(pin.Pin.Position.Latitude, pin.Pin.Position.Longitude));
                        marker.SetTitle(pin.Pin.Label);
                        marker.SetSnippet(pin.Pin.Address);
                        //if(pin.Id.StartsWith("P")) marker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.tripsPin));
                        //else marker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.tripsPinComplete));

                        NativeMap.AddMarker(marker);
                    }
                    catch(Exception exc)
                    {
                        Console.WriteLine("Custom pin exception: {0}", exc);
                    }
                }
                isDrawn = true;
            }
            if(e.PropertyName.Equals("HasZoomEnabled"))
            {
                //Console.WriteLine("Test test");
                NativeMap.Clear();
                isDrawn = false;
            }
        }
        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);

            if (changed)
            {
                isDrawn = false;
            }
        }

        void OnInfoWindowClick(object sender, GoogleMap.InfoWindowClickEventArgs e)
        {
            var customPin = GetCustomPin(e.Marker);
            if (customPin == null)
            {
                throw new Exception("Custom pin not found");
            }

            if (!string.IsNullOrWhiteSpace(customPin.Url))
            {
                var url = Android.Net.Uri.Parse(customPin.Url);
                var intent = new Intent(Intent.ActionView, url);
                intent.AddFlags(ActivityFlags.NewTask);
                Android.App.Application.Context.StartActivity(intent);
            }
        }

        public Android.Views.View GetInfoContents(Marker marker)
        {
            var inflater = Android.App.Application.Context.GetSystemService(Context.LayoutInflaterService) as Android.Views.LayoutInflater;
            if (inflater != null)
            {
                Android.Views.View view;

                var customPin = GetCustomPin(marker);
                if (customPin == null)
                {
                    throw new Exception("Custom pin not found");
                }

                view = inflater.Inflate(Resource.Layout.MapInfoWindow, null);

                var infoTitle = view.FindViewById<TextView>(Resource.Id.InfoWindowTitle);
                var infoSubtitle = view.FindViewById<TextView>(Resource.Id.InfoWindowSubtitle);

                if (infoTitle != null)
                {
                    infoTitle.Text = marker.Title;
                }
                if (infoSubtitle != null)
                {
                    infoSubtitle.Text = marker.Snippet;
                }

                return view;
            }
            return null;
        }

        public Android.Views.View GetInfoWindow(Marker marker)
        {
            return null;
        }

        TripsPin GetCustomPin(Marker annotation)
        {
            var position = new Position(annotation.Position.Latitude, annotation.Position.Longitude);
            foreach (var pin in customPins)
            {
                if (pin.Pin.Position == position)
                {
                    return pin;
                }
            }
            return null;
        }
        public void OnMapReady(GoogleMap googleMap)
        {
            InvokeOnMapReadyBaseClassHack(googleMap);
            map = googleMap;
            //map.SetMapStyle(GetCurrentMapStyle());
            map.UiSettings.ZoomControlsEnabled = false;
        }
        private void InvokeOnMapReadyBaseClassHack(GoogleMap googleMap)
        {
            System.Reflection.MethodInfo onMapReadyMethodInfo = null;

            Type baseType = typeof(MapRenderer);
            foreach (var currentMethod in baseType.GetMethods(System.Reflection.BindingFlags.NonPublic |
                                                             System.Reflection.BindingFlags.Instance |
                                                              System.Reflection.BindingFlags.DeclaredOnly))
            {

                if (currentMethod.IsFinal && currentMethod.IsPrivate)
                {
                    if (string.Equals(currentMethod.Name, "OnMapReady", StringComparison.Ordinal))
                    {
                        onMapReadyMethodInfo = currentMethod;

                        break;
                    }

                    if (currentMethod.Name.EndsWith(".OnMapReady", StringComparison.Ordinal))
                    {
                        onMapReadyMethodInfo = currentMethod;

                        break;
                    }
                }
            }

            if (onMapReadyMethodInfo != null)
            {
                onMapReadyMethodInfo.Invoke(this, new[] { googleMap });
            }
        }
        public MapStyleOptions GetCurrentMapStyle()
        {
            MapStyleOptions style = new MapStyleOptions(
            "[" +
            "    {" +
            "        \"featureType\": \"administrative.locality\"," +
            "        \"elementType\": \"all\"," +
            "        \"stylers\": [" +
            "            {" +
            "                \"hue\": \"#2c2e33\"" +
            "            }," +
            "            {" +
            "                \"saturation\": 7" +
            "            }," +
            "            {" +
            "                \"lightness\": 19" +
            "            }," +
            "            {" +
            "                \"visibility\": \"on\"" +
            "            }" +
            "        ]" +
            "    }," +
            "    {" +
            "        \"featureType\": \"landscape\"," +
            "        \"elementType\": \"all\"," +
            "        \"stylers\": [" +
            "            {" +
            "                \"hue\": \"#ffffff\"" +
            "            }," +
            "            {" +
            "                \"saturation\": -100" +
            "            }," +
            "            {" +
            "                \"lightness\": 100" +
            "            }," +
            "            {" +
            "                \"visibility\": \"simplified\"" +
            "            }" +
            "        ]" +
            "    }," +
            "    {" +
            "        \"featureType\": \"poi\"," +
            "        \"elementType\": \"all\"," +
            "        \"stylers\": [" +
            "            {" +
            "                \"hue\": \"#ffffff\"" +
            "            }," +
            "            {" +
            "                \"saturation\": -100" +
            "            }," +
            "            {" +
            "                \"lightness\": 100" +
            "            }," +
            "            {" +
            "                \"visibility\": \"off\"" +
            "            }" +
            "        ]" +
            "    }," +
            "    {" +
            "        \"featureType\": \"road\"," +
            "        \"elementType\": \"geometry\"," +
            "        \"stylers\": [" +
            "            {" +
            "                \"hue\": \"#bbc0c4\"" +
            "            }," +
            "            {" +
            "                \"saturation\": -93" +
            "            }," +
            "            {" +
            "                \"lightness\": 31" +
            "            }," +
            "            {" +
            "                \"visibility\": \"simplified\"" +
            "            }" +
            "        ]" +
            "    }," +
            "    {" +
            "        \"featureType\": \"road\"," +
            "        \"elementType\": \"labels\"," +
            "        \"stylers\": [" +
            "            {" +
            "                \"hue\": \"#bbc0c4\"" +
            "            }," +
            "            {" +
            "                \"saturation\": -93" +
            "            }," +
            "            {" +
            "                \"lightness\": 31" +
            "            }," +
            "            {" +
            "                \"visibility\": \"on\"" +
            "            }" +
            "        ]" +
            "    }," +
            "    {" +
            "        \"featureType\": \"road.arterial\"," +
            "        \"elementType\": \"labels\"," +
            "        \"stylers\": [" +
            "            {" +
            "                \"hue\": \"#bbc0c4\"" +
            "            }," +
            "            {" +
            "                \"saturation\": -93" +
            "            }," +
            "            {" +
            "                \"lightness\": -2" +
            "            }," +
            "            {" +
            "                \"visibility\": \"simplified\"" +
            "            }" +
            "        ]" +
            "    }," +
            "    {" +
            "        \"featureType\": \"road.local\"," +
            "        \"elementType\": \"geometry\"," +
            "        \"stylers\": [" +
            "            {" +
            "                \"hue\": \"#e9ebed\"" +
            "            }," +
            "            {" +
            "                \"saturation\": -90" +
            "            }," +
            "            {" +
            "                \"lightness\": -8" +
            "            }," +
            "            {" +
            "                \"visibility\": \"simplified\"" +
            "            }" +
            "        ]" +
            "    }," +
            "    {" +
            "        \"featureType\": \"transit\"," +
            "        \"elementType\": \"all\"," +
            "        \"stylers\": [" +
            "            {" +
            "                \"hue\": \"#e9ebed\"" +
            "            }," +
            "            {" +
            "                \"saturation\": 10" +
            "            }," +
            "            {" +
            "                \"lightness\": 69" +
            "            }," +
            "            {" +
            "                \"visibility\": \"on\"" +
            "            }" +
            "        ]" +
            "    }," +
            "    {" +
            "        \"featureType\": \"water\"," +
            "        \"elementType\": \"all\"," +
            "        \"stylers\": [" +
            "            {" +
            "                \"hue\": \"#e9ebed\"" +
            "            }," +
            "            {" +
            "                \"saturation\": -78" +
            "            }," +
            "            {" +
            "                \"lightness\": 67" +
            "            }," +
            "            {" +
            "                \"visibility\": \"simplified\"" +
            "            }" +
            "        ]" +
            "    }" +
            "]  "
            );
            return style;
        }
    }
}