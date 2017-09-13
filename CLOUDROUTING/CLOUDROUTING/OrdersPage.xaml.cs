using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CLOUDROUTING
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OrdersPage : ContentPage
    {
        Image triggerRouting = null;
        public OrdersPage()
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);

            RelativeLayout relativeLayout = new RelativeLayout { Margin = new Thickness(0, 0, 0, 0), Padding = new Thickness(0, 0, 0, 0), HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand };
            StackLayout stackLayout = new StackLayout
            {
                Padding = new Thickness(30, 40, 30, 0),
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            relativeLayout.Children.Add(new Image { Source = "@drawables/tripsmdpi.jpg", Aspect = Aspect.AspectFill }, Constraint.Constant(0), Constraint.Constant(0), Constraint.RelativeToParent((parent) => { return parent.Width; }), Constraint.RelativeToParent((parent) => { return parent.Height; }));
            relativeLayout.Children.Add(stackLayout, Constraint.Constant(0), Constraint.Constant(0), Constraint.RelativeToParent((parent) => { return parent.Width; }));
            ScrollView scroll = new ScrollView { Content = relativeLayout };
            Content = scroll;

            triggerRouting = new Image
            {
                Source = "@drawables/trigger.png", WidthRequest = 89, HeightRequest = 89, Opacity = 0
            };
            relativeLayout.Children.Add(triggerRouting, Constraint.RelativeToParent((parent) => { return parent.Width - triggerRouting.Width - 0.04 * parent.Width; }), Constraint.RelativeToParent((parent) => { return Application.Current.MainPage.Height - triggerRouting.Height - 0.04 * Application.Current.MainPage.Height; }));

            var avatarSize = Device.OnPlatform(75, 75, 75);
            stackLayout.Children.Add(new Image { Source = "@drawables/crpagelogowhite.png", WidthRequest = avatarSize, HeightRequest = avatarSize, Aspect = Aspect.AspectFit, Margin = new Thickness(0, 0, 0, 10) });
            stackLayout.Children.Add(new Label { Text = "Driver #" + functions.Credentials.driverId + " orders", FontFamily = Device.OnPlatform(null, "latoregular.ttf#Lato", null), FontSize = 20, TextColor = Color.White, HorizontalTextAlignment = TextAlignment.Center, Margin = new Thickness(0, 0, 0, 70) });

            StackLayout newFrameStack = new StackLayout { };

            Frame newFrame = new Frame { CornerRadius = 10, BackgroundColor = Color.FromHex("#6e6fe3"), HasShadow = true, Margin = new Thickness(0, 0, 0, 20), Padding = new Thickness(20, 20), Opacity = 0, Content = newFrameStack };

            Label[] frameLabels = new Label[]
            {
                new Label { FontFamily = Device.OnPlatform(null, "latoblack.ttf#Lato Black", null), FontSize = 20, TextColor = Color.White, HorizontalTextAlignment = TextAlignment.Start, Margin = new Thickness(0, 0, 0, 25) },
                new Label { FontFamily = Device.OnPlatform(null, "latolight.ttf#Lato Light", null), FontSize = 18, TextColor = Color.White, HorizontalTextAlignment = TextAlignment.Start, Margin = new Thickness(0, 0, 0, 10) },
                new Label { FontFamily = Device.OnPlatform(null, "latolight.ttf#Lato Light", null), FontSize = 18, TextColor = Color.White, HorizontalTextAlignment = TextAlignment.Start, Margin = new Thickness(0, 0, 0, 10) },
                new Label { FontFamily = Device.OnPlatform(null, "latolight.ttf#Lato Light", null), FontSize = 18, TextColor = Color.White, HorizontalTextAlignment = TextAlignment.Start, Margin = new Thickness(0, 0, 0, 10) },
                new Label { FontFamily = Device.OnPlatform(null, "latolight.ttf#Lato Light", null), FontSize = 18, TextColor = Color.White, HorizontalTextAlignment = TextAlignment.Start, Margin = new Thickness(0, 0, 0, 10) },
                new Label { FontFamily = Device.OnPlatform(null, "latolight.ttf#Lato Light", null), FontSize = 18, TextColor = Color.White, HorizontalTextAlignment = TextAlignment.Start, Margin = new Thickness(0, 0, 0, 10) },
                new Label { FontFamily = Device.OnPlatform(null, "latoregular.ttf#Lato", null), FontSize = 18, TextColor = Color.White, HorizontalTextAlignment = TextAlignment.Start, Margin = new Thickness(0, 20, 0, 10) }
            };

            newFrameStack.Children.Add(frameLabels[0]); newFrameStack.Children.Add(frameLabels[1]); newFrameStack.Children.Add(frameLabels[2]); newFrameStack.Children.Add(frameLabels[3]); newFrameStack.Children.Add(frameLabels[4]); newFrameStack.Children.Add(frameLabels[5]); newFrameStack.Children.Add(frameLabels[6]);
            Button closeButton = new Button { Text = "Close", FontFamily = Device.OnPlatform(null, "latoblack.ttf#Lato Black", null), FontSize = 18, TextColor = Color.White, Margin = new Thickness(0, 20, 0, 10), BorderRadius = 10, BorderColor = Color.FromHex("#dc0c0c"), BackgroundColor = Color.FromHex("#6e6fe3"), HorizontalOptions = LayoutOptions.Center };
            newFrameStack.Children.Add(closeButton);

            ActivityIndicator loading = new ActivityIndicator { IsRunning = false, Opacity = 0, IsEnabled = false, HeightRequest = 75, WidthRequest = 75 };
            relativeLayout.Children.Add(loading, Constraint.RelativeToParent((parent) => { return parent.Width / 2 - loading.WidthRequest / 2; }), Constraint.RelativeToParent((parent) => { return parent.Height / 2 - loading.HeightRequest / 2; }));

            closeButton.Clicked += async(o, e) =>
            {
                var executed = await newFrame.FadeTo(0, 300, Easing.Linear);
                relativeLayout.Children.Remove(newFrame);
            };
            foreach (functions.Order order in functions.Credentials.orderArray)
            {
                Frame frame = functions.createMisc.CreateNewOrdersForm(order);
                frame.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(() => {
                        /*frameLabels[0].Text = "ORDER #" + order.Id;
                        frameLabels[1].Text = "Delivery Address: " + order.StreetName + ", nr. " + order.StreetNumber;
                        frameLabels[2].Text = "Location: " + order.CityName + " (Country code: " + order.CountryCode + ")";
                        frameLabels[3].Text = "Type of order: " + order.OrderType;
                        frameLabels[4].Text = "Duration: " + order.StopDurationInSec + " seconds (" + order.TimeWindowFrom + " - " + order.TimeWindowTill + ")";
                        frameLabels[5].Text = "Comment: " + order.Comment;
                        frameLabels[6].Text = "Order is not complete";
                        relativeLayout.Children.Add(newFrame, Constraint.Constant(0.07 * Application.Current.MainPage.Width), Constraint.Constant(0.1 * Application.Current.MainPage.Height), Constraint.Constant(0.86 * Application.Current.MainPage.Width), Constraint.Constant(0.8 * Application.Current.MainPage.Height));
                        newFrame.FadeTo(1, 300, Easing.Linear);*/
                        functions.createMisc.ShowOrderInfoLabel(order);
                    })
                });
                stackLayout.Children.Add(frame);
            }
            scroll.Scrolled += delegate
            {
                newFrame.TranslationY = scroll.ScrollY;
                triggerRouting.TranslationY = scroll.ScrollY;
            };
            triggerRouting.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () => {
                    string url = functions.Credentials.apibaseUrl + "/routing/trigger";
                    DisplayAlert("Triggering routing..", "Attempting to start a route trigger..", "OK");
                    HttpResponseMessage response = await functions.ClientFunctions.Instance.OrdersTriggerAsync(url);

                    if (!response.IsSuccessStatusCode) DisplayAlert("Trigger request failed!", "Your trigger request failed. There were either no available orders to be triggered or the routing couldn't be done for any order.", "OK");
                    else
                    {
                        DisplayAlert("Success!", "The trigger request succeeded!", "OK");
                        await App.Current.MainPage.Navigation.PopAsync();
                        await functions.ClientFunctions.Instance.cr_TriggerPage(3, loading);
                    }
                })
            });
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await WaitAndExecute(750, () =>
            {
                if (functions.Credentials.orderArray.Any()) triggerRouting.FadeTo(1, 200, Easing.Linear);
            });
        }
        protected async Task WaitAndExecute(int milisec, Action actionToExecute)
        {
            await Task.Delay(milisec);
            actionToExecute();
        }
    }
}