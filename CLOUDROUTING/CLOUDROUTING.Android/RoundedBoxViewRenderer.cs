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

[assembly: ExportRenderer(typeof(RoundedBoxView), typeof(RoundedBoxViewRenderer))]
namespace CLOUDROUTING.Droid
{
    public class RoundedBoxViewRenderer : BoxRenderer
    {
        public RoundedBoxViewRenderer()
        {
            this.SetWillNotDraw(false);
        }
        public override void Draw(Canvas canvas) {
            RoundedBoxView rbv = (RoundedBoxView)this.Element;

            Rect rc = new Rect();
            GetDrawingRect(rc);

            Paint p = new Paint()
            {
                Color = rbv.Color.ToAndroid(),
                AntiAlias = true
            };

            canvas.DrawRoundRect(new RectF(rc), (float)rbv.CornerRadius, (float)rbv.CornerRadius, p);
        }
    }
}