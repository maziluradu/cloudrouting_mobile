using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using CLOUDROUTING;
using CLOUDROUTING.Droid;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;

[assembly: ExportRenderer(typeof(crEntry), typeof(crEntryRenderer))]
namespace CLOUDROUTING.Droid
{
    public class crEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            if (Control == null || Element == null || e.OldElement != null) return;

            var element = (crEntry)Element;
            var customColor = element.BorderColor.ToAndroid();
            Control.Background.SetColorFilter(customColor, PorterDuff.Mode.SrcAtop);
        }
    }
}