using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CLOUDROUTING
{
	public partial class MainPage : ContentPage
	{
        private double _width;
        private double _height;
        Frame[] logoFrames = new Frame[]
        {
            new Frame { BackgroundColor = Color.FromHex("#ffffff"), Opacity = 0 },
            new Frame { BackgroundColor = Color.FromHex("#ffffff"), Opacity = 0 },
            new Frame { BackgroundColor = Color.FromHex("#ffffff"), Opacity = 0 }
        };
        Image[] logoMarkers = new Image[]
        {
            new Image { Source = "@drawables/mapmarker.png", WidthRequest = 40, HeightRequest = 40, Aspect = Aspect.AspectFit, Opacity = 0 },
            new Image { Source = "@drawables/mapmarker.png", WidthRequest = 40, HeightRequest = 40, Aspect = Aspect.AspectFit, Opacity = 0 },
            new Image { Source = "@drawables/mapmarker.png", WidthRequest = 40, HeightRequest = 40, Aspect = Aspect.AspectFit, Opacity = 0 },
            new Image { Source = "@drawables/mapmarker.png", WidthRequest = 40, HeightRequest = 40, Aspect = Aspect.AspectFit, Opacity = 0 }
        };
        public MainPage()
		{
			InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);

            crEntry cr_CustomerKey = new crEntry { Placeholder = "Customer Key", PlaceholderColor = Color.White, TextColor = Color.White, FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(crEntry)), Opacity = 0.5, HeightRequest = 55, WidthRequest = 150, Text = "", BorderColor = Color.FromHex("#ffffff"), HorizontalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.CenterAndExpand, Margin = new Thickness(0, 10, 0, 10) };
            crEntry cr_DriverID = new crEntry { Placeholder = "Driver ID", PlaceholderColor = Color.White, TextColor = Color.White, FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(crEntry)), Opacity = 0.5, HeightRequest = 55, WidthRequest = 150, Text = "", BorderColor = Color.FromHex("#ffffff"), HorizontalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.CenterAndExpand, Margin = new Thickness(0, 0, 0, 10) };

            Image loginButton = new Image { Source = "@drawables/login.png", Aspect = Aspect.AspectFit, WidthRequest = 200, HeightRequest = 86 };
            ActivityIndicator loading = new ActivityIndicator { IsRunning = false, Opacity = 0, IsEnabled = false, HeightRequest = 75, WidthRequest = 75 };
            relativeLayout.Children.Add(loading, Constraint.RelativeToParent((parent) => { return parent.Width/2 - loading.WidthRequest/2; }), Constraint.RelativeToParent((parent) => { return parent.Height/2 - loading.HeightRequest/2; }));
            relativeLayout.Children.Add(logoFrames[0], Constraint.RelativeToParent((parent) => { return parent.Width / 2 - parent.Height / 6; }), Constraint.RelativeToParent((parent) => { return parent.Height / 34; }), Constraint.RelativeToParent((parent) => { return parent.Height / 3; }), Constraint.RelativeToParent((parent) => { return parent.Height / 3; }));
            relativeLayout.Children.Add(logoFrames[1], Constraint.RelativeToParent((parent) => { return parent.Width / 2 - parent.Height / 7.5; }), Constraint.RelativeToParent((parent) => { return parent.Height / 16; }), Constraint.RelativeToParent((parent) => { return parent.Height / 3.75; }), Constraint.RelativeToParent((parent) => { return parent.Height / 3.75; }));
            relativeLayout.Children.Add(logoFrames[2], Constraint.RelativeToParent((parent) => { return parent.Width / 2 - parent.Height / 10.5; }), Constraint.RelativeToParent((parent) => { return parent.Height / 10; }), Constraint.RelativeToParent((parent) => { return parent.Height / 5.25; }), Constraint.RelativeToParent((parent) => { return parent.Height / 5.25; }));
            relativeLayout.Children.Add(logoMarkers[0], Constraint.RelativeToParent((parent) => { return parent.Width / 2.75; }), Constraint.RelativeToParent((parent) => { return parent.Height / 20; }));
            relativeLayout.Children.Add(logoMarkers[1], Constraint.RelativeToParent((parent) => { return parent.Width / 4.75; }), Constraint.RelativeToParent((parent) => { return parent.Height / 6; }));
            relativeLayout.Children.Add(logoMarkers[2], Constraint.RelativeToParent((parent) => { return parent.Width - parent.Width / 2.5; }), Constraint.RelativeToParent((parent) => { return parent.Height / 22; }));
            relativeLayout.Children.Add(logoMarkers[3], Constraint.RelativeToParent((parent) => { return parent.Width - parent.Width / 3; }), Constraint.RelativeToParent((parent) => { return parent.Height / 4; }));
            relativeLayout.Children.Add(new Image { Source = "@drawables/crlogo.png", Aspect = Aspect.AspectFit}, Constraint.RelativeToParent((parent) => { return parent.Width / 2 - parent.Height / 18; }), Constraint.RelativeToParent((parent) => { return parent.Height / 7; }), Constraint.RelativeToParent((parent) => { return parent.Height / 9; }), Constraint.RelativeToParent((parent) => { return parent.Height / 9; }));

            loginButton.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    loading.IsEnabled = true; loading.Opacity = 1; loading.IsRunning = true;
                    await functions.ClientFunctions.Instance.GetDeviceLocation();
                    functions.Credentials.customerKey = cr_CustomerKey.Text;
                    functions.Credentials.driverId = cr_DriverID.Text;
                    string url = functions.Credentials.apibaseUrl + "/login/driver";

                    string json = null;
                    try
                    {
                        json = await functions.ClientFunctions.Instance.LoginRequestAsync(url);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception is: (1) {0}", ex);
                    }
                    if (json == null) DisplayAlert("Login Failed!", "Your login request has failed. Please check your credentials and try again later", "OK");
                    else
                    {
                        functions.ClientFunctions.Instance.cr_TriggerPage(1);
                        cr_CustomerKey.Text = ""; cr_DriverID.Text = "";
                    }
                    loading.IsEnabled = false; loading.Opacity = 0; loading.IsRunning = false;
                })
            });

            mainStackLayout.Children.Add(new Label { Text = "Welcome back! Please log into your account.", FontSize = 14, TextColor = Color.White, HorizontalTextAlignment = TextAlignment.Center, FontFamily = Device.OnPlatform(null, "latolight.ttf#Lato Light", null), Margin = new Thickness(0, 10, 0, 0) });
            mainStackLayout.Children.Add(cr_CustomerKey);
            mainStackLayout.Children.Add(cr_DriverID);
            mainStackLayout.Children.Add(loginButton);
            mainStackLayout.Children.Add(new Label { FontFamily = Device.OnPlatform(null, "latolight.ttf#Lato Light", null), FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)), Text = "Please add your credentials in order to login.", HorizontalTextAlignment = TextAlignment.Center, TextColor = Color.White });

            cr_CustomerKey.TextChanged += delegate {
                cr_CustomerKey.TextColor = Color.White;
            };
        }
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            _width = width;
            _height = height;

            foreach(var num in logoFrames)
            {
                num.CornerRadius = ((float)(num.Width)) / 2;
            }
            Task.Run(async () =>
            {
                await Task.Delay(1000);
                await logoFrames[2].FadeTo(0.07, 400);
                await Task.Delay(400);
                await logoFrames[1].FadeTo(0.07, 400);
                await Task.Delay(400);
                await logoFrames[0].FadeTo(0.07, 400);
                await Task.Delay(400);
                await logoMarkers[0].FadeTo(1, 400);
                await Task.Delay(400);
                await logoMarkers[1].FadeTo(1, 400);
                await Task.Delay(400);
                await logoMarkers[2].FadeTo(1, 400);
                await Task.Delay(400);
                await logoMarkers[3].FadeTo(1, 400);
            });
        }
    }
}
