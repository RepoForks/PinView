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
using Android.Graphics;

namespace PinView.Droid
{
    public class PinSavedState : View.BaseSavedState
    {
        public string Pin
        {
            get;
        }

        public int DigitCount
        {
            get;
            set;
        }

        public int AccentHeight
        {
            get;
            set;
        }

        public int DigitWidth
        {
            get;
            set;
        }

        public int DigitHeight
        {
            get;
            set;
        }

        public int DigitSpacing
        {
            get;
            set;
        }

        public int TextSize
        {
            get;
            set;
        }

        public Color DigitBorderColor
        {
            get;
            set;
        }

        public Color DigitBackgroundColor
        {
            get;
            set;
        }

        public Color AccentColor
        {
            get;
            set;
        }

        public Color TextColor
        {
            get;
            set;
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

        public void Save(PinWidget view)
        {
            DigitCount = (int)view.DigitCount;
            DigitBorderColor = view.DigitBorderColor;
            DigitBackgroundColor = view.DigitBackgroundColor;
            DigitHeight = view.DigitHeight;
            DigitWidth = view.DigitWidth;
            DigitSpacing = view.DigitSpacing;
            AccentColor = view.AccentColor;
            AccentHeight = view.AccentHeight;
            TextSize = view.TextSize;
            TextColor = view.TextColor;
        }

        public void Restore(PinWidget view)
        {
            view.DigitCount = (uint)DigitCount;
            view.DigitBorderColor = DigitBorderColor;
            view.DigitBackgroundColor = DigitBackgroundColor;
            view.DigitHeight = DigitHeight;
            view.DigitWidth = DigitWidth;
            view.DigitSpacing = DigitSpacing;
            view.AccentColor = AccentColor;
            view.AccentHeight = AccentHeight;
            view.TextSize = TextSize;
            view.TextColor = TextColor;
        }
    }
}