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

namespace PinView.Droid
{
    internal class DigitView : TextView
    {
        private Context context;

        public DigitView(Context context) : base(context)
        {
            this.context = context;
        }
    }
}