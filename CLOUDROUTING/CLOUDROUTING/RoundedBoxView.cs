using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace CLOUDROUTING
{
    public class RoundedBoxView : BoxView
    {
        public static readonly BindableProperty CornerRadiusProperty = 
            BindableProperty.Create<RoundedBoxView, double>(p => p.CornerRadius, 0);

        public double CornerRadius {
            get { return (double) base.GetValue(CornerRadiusProperty); }
            set { base.SetValue(CornerRadiusProperty, value);  }
        }
    }
}
