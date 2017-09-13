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
	public partial class TripsPage : ContentPage
	{
		public TripsPage ()
		{
			InitializeComponent ();

            NavigationPage.SetHasNavigationBar(this, false);

            RelativeLayout relativeLayout = new RelativeLayout { Margin = new Thickness(0, 0, 0, 0), Padding = new Thickness(0, 0, 0, 0), HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand };
            StackLayout stackLayout = new StackLayout {
                Padding = new Thickness(30, 40, 30, 0)
            };
            relativeLayout.Children.Add(new Image { Source = "@drawables/tripsmdpi.jpg", Aspect = Aspect.AspectFill }, Constraint.Constant(0) , Constraint.Constant(0), Constraint.RelativeToParent((parent) => { return parent.Width; }), Constraint.RelativeToParent((parent) => { return parent.Height; }));
            relativeLayout.Children.Add(stackLayout, Constraint.Constant(0));
            Content = new ScrollView { Content = relativeLayout, Margin = new Thickness(0, 0, 0, 0), Padding = new Thickness(0, 0, 0, 0), HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand };

            var avatarSize = Device.OnPlatform(75, 75, 75);
            stackLayout.Children.Add(new Image { Source = "@drawables/crpagelogowhite.png", WidthRequest = avatarSize, HeightRequest = avatarSize, Aspect = Aspect.AspectFit, Margin = new Thickness(0, 0, 0, 10) });
            stackLayout.Children.Add(new Label { Text = "Driver #" + functions.Credentials.driverId + " trips", FontFamily = Device.OnPlatform(null, "latoregular.ttf#Lato", null), FontSize = 20, TextColor = Color.White, HorizontalTextAlignment = TextAlignment.Center, Margin = new Thickness(0, 0, 0, 70) });

            //functions.Credentials.tripsArray = functions.Credentials.tripsArray.OrderBy(p => p.).ToList()
            foreach(var trip in functions.Credentials.tripsArray)
            {
                stackLayout.Children.Add(functions.createMisc.CreateNewTripsForm(trip, (trip.Finished == 0 ? true : false)));
            }
        }
	}
}