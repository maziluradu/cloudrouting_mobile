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
	public partial class MenuPage : ContentPage
	{
		public MenuPage ()
		{
			InitializeComponent ();

            NavigationPage.SetHasNavigationBar(this, false);

            RelativeLayout layout = new RelativeLayout();
            Content = layout;

            StackLayout driverInfo = new StackLayout();

            var avatarSize = Device.OnPlatform(75, 75, 75);
            Image avatar = new Image
            {
                Source = "@drawable/crpagelogoblue.png",
                WidthRequest = avatarSize,
                HeightRequest = avatarSize,
                Aspect = Aspect.AspectFit
            };

            Label header = new Label
            {
                Text = "Driver #" + functions.Credentials.driverId,
                FontFamily = Device.OnPlatform(
                    null,
                    "latoregular.ttf#Lato", // Android
                    null
                  ),
                FontSize = 20,
                TextColor = Color.FromHex("#343334"),
                HorizontalTextAlignment = TextAlignment.Center
            };

            driverInfo.Children.Add(avatar);
            driverInfo.Children.Add(header);

            layout.Children.Add(driverInfo, 
            Constraint.Constant(0), // X Constraint
            Constraint.RelativeToParent((parent) => // Y Constraint
            {
                return (.08 * parent.Height);
            }),
            Constraint.RelativeToParent((parent) =>  // Width
            {
                return parent.Width;
            }));

            var controlGrid = new Grid { ColumnSpacing = 0, RowSpacing = 0, Padding = 5 };
            controlGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            controlGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            controlGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            controlGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            controlGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
            controlGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(6, GridUnitType.Star) });
            controlGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });

            Label[] crLabel = new Label[] {
                new Label
                {
                    Text = "Driver Profile",
                    FontFamily = Device.OnPlatform(
                        null,   
                        "latoregular.ttf#Lato", // Android
                        null
                      ),
                    FontSize = 22,
                    TextColor = Color.FromHex("#343334"),
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalOptions = LayoutOptions.Center
                },
                new Label
                {
                    Text = "My Trips",
                    FontFamily = Device.OnPlatform(
                        null,
                        "latoregular.ttf#Lato", // Android
                        null
                      ),
                    FontSize = 22,
                    TextColor = Color.FromHex("#343334"),
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalOptions = LayoutOptions.Center
                },
                new Label
                {
                    Text = "Unplanned Orders",
                    FontFamily = Device.OnPlatform(
                        null,
                        "latoregular.ttf#Lato", // Android
                        null
                      ),
                    FontSize = 22,
                    TextColor = Color.FromHex("#343334"),
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalOptions = LayoutOptions.Center
                },
                new Label
                {
                    Text = "Logout",
                    FontFamily = Device.OnPlatform(
                        null,
                        "latoregular.ttf#Lato", // Android
                        null
                      ),
                    FontSize = 22,
                    TextColor = Color.FromHex("#343334"),
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalOptions = LayoutOptions.Center
                }
            };

            var imageSize = Device.OnPlatform(18, 18, 18);
            Image[] menuIcons = new Image[]
            {
                new Image {
                    Source = "@drawables/crusericon.png",
                    VerticalOptions = LayoutOptions.Center,
                    WidthRequest = imageSize,
                    HeightRequest = imageSize,
                    HorizontalOptions = LayoutOptions.Start,
                    Aspect = Aspect.AspectFit
                },
                new Image {
                    Source = "@drawables/crtrips.png",
                    VerticalOptions = LayoutOptions.Center,
                    WidthRequest = imageSize,
                    HeightRequest = imageSize,
                    HorizontalOptions = LayoutOptions.Start,
                    Aspect = Aspect.AspectFit
                },
                new Image {
                    Source = "@drawables/crorders.png",
                    VerticalOptions = LayoutOptions.Center,
                    WidthRequest = imageSize,
                    HeightRequest = imageSize,
                    HorizontalOptions = LayoutOptions.Start,
                    Aspect = Aspect.AspectFit
                },
                new Image {
                    Source = "@drawables/crlogout.png",
                    VerticalOptions = LayoutOptions.Center,
                    WidthRequest = imageSize,
                    HeightRequest = imageSize,
                    HorizontalOptions = LayoutOptions.Start,
                    Aspect = Aspect.AspectFit
                }
            };

            Image[] nextIcons = new Image[]
            {
                new Image {
                    Source = "@drawables/next.png",
                    VerticalOptions = LayoutOptions.Center,
                    WidthRequest = imageSize-2,
                    HeightRequest = imageSize-2,
                    HorizontalOptions = LayoutOptions.End
                },
                new Image {
                    Source = "@drawables/next.png",
                    VerticalOptions = LayoutOptions.Center,
                    WidthRequest = imageSize-2,
                    HeightRequest = imageSize-2,
                    HorizontalOptions = LayoutOptions.End
                },
                new Image {
                    Source = "@drawables/next.png",
                    VerticalOptions = LayoutOptions.Center,
                    WidthRequest = imageSize-2,
                    HeightRequest = imageSize-2,
                    HorizontalOptions = LayoutOptions.End
                },
                new Image {
                    Source = "@drawables/next.png",
                    VerticalOptions = LayoutOptions.Center,
                    WidthRequest = imageSize-2,
                    HeightRequest = imageSize-2,
                    HorizontalOptions = LayoutOptions.End
                }
            };
            for (int i = 0; i < 4; i++) controlGrid.Children.Add(menuIcons[i], 0, i);
            for (int i = 0; i < 4; i++) controlGrid.Children.Add(crLabel[i], 1, i);
            for (int i = 0; i < 4; i++) controlGrid.Children.Add(nextIcons[i], 2, i);

            ActivityIndicator loading = new ActivityIndicator { IsRunning = false, Opacity = 0, IsEnabled = false, HeightRequest = 75, WidthRequest = 75 };
            layout.Children.Add(loading, Constraint.RelativeToParent((parent) => { return parent.Width / 2 - loading.WidthRequest / 2; }), Constraint.RelativeToParent((parent) => { return parent.Height / 2 - loading.HeightRequest / 2; }));

            functions.ClientFunctions.AddPageChangeFunction(crLabel[0], menuIcons[0], nextIcons[0], 2, loading);
            functions.ClientFunctions.AddPageChangeFunction(crLabel[1], menuIcons[1], nextIcons[1], 3, loading);
            functions.ClientFunctions.AddPageChangeFunction(crLabel[2], menuIcons[2], nextIcons[2], 4, loading);
            functions.ClientFunctions.AddPageChangeFunction(crLabel[3], menuIcons[3], nextIcons[3], 5, loading);

            layout.Children.Add(controlGrid, Constraint.RelativeToParent((parent) => { return (0.15 * parent.Width); }), Constraint.RelativeToParent((parent) => { return (.47 * parent.Height); }), Constraint.RelativeToParent((parent) => { return (0.7 * parent.Width); }), Constraint.RelativeToParent((parent) => { return (0.404 * parent.Height); }));
        }
	}
}