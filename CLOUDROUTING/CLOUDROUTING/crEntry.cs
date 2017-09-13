using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace CLOUDROUTING
{
    public class crEntry : Entry
    {
        /// <summary>
        /// The PlaceholderTextColor property
        /// </summary>
        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create("BorderColor", typeof(Color), typeof(Entry), Color.White);

        /// <summary>
        /// Gets or sets the color of the border.
        /// </summary>
        /// <value>The color of the border.</value>
        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }
    }
}
