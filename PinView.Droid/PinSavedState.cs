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
    public class PinSavedState : View.BaseSavedState
    {
        public string Pin
        {
            get;
        }

        public PinSavedState(IParcelable parcel, string pin) : base(parcel)
        {
            Pin = pin;
        }

        public PinSavedState(Parcel parcel) : base(parcel)
        {
            Pin = parcel.ReadString();
        }

        public override void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {
            base.WriteToParcel(dest, flags);
            dest.WriteString(Pin);
        }
    }
}