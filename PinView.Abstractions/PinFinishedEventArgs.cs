using System;

namespace PinView.Abstractions
{
    public class PinCompletedEventArgs : EventArgs
    {
        public string Pin
        {
            get;
        }

        public PinCompletedEventArgs(string pin)
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