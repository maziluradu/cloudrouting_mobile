using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CLOUDROUTING
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DriverPage : ContentPage
	{
        public DriverPage()
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);

            StackLayout stackLayout = new StackLayout
            {
            };
            Content = new ScrollView { Content = stackLayout };

            var avatarSize = Device.OnPlatform(75, 75, 75);
            stackLayout.Children.Add(new Image { Source = "@drawables/crpagelogowhite.png", WidthRequest = avatarSize, HeightRequest = avatarSize, Aspect = Aspect.AspectFit, Margin = new Thickness(0, 100, 0, 20) });
            stackLayout.Children.Add(new Label { Text = "Driver #" + functions.Credentials.driverId, FontFamily = Device.OnPlatform(null, "latoregular.ttf#Lato", null), FontSize = 20, TextColor = Color.White, HorizontalTextAlignment = TextAlignment.Center });
            stackLayout.Children.Add(new Label { Text = "(Customer Key: " + functions.Credentials.customerKey + ")", FontFamily = Device.OnPlatform(null, "latolight.ttf#Lato Light", null), FontSize = 18, TextColor = Color.White, HorizontalTextAlignment = TextAlignment.Center, Margin = new Thickness(0, 0, 0, 50) });
            if (functions.Credentials.tripsArray.Count > 0)
            {
                stackLayout.Children.Add(new Label { Text = "Trip Start Time", FontFamily = Device.OnPlatform(null, "latolight.ttf#Lato Light", null), FontSize = 18, TextColor = Color.White, HorizontalTextAlignment = TextAlignment.Center, Margin = new Thickness(0, 0, 0, 25) });
                stackLayout.Children.Add(new Label { Text = functions.Credentials.tripsArray[0].StartTime.ToString("HH:mm:ss"), FontFamily = Device.OnPlatform(null, "latoregular.ttf#Lato", null), FontSize = 22, TextColor = Color.White, HorizontalTextAlignment = TextAlignment.Center, Margin = new Thickness(0, 0, 0, 50) });
                stackLayout.Children.Add(new Label { Text = "Maximum Trip Finish Time", FontFamily = Device.OnPlatform(null, "latolight.ttf#Lato Light", null), FontSize = 18, TextColor = Color.White, HorizontalTextAlignment = TextAlignment.Center, Margin = new Thickness(0, 0, 0, 25) });
                stackLayout.Children.Add(new Label { Text = functions.Credentials.tripsArray[0].FinishTime.ToString("HH:mm:ss"), FontFamily = Device.OnPlatform(null, "latoregular.ttf#Lato", null), FontSize = 22, TextColor = Color.White, HorizontalTextAlignment = TextAlignment.Center, Margin = new Thickness(0, 0, 0, 50) });
            }
            else
            {
                stackLayout.Children.Add(new Label { Text = "No trip assigned to driver!", FontFamily = Device.OnPlatform(null, "latoregular.ttf#Lato", null), FontSize = 22, TextColor = Color.White, HorizontalTextAlignment = TextAlignment.Center, Margin = new Thickness(0, 0, 0, 50) });
            }
        }
	}
}