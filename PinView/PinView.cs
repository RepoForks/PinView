using PinView.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace PinView
{
    public class PinView : View
    {
        #region Length Property
        /// <summary>
        /// Backing store for the LengthProperty.
        /// </summary>
        public static readonly BindableProperty LengthProperty = BindableProperty.Create(nameof(Length), typeof(int), typeof(PinView), 4);

        /// <summary>
        /// Length of the Pin for the PinView.
        /// </summary>
        public int Length
        {
            get
            {
                return (int)GetValue(LengthProperty);
            }
            set
            {
                SetValue(LengthProperty, value);
            }
        }
        #endregion

        #region Completed Command
        /// <summary>
        /// Backing store for Completed Command.
        /// </summary>
        public static readonly BindableProperty CompletedCommandProperty = BindableProperty.Create(nameof(CompletedCommand), typeof(Command<string>), typeof(PinView), default(Command<string>));

        /// <summary>
        /// Command that fires when Pin is entered.
        /// </summary>
        public Command<string> CompletedCommand
        {
            get
            {
                return (Command<string>)GetValue(CompletedCommandProperty);
            }
            set
            {
                SetValue(CompletedCommandProperty, value);
            }
        }
        #endregion

        #region Pin
        /// <summary>
        /// Backing store for Pin. 
        /// </summary>
        internal static readonly BindableProperty PinProperty = BindableProperty.Create(nameof(Pin), typeof(string), typeof(PinView), string.Empty, propertyChanged: PinChanged);

        private static void PinChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as PinView;
            if (view != null)
            {
                view.OnPinChanged(view.Pin);
            }
        }

        /// <summary>
        /// The Pin entered.
        /// </summary>
        internal string Pin
        {
            get
            {
                return (string)GetValue(PinProperty);
            }
            set
            {
                SetValue(PinProperty, value);
            }
        }
        #endregion

        /// <summary>
        /// Fires when pin is completely input.
        /// </summary>
        public EventHandler<PinFinishedEventArgs> PinCompleted;

        public PinView()
        {
            
        }

        /// <summary>
        /// Handles firing off events and commands if necessary.
        /// </summary>
        /// <param name="pin">The current pin.</param>
        protected virtual void OnPinChanged(string pin)
        {
            if (string.IsNullOrEmpty(pin))
            {
                return;
            }

            if (pin.Length == Length)
            {
                var handler = PinCompleted;

                if (CompletedCommand != default(Command))
                {
                    if (CompletedCommand.CanExecute(pin))
                    {
                        CompletedCommand.Execute(pin);
                    }
                }

                handler?.Invoke(this, new PinFinishedEventArgs(pin));
            }
        }
    }
}
