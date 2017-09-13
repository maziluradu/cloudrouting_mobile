using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;
using static CLOUDROUTING.functions;

namespace CLOUDROUTING
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TripsInfoPage : ContentPage
	{
		public TripsInfoPage (Trip trip)
		{
			InitializeComponent ();
            NavigationPage.SetHasNavigationBar(this, false);
            var map = new TripsMap
            {
                MapType = MapType.Street,
                IsShowingUser = true,
                HeightRequest = App.Current.MainPage.Height,
                WidthRequest = App.Current.MainPage.Width,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };
            var relativeLayout = new RelativeLayout (); // Layout for Map + Frame
            var stackLayout = new StackLayout(); // Stack Layout for the information inside the frame
            var scrollView = new ScrollView { Content = stackLayout/*, Margin = new Thickness(0, 10, 0, 0)*/ }; // Scroller for the stack layout from inside the frame
            var frame = new Frame { BackgroundColor = Color.White, TranslationY = App.Current.MainPage.Height - (App.Current.MainPage.Height * 0.3), Content = scrollView }; // The frame itself
            relativeLayout.Children.Add(map, Constraint.Constant(0), Constraint.Constant(0), Constraint.RelativeToParent((parent) => { return parent.Width; }), Constraint.RelativeToParent((parent) => { return parent.Height; }));
            relativeLayout.Children.Add(frame, Constraint.RelativeToParent((parent) => { return parent.Width * 0.1; }), Constraint.RelativeToParent((parent) => { return parent.Height * 0.1; }), Constraint.RelativeToParent((parent) => { return parent.Width * 0.8; }), Constraint.RelativeToParent((parent) => { return parent.Height * 0.8; }));
            Content = relativeLayout;
            try
            {
                map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(Credentials.deviceLocation.Latitude, Credentials.deviceLocation.Longitude), Distance.FromMiles(0.3)));
                map.RouteCoordinates.Add(new Position(Credentials.deviceLocation.Latitude, Credentials.deviceLocation.Longitude));
                var cpin = new TripsPin { Pin = new Pin { Type = PinType.Place, Position = new Position(Credentials.deviceLocation.Longitude, Credentials.deviceLocation.Latitude), Address = "Current position", Label = "Current Position" }, Id = "CLOUDROUTING_0", Url = "http://maps.google.com" };
                map.Pins.Add(cpin.Pin);
                map.TripPins.Add(cpin);
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception: {0}", e);
            }
            var xButton = new Frame { Content = new Label { Text = "X", TextColor = Color.White, FontFamily = Device.OnPlatform(null, "latobold.ttf#Lato", null), HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center }, BackgroundColor = Color.Purple, CornerRadius = 15, WidthRequest = 30, HeightRequest = 30, HorizontalOptions = LayoutOptions.End, Padding = new Thickness(0, 0, 0, 0), Opacity = 0 };
            relativeLayout.Children.Add(xButton, Constraint.RelativeToParent((parent) => { return parent.Width * 0.9 - 15; }), Constraint.RelativeToParent((parent) => { return parent.Height * 0.1 - 15; }));
            map.TripPins = new List<TripsPin>();
            foreach (var order in Credentials.orderArray)
            {
                if (order.TripId == trip.Id)
                {
                    try
                    {
                        var cpin = new TripsPin { Pin = new Pin { Type = PinType.Place, Position = new Position(order.GivenX, order.GivenY), Address = order.StreetName + " nr. " + order.StreetNumber + System.Environment.NewLine + order.TimeWindowFrom + "-" + order.TimeWindowTill + System.Environment.NewLine + (order.Complete.Equals("true") ? "(complete)" : "(in progress)"), Label = "Order #" + order.Id }, Id = (order.Complete.Equals("true") ? "C" : "PC") + "LOUDROUTING_" + order.Id, Url = "http://maps.google.com" };
                        map.Pins.Add(cpin.Pin);
                        map.TripPins.Add(cpin);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Can't add new pin! Exception: {0}", e);
                    }
                    if (order.Complete.Equals("false")) map.RouteCoordinates.Add(new Position(order.GivenX, order.GivenY));
                    stackLayout.Children.Add(createMisc.CreateNewTripInfoForm(order, frame, xButton, map, trip/*, xOptimize*/));
                }
                //else Console.WriteLine("Order TripID #" + order.TripId + " is different from current Trip ID #" + trip.Id + ". The order was skipped.");
            }
            frame.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    await frame.TranslateTo(0, 0, 250, Easing.CubicOut);
                    await xButton.FadeTo(100, 250, Easing.CubicOut);
                    //xOptimize.FadeTo(100, 250, Easing.CubicOut);
                })
            });
            xButton.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    if (frame.TranslationY == 0)
                    {
                        await xButton.FadeTo(0, 250, Easing.CubicOut);
                        //await xOptimize.FadeTo(0, 250, Easing.CubicOut);
                        await frame.TranslateTo(0, App.Current.MainPage.Height - (App.Current.MainPage.Height * 0.3), 250, Easing.CubicOut);
                    }
                    else
                    {
                        await frame.TranslateTo(0, 0, 250, Easing.CubicOut);
                        await xButton.FadeTo(100, 250, Easing.CubicOut);
                        //xOptimize.FadeTo(100, 250, Easing.CubicOut);
                    }
                })
            });
            scrollView.Scrolled += delegate
            {
                if(frame.TranslationY != 0)
                {
                    frame.TranslateTo(0, 0, 250, Easing.CubicOut);
                    xButton.FadeTo(100, 250, Easing.CubicOut);
                    //xOptimize.FadeTo(100, 250, Easing.CubicOut);
                }
            };
        }
	}
}