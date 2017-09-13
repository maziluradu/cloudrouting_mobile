using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Xamarin.Forms.Maps;

namespace CLOUDROUTING
{
    public class functions
    {
        public static class Credentials
        {
            public static string customerKey { get; set; }
            public static string driverId { get; set; }
            public static string cookieValue { get; set; }
            public static string apibaseUrl { get; set; }
            public static List<Order> orderArray { get; set; }
            public static List<Trip> tripsArray { get; set; }
            public static Plugin.Geolocator.Abstractions.Position deviceLocation { get; set; }
        }
        public class Order
        {
            public int Id { get; set; }
            public string OrderNumber { get; set; }
            public string StreetName { get; set; }
            public string StreetNumber { get; set; }
            public string CityName { get; set; }
            public string CountryCode { get; set; }
            public float GivenX { get; set; }
            public float GivenY { get; set; }
            public float GeoX { get; set; }
            public float GeoY { get; set; }
            public string OrderType { get; set; }
            public int FixedDurationInSec { get; set; }
            public string TimeWindowFrom { get; set; }
            public string TimeWindowTill { get; set; }
            public string AccountId { get; set; }
            public string DriverId { get; set; }
            public int TripId { get; set; }
            public string StopStartTime { get; set; }
            public string StopFinishTime { get; set; }
            public int StopDurationInSec { get; set; }
            public string Complete { get; set; }
            public string Comment { get; set; }
        }
        public class Trip
        {
            public int Id { get; set; }
            public string AccountId { get; set; }
            public string DriverId { get; set; }
            public DateTime AvailableFromTime { get; set; }
            public DateTime AvailableTillTime { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime FinishTime { get; set; }
            public float TotalDistanceInKm { get; set; }
            public int TotalDurationInSec { get; set; }
            public int NOfStops { get; set; }
            public int Finished { get; set; }
        }
        public class ClientFunctions
        {
            public static ClientFunctions Instance = new ClientFunctions();
            public HttpClientHandler handler;
            public HttpClient client;

            public ClientFunctions()
            {
                handler = new HttpClientHandler { UseCookies = false };
                client = new HttpClient(handler);
                client.Timeout = TimeSpan.FromSeconds(10);
                //Credentials.apibaseUrl = "http://192.168.0.102:1337/api"; // HOME ADDRESS
                Credentials.apibaseUrl = "http://172.20.31.56:1337/api"; // ORTEC ADDRESS
                //Credentials.apibaseUrl = "http://cloudroutingortec.azurewebsites.net/api"; // AZURE ADDRESS
            }

            public async Task<int> GetDeviceLocation()
            {
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 50;
                Credentials.deviceLocation = await locator.GetPositionAsync(TimeSpan.FromSeconds(10));
                return 0;
            }
            public async Task<int> cr_TriggerPage(int page, ActivityIndicator loading = null)
            {   // MAIN PAGE: 0, MENU PAGE: 1, DRIVER PAGE: 2, TRIPS PAGE: 3, ORDERS PAGE: 4, LOGOUT: 5
                switch(page)
                {
                    case 1:
                    {
                        if (loading != null) { loading.IsEnabled = true; loading.Opacity = 1; loading.IsRunning = true; }
                        await App.Current.MainPage.Navigation.PushAsync(new MenuPage());
                        if (loading != null) { loading.IsEnabled = false; loading.Opacity = 0; loading.IsRunning = false; }
                        break;
                    }
                    case 2:
                    {
                        string url = Credentials.apibaseUrl + "/driver/trips";
                        if (loading != null) { loading.IsEnabled = true; loading.Opacity = 1; loading.IsRunning = true; }
                        Credentials.tripsArray = await Instance.TripsRequestAsync(url);
                        await App.Current.MainPage.Navigation.PushAsync(new DriverPage());
                        if (loading != null) { loading.IsEnabled = false; loading.Opacity = 0; loading.IsRunning = false; }
                        break;
                    }
                    case 3:
                    {
                        string url = Credentials.apibaseUrl + "/driver/trips";
                        if (loading != null) { loading.IsEnabled = true; loading.Opacity = 1; loading.IsRunning = true; }
                        Credentials.tripsArray = await Instance.TripsRequestAsync(url);

                        if (Credentials.tripsArray.Count == 0) App.Current.MainPage.DisplayAlert("No trips!", "There are currently no trips logged.", "OK");
                        else
                        {
                            if (Credentials.tripsArray.Count > 1) await App.Current.MainPage.Navigation.PushAsync(new TripsPage());
                            else
                            {
                                foreach(var trip in Credentials.tripsArray)
                                {
                                    Console.WriteLine("Only one trip found! ID: " + trip.Id);
                                    Credentials.orderArray = await ClientFunctions.Instance.OrdersRequestAsync(Credentials.apibaseUrl + "/routing/orders/" + trip.Id);
                                    Credentials.orderArray = Credentials.orderArray.OrderBy(o => Int32.Parse(o.OrderNumber)).ToList().OrderBy(o => o.Complete).ToList();
                                    ClientFunctions.ModifyOrders(Credentials.orderArray);
                                    App.Current.MainPage.Navigation.PushAsync(new TripsInfoPage(trip));
                                    break;
                                }
                            }
                        }
                        if (loading != null) { loading.IsEnabled = false; loading.Opacity = 0; loading.IsRunning = false; }
                        break;
                    }
                    case 4:
                    {
                        string url = Credentials.apibaseUrl + "/driver/orders";
                        if (loading != null) { loading.IsEnabled = true; loading.Opacity = 1; loading.IsRunning = true; }
                        Credentials.orderArray = await Instance.OrdersRequestAsync(url);
                        Credentials.orderArray = Credentials.orderArray.OrderBy(o => Int32.Parse(o.OrderNumber)).ToList().OrderBy(o => o.Complete).ToList();
                        ClientFunctions.ModifyOrders(Credentials.orderArray);
                        if (Credentials.orderArray.Count == 0) App.Current.MainPage.DisplayAlert("No available orders!", "There are currently no available orders.", "OK");
                        else
                        {
                            await App.Current.MainPage.Navigation.PushAsync(new OrdersPage());
                        }
                        if (loading != null) { loading.IsEnabled = false; loading.Opacity = 0; loading.IsRunning = false; }
                        break;
                    }
                    case 5:
                    {
                        string url = Credentials.apibaseUrl + "/login/logout";
                        if (loading != null) { loading.IsEnabled = true; loading.Opacity = 1; loading.IsRunning = true; }
                        await Task.Delay(1000);
                        string json = await Instance.LogoutRequestAsync(url);

                        if (json == null) App.Current.MainPage.DisplayAlert("Logout failed!", "Your logout request failed. You were either not logged in or a server error occured.", "OK");
                        else
                        {
                            await App.Current.MainPage.Navigation.PopAsync();
                        }
                        if (loading != null) loading.IsEnabled = false; loading.Opacity = 0; loading.IsRunning = false;
                        break;
                    }
                }
                return 0;
            }
            public static void ModifyOrders(List<Order> orders)
            {
                DateTime tempdt;
                foreach(var order in orders)
                {
                    tempdt = DateTime.Parse(order.TimeWindowFrom);
                    order.TimeWindowFrom = (tempdt.Hour < 10 ? "0" : "") + tempdt.Hour + ":" + (tempdt.Minute < 10 ? "0" : "") + tempdt.Minute;
                    tempdt = DateTime.Parse(order.TimeWindowTill);
                    order.TimeWindowTill = (tempdt.Hour < 10 ? "0" : "") + tempdt.Hour + ":" + (tempdt.Minute < 10 ? "0" : "") + tempdt.Minute;
                }
            }
            public static void AddPageChangeFunction(Label text, Image next, Image icon, int page, ActivityIndicator loading = null)
            {
                text.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(async () =>
                    {
                        await icon.TranslateTo(6, 0, 75, Easing.Linear);
                        await Instance.cr_TriggerPage(page, loading);
                        icon.TranslationX = 0;
                    })
                });
                next.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(async () =>
                    {
                        await icon.TranslateTo(6, 0, 75, Easing.Linear);
                        await Instance.cr_TriggerPage(page, loading);
                        icon.TranslationX = 0;
                    })
                });
                icon.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(async () =>
                    {
                        await icon.TranslateTo(6, 0, 75, Easing.Linear);
                        await Instance.cr_TriggerPage(page, loading);
                        icon.TranslationX = 0;
                    })
                });
            }
            public async Task<string> LoginRequestAsync(string url)
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                var content = new StringContent(JsonConvert.SerializeObject(new { customerKey = Credentials.customerKey, driverId = Credentials.driverId }), Encoding.UTF8, "application/json");
                var response = new HttpResponseMessage();
                try
                {
                    response = await client.PostAsync(url, content);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception is: {0}", e);
                    return null;
                }
                Console.WriteLine("Connection set! Status code: {0}", response.StatusCode);
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var resultJson = response.Content.ReadAsStringAsync().Result;
                        Credentials.cookieValue = response.Headers.GetValues("Set-Cookie").First();
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("Exception is (2): {0}", ex);
                    }
                    return "1";
                }
                return null;
            }
            public async Task<string> SetOrderCompleteAsync(string url, int oid, string ocomplete)
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                var content = new StringContent(JsonConvert.SerializeObject(new { orderId = oid, isComplete = ocomplete }), Encoding.UTF8, "application/json");
                var response = new HttpResponseMessage();
                response = await client.PostAsync(url, content);
                if (response.IsSuccessStatusCode) return true.ToString();
                return false.ToString();
            }
            public async Task<string> LogoutRequestAsync(string url)
            {
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var resultJson = response.Content.ReadAsStringAsync().Result;
                    return resultJson;
                }
                return "0";
            }
            public async Task<List<Order>> OrdersRequestAsync(string url)
            {
                client.DefaultRequestHeaders.Add("Cookie", Credentials.cookieValue);
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var resultJson = response.Content.ReadAsStringAsync().Result;
                    Credentials.orderArray = JsonConvert.DeserializeObject<List<Order>>(resultJson);
                    Credentials.orderArray = Credentials.orderArray.OrderBy(o => Int32.Parse(o.OrderNumber)).ToList().OrderBy(o => o.Complete).ToList();
                    ClientFunctions.ModifyOrders(Credentials.orderArray);
                }
                return Credentials.orderArray;
            }
            public async Task<List<Trip>> TripsRequestAsync(string url)
            {
                client.DefaultRequestHeaders.Add("Cookie", Credentials.cookieValue);
                //var content = new StringContent(JsonConvert.SerializeObject(new { customerKey = Credentials.customerKey, driverId = Credentials.driverId }), Encoding.UTF8, "application/json");
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var resultJson = response.Content.ReadAsStringAsync().Result;
                    Credentials.tripsArray = JsonConvert.DeserializeObject<List<Trip>>(resultJson);
                }
                return Credentials.tripsArray;
            }
            public async Task<HttpResponseMessage> OrdersTriggerAsync(string url)
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                var content = new StringContent(JsonConvert.SerializeObject(new { customerKey = Credentials.customerKey, driverId = Credentials.driverId }), Encoding.UTF8, "application/json");
                var response = new HttpResponseMessage();
                try
                {
                    response = await client.PostAsync(url, content);
                }
                catch (Exception e)
                {
                    Console.WriteLine("(OrderTrigger) Exception is (1): {0}", e);
                    return null;
                }
                return response;
            }
            public async Task<List<Order>> OrdersOptimizeAsync(string url)
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                var content = new StringContent(JsonConvert.SerializeObject(new { customerKey = Credentials.customerKey, driverId = Credentials.driverId }), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    var resultJson = response.Content.ReadAsStringAsync().Result;
                    Credentials.orderArray = JsonConvert.DeserializeObject<List<Order>>(resultJson);
                    Credentials.orderArray = Credentials.orderArray.OrderBy(o => Int32.Parse(o.OrderNumber)).ToList().OrderBy(o => o.Complete).ToList();
                    ClientFunctions.ModifyOrders(Credentials.orderArray);
                }
                return Credentials.orderArray;
            }
        }
        public class createMisc
        {
            public static Frame CreateNewTripsForm(Trip trip, bool inProgress)
            {
                Grid grid = new Grid
                {
                    RowDefinitions =
                    {
                        new RowDefinition { Height = new GridLength(3, GridUnitType.Star) },
                        new RowDefinition { Height = new GridLength(2, GridUnitType.Star) },
                        new RowDefinition { Height = new GridLength(2, GridUnitType.Star) }
                    },
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                    },
                };
                grid.Children.Add(new Label { Text = "TRIP #" + trip.Id + ((inProgress == true) ? " (In progress)" : " (Finished)"), FontFamily = Device.OnPlatform(null, "latoblack.ttf#Lato Black", null), FontSize = 16, TextColor = Color.White, HorizontalTextAlignment = TextAlignment.Start, Margin = new Thickness(0, 0, 0, 10) }, 0, 0);
                grid.Children.Add(new Label { Text = "Started " + trip.StartTime.Day + "/" + trip.StartTime.Month + "/" + trip.StartTime.Year, FontFamily = Device.OnPlatform(null, "latolight.ttf#Lato Light", null), FontSize = 16, TextColor = Color.White, HorizontalTextAlignment = TextAlignment.Start }, 0, 1);
                grid.Children.Add(new Label { Text = "Tap for more information!", FontFamily = Device.OnPlatform(null, "latolight.ttf#Lato Light", null), FontSize = 16, TextColor = Color.White, HorizontalTextAlignment = TextAlignment.Start }, 0, 2);

                Frame frame = new Frame
                {
                    CornerRadius = 10,
                    BackgroundColor = Color.FromHex("#6e6fe3"),
                    Content = grid,
                    HasShadow = false,
                    Margin = new Thickness(0, 0, 0, 20)
                };

                frame.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(async () => { /*App.Current.MainPage.DisplayAlert("Loading..", "Getting device location..", "OK");*/ App.Current.MainPage.DisplayAlert("Loading..", "Fetching orders..", "OK");  Credentials.orderArray = await ClientFunctions.Instance.OrdersRequestAsync(Credentials.apibaseUrl + "/routing/orders/" + trip.Id); Credentials.orderArray = Credentials.orderArray.OrderBy(o => Int32.Parse(o.OrderNumber)).ToList().OrderBy(o => o.Complete).ToList(); ClientFunctions.ModifyOrders(Credentials.orderArray); App.Current.MainPage.Navigation.PushAsync(new TripsInfoPage(trip)); }) });
                return frame;
            }
            public static Frame CreateNewOrdersForm(Order order)
            {
                StackLayout grid = new StackLayout();
                grid.Children.Add(new Label { Text = "ORDER #" + order.Id, FontFamily = Device.OnPlatform(null, "latoblack.ttf#Lato Black", null), FontSize = 16, TextColor = Color.White, HorizontalTextAlignment = TextAlignment.Start, Margin = new Thickness(0, 0, 0, 10) });
                grid.Children.Add(new Label { Text = order.StreetName + ", nr " + order.StreetNumber + " (" + order.CityName + ")", FontFamily = Device.OnPlatform(null, "latolight.ttf#Lato Light", null), FontSize = 16, TextColor = Color.White, HorizontalTextAlignment = TextAlignment.Start });
                grid.Children.Add(new Label { Text = "(" + order.TimeWindowFrom + " - " + order.TimeWindowTill + ")  (" + order.OrderType + ")", FontFamily = Device.OnPlatform(null, "latolight.ttf#Lato Light", null), FontSize = 16, TextColor = Color.White, HorizontalTextAlignment = TextAlignment.Start });

                Frame frame = new Frame
                {
                    CornerRadius = 10,
                    BackgroundColor = Color.FromHex("#6e6fe3"),
                    Content = grid,
                    HasShadow = false,
                    Margin = new Thickness(0, 0, 0, 20)
                };
                return frame;
            }
            public static int ShowOrderInfoLabel(Order order)
            {
                App.Current.MainPage.DisplayAlert("Order #" + order.Id, System.Environment.NewLine + "Address: " + order.StreetName + ", no. " + order.StreetNumber + " (" + order.CityName + ", " + order.CountryCode + ")" + System.Environment.NewLine + System.Environment.NewLine + "Order Time Window: " + order.TimeWindowFrom + "-" + order.TimeWindowTill + System.Environment.NewLine + System.Environment.NewLine + "Estimated duration: " + (order.FixedDurationInSec/60).ToString().PadLeft(2, '0') + ":" + (order.FixedDurationInSec%60).ToString().PadLeft(2, '0') + " minutes" + System.Environment.NewLine + System.Environment.NewLine + "Order Type: " + order.OrderType + System.Environment.NewLine + System.Environment.NewLine + (order.Complete.Equals("true") ? "Order complete" : "Order in progress"), "Ok");
                return 0;
            }
            public static Frame CreateNewTripInfoForm(Order order, Frame oframe, Frame xButton, TripsMap map, Trip trip)
            {
                StackLayout stack = new StackLayout();
                var orderLabel = new Label { Text = "ORDER #" + order.Id + " (" + (order.Complete.Equals("false") ? "in progress" : "complete") + ")", FontFamily = Device.OnPlatform(null, "latoblack.ttf#Lato Black", null), FontSize = 16, TextColor = Color.Black, HorizontalTextAlignment = TextAlignment.Start, Margin = new Thickness(0, 0, 0, 10) };
                var moreInfoLabel = new Label { Text = "(more info)", FontFamily = Device.OnPlatform(null, "latoblack.ttf#Lato Black", null), FontSize = 16, TextColor = Color.Black, HorizontalTextAlignment = TextAlignment.Start, Margin = new Thickness(0, 0, 0, 10) };
                stack.Children.Add(orderLabel);
                stack.Children.Add(new Label { Text = order.StreetName + ", nr " + order.StreetNumber + " (" + order.CityName + ")", FontFamily = Device.OnPlatform(null, "latolight.ttf#Lato Light", null), FontSize = 16, TextColor = Color.Black, HorizontalTextAlignment = TextAlignment.Start });
                stack.Children.Add(moreInfoLabel);
                //stack.Children.Add(new Label { Text = "(" + order.TimeWindowFrom + " - " + order.TimeWindowTill + ")  (" + order.OrderType + ")", FontFamily = Device.OnPlatform(null, "latolight.ttf#Lato Light", null), FontSize = 16, TextColor = Color.Black, HorizontalTextAlignment = TextAlignment.Start });
                var frame = new Frame { Content = stack, BackgroundColor = Color.FromHex("#fcfcfc") };

                moreInfoLabel.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(async () =>
                    {
                        ShowOrderInfoLabel(order);
                    })
                });
                frame.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(async () =>
                    {
                        if (oframe.TranslationY != 0)
                        {
                            await oframe.TranslateTo(0, 0, 250, Easing.CubicOut);
                            await xButton.FadeTo(100, 250, Easing.CubicOut);
                            //xOptimize.FadeTo(100, 250, Easing.CubicOut);
                        }
                        else
                        {
                            var result = (order.Complete.Equals("true") ? await App.Current.MainPage.DisplayAlert("Change order status?", "Do you want to set this order as incomplete?", "No", "Yes") : await App.Current.MainPage.DisplayAlert("Change order status?", "Do you want to set this order as complete?", "No", "Yes"));
                            if (result == false)
                            {
                                string url = functions.Credentials.apibaseUrl + "/routing/orders/setcomplete";
                                string json = null;
                                try
                                {
                                    json = await functions.ClientFunctions.Instance.SetOrderCompleteAsync(url, order.Id, (order.Complete.Equals("true") ? "false" : "true"));
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Exception is: (1) {0}", ex);
                                }
                                if (json == null) App.Current.MainPage.DisplayAlert("Order status change failed!", "Your order status change request has failed. The server is either taking too long to respond or it is no longer working. Please try again later.", "OK");
                                else
                                {
                                    if (order.Complete.Equals("true"))
                                    {
                                        order.Complete = "false";
                                        orderLabel.Text = "ORDER #" + order.Id + " (" + (order.Complete.Equals("false") ? "in progress" : "complete") + ")";
                                    }
                                    else
                                    {
                                        order.Complete = "true";
                                        orderLabel.Text = "ORDER #" + order.Id + " (" + (order.Complete.Equals("false") ? "in progress" : "complete") + ")";
                                    }
                                    result = await App.Current.MainPage.DisplayAlert("Optimize route?", "You have changed the status of an order. Would you like to optimize the route?", "No", "Yes");
                                    if (result == false)
                                    {
                                        url = functions.Credentials.apibaseUrl + "/routing/optimize/" + trip.Id;
                                        App.Current.MainPage.DisplayAlert("Optimizing Trip..", "Attempting to optimize the current trip..", "OK");
                                        var response = await functions.ClientFunctions.Instance.OrdersOptimizeAsync(url);

                                        if (response.Count() == 0) App.Current.MainPage.DisplayAlert("Optimize request failed!", "Your optimize request failed. There were either no available orders to be triggered or the optimization couldn't be done for any order.", "OK");
                                        else
                                        {
                                            App.Current.MainPage.DisplayAlert("Success!", "The optimization request has succeeded!", "OK");
                                            map.RouteCoordinates.Clear();
                                            map.Pins.Clear();
                                            map.TripPins.Clear();
                                            map.HasZoomEnabled = true; map.HasZoomEnabled = false; // These are for calling the OnElementPropertyChanged

                                            map.RouteCoordinates.Add(new Xamarin.Forms.Maps.Position(Credentials.deviceLocation.Latitude, Credentials.deviceLocation.Longitude));
                                            var cpin = new TripsPin { Pin = new Pin { Type = PinType.Place, Position = new Xamarin.Forms.Maps.Position(Credentials.deviceLocation.Longitude, Credentials.deviceLocation.Latitude), Address = "Current position", Label = "Current Position" }, Id = "CLOUDROUTING_0", Url = "test" };
                                            map.Pins.Add(cpin.Pin);
                                            map.TripPins.Add(cpin);
                                            Credentials.orderArray = Credentials.orderArray.OrderBy(o => Int32.Parse(o.OrderNumber)).ToList().OrderBy(o => o.Complete).ToList();
                                            foreach (var xorder in Credentials.orderArray)
                                            {
                                                if (xorder.TripId == trip.Id)
                                                {
                                                    try
                                                    {
                                                        cpin = new TripsPin { Pin = new Pin { Type = PinType.Place, Position = new Xamarin.Forms.Maps.Position(xorder.GivenX, xorder.GivenY), Address = xorder.StreetName + " nr. " + xorder.StreetNumber, Label = "Order #" + xorder.Id }, Id = (xorder.Complete.Equals("true") ? "C" : "PC") + "LOUDROUTING_" + xorder.Id, Url = "test" };
                                                        map.Pins.Add(cpin.Pin);
                                                        map.TripPins.Add(cpin);
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        Console.WriteLine("Can't add new pin! Exception: {0}", e);
                                                    }
                                                    if (xorder.Complete.Equals("false")) map.RouteCoordinates.Add(new Xamarin.Forms.Maps.Position(xorder.GivenX, xorder.GivenY));
                                                }
                                            }

                                            var currentPos = new Xamarin.Forms.Maps.Position(Credentials.deviceLocation.Latitude, Credentials.deviceLocation.Longitude);
                                            if (currentPos.Latitude == Credentials.deviceLocation.Latitude) currentPos = new Xamarin.Forms.Maps.Position(currentPos.Latitude + 0.0001, currentPos.Longitude + 0.0001); else currentPos = new Xamarin.Forms.Maps.Position(Credentials.deviceLocation.Latitude, Credentials.deviceLocation.Longitude);
                                            map.MoveToRegion(MapSpan.FromCenterAndRadius(currentPos, Distance.FromMiles(0.3))); // This is for editing the "VisibleRegion" property thus recreating the polylines and pins
                                        }
                                    }
                                }
                            }
                        }
                    })
                });
                return frame;
            }
        }
    }
}
