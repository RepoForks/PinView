using System;

namespace PinView.Abstractions
{
    public class PinFinishedEventArgs : EventArgs
    {
        public string Pin
        {
            get;
        }

        public PinFinishedEventArgs(string pin)
        {
            if (string.IsNullOrEmpty(pin))
            {
                Pin = string.Empty;
            }
            else
            {
                Pin = pin;
            }
        }
    }
}