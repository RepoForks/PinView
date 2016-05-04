using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using PinView.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;


namespace PinView.Droid
{
    [Register("com.ameaney." + nameof(PinView))]
    public class PinWidget : HorizontalScrollView
    {
        #region Properties
        public uint DigitCount
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
        #endregion

        public event EventHandler<PinCompletedEventArgs> PinCompleted;

        public IOnPinCompletedListener PinListener
        {
            get;
            set;
        }

        private const string MASK = "•";
        private const double THRESHOLD = 30;

        private EditText _pinInputField;
        private LinearLayout _mainLayout;

        private TouchPoint _lastPoint = new TouchPoint(-1, -1);

        public PinWidget(Context context) : this(context, null) { }

        public PinWidget(Context context, IAttributeSet attribs) : this(context, attribs, 0) { }

        public PinWidget(Context context, IAttributeSet attribs, int defStyleAttr) : base(context, attribs, defStyleAttr)
        {
            this.FillViewport = true;
            ScrollBarStyle = ScrollbarStyles.OutsideInset;

            TypedArray array = context.ObtainStyledAttributes(attribs, Resource.Styleable.PinView);

            var method = array.GetString(Resource.Styleable.PinView_onPinCompleted);
            PinListener = new PrivatePinListener(method, this);

            DigitCount = (uint)array.GetInt(Resource.Styleable.PinView_digitCount, PinDefaults.DIGIT_COUNT);

            DisplayMetrics metrics = Resources.DisplayMetrics;

            // Digit dimensions
            DigitWidth = array.GetDimensionPixelSize(Resource.Styleable.PinView_digitWidth, (int) TypedValue.ApplyDimension(ComplexUnitType.Dip, PinDefaults.DIGIT_WIDTH, metrics));
            DigitHeight = array.GetDimensionPixelSize(Resource.Styleable.PinView_digitHeight, (int) TypedValue.ApplyDimension(ComplexUnitType.Dip, PinDefaults.DIGIT_HEIGHT, metrics));
            DigitSpacing = array.GetDimensionPixelSize(Resource.Styleable.PinView_digitSpacing, (int) TypedValue.ApplyDimension(ComplexUnitType.Dip, PinDefaults.DIGIT_SPACING, metrics));
            TextSize = array.GetDimensionPixelSize(Resource.Styleable.PinView_textSize, (int) TypedValue.ApplyDimension(ComplexUnitType.Dip, PinDefaults.TEXT_SIZE, metrics));

            TypedValue resolvedColor = new TypedValue();
            Resources.Theme theme = context.Theme;

            // Colors
            theme.ResolveAttribute(Android.Resource.Attribute.TextColorPrimary, resolvedColor, true);
            TextColor = array.GetColor(Resource.Styleable.PinView_textColor,
                resolvedColor.ResourceId > 0 ? ContextCompat.GetColor(context, resolvedColor.ResourceId) : resolvedColor.Data);

            DigitBorderColor = array.GetColor(Resource.Styleable.PinView_borderColor, TextColor);

            resolvedColor = new TypedValue();
            theme.ResolveAttribute(Android.Resource.Attribute.ColorAccent, resolvedColor, true);
            AccentColor = array.GetColor(Resource.Styleable.PinView_accentColor,
                resolvedColor.ResourceId > 0 ? ContextCompat.GetColor(context, resolvedColor.ResourceId) : resolvedColor.Data);

            resolvedColor = new TypedValue();
            theme.ResolveAttribute(Android.Resource.Attribute.WindowBackground, resolvedColor, true);
            DigitBackgroundColor = array.GetColor(Resource.Styleable.PinView_digitBackgroundColor,
                resolvedColor.ResourceId > 0 ? ContextCompat.GetColor(context, resolvedColor.ResourceId) : resolvedColor.Data);
            
            AccentHeight = (int)array.GetDimension(Resource.Styleable.PinView_accentHeight, TypedValue.ApplyDimension(ComplexUnitType.Dip, PinDefaults.ACCENT_HEIGHT, metrics));

            array.Recycle();

            LoadViews();
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            TouchPoint point = new TouchPoint(e.GetX(), e.GetY());
            if (e.Action == MotionEventActions.Down)
            {
                _lastPoint = point;
            }
            else if (e.Action == MotionEventActions.Up)
            {
                var newPos = point;

                double dist = point.DisplacementFrom(_lastPoint);

                if (dist < THRESHOLD)
                {
                    _pinInputField.RequestFocus();

                    ShowKeyboard();
                    return true;
                }
            }
            else if (e.Action == MotionEventActions.Move)
            {
                double dist = point.DisplacementFrom(_lastPoint);
                if (dist > THRESHOLD)
                {
                    _lastPoint = new TouchPoint(-1, -1);
                }
            }
            return base.OnTouchEvent(e);
        }

        protected override IParcelable OnSaveInstanceState()
        {
            var initState = base.OnSaveInstanceState();
            var state = new PinSavedState(initState, _pinInputField.Text);
            return state;
        }

        protected override void OnRestoreInstanceState(IParcelable state)
        {
            var pinState = (PinSavedState)state;
            base.OnRestoreInstanceState(pinState.SuperState);
            _pinInputField.Text = pinState.Pin;
            _pinInputField.SetSelection(_pinInputField.Text.Length);
            CenterSelectedDigit();
        }

        private Drawable SetupDrawable()
        {
            ColorDrawable box = new ColorDrawable(DigitBorderColor);
            ColorDrawable background = new ColorDrawable(DigitBackgroundColor);
            ColorDrawable accent = new ColorDrawable(AccentColor);

            LayerDrawable selectedBackground = new LayerDrawable(new Drawable[] { box, accent, background });
            selectedBackground.SetLayerInset(1, 5, 5, 5, 5);
            selectedBackground.SetLayerInset(2, 5, 5, 5, 5 + AccentHeight);

            LayerDrawable defaultBackground = new LayerDrawable(new Drawable[] { box, background });
            defaultBackground.SetLayerInset(1, 5, 5, 5, 5);

            StateListDrawable stateListDrawable = new StateListDrawable();
            stateListDrawable.AddState(new int[] { Android.Resource.Attribute.StateSelected }, selectedBackground);
            stateListDrawable.AddState(new int[] { }, defaultBackground);

            return stateListDrawable;
        }

        private void LoadViews()
        {
            this.RemoveAllViews();

            LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent);
            layoutParams.Gravity = GravityFlags.Center;

            _mainLayout = new LinearLayout(Context);
            _mainLayout.LayoutParameters = layoutParams;
            _mainLayout.SetGravity(GravityFlags.Center);
            _mainLayout.Orientation = Android.Widget.Orientation.Horizontal;

            this.AddView(_mainLayout);

            LinearLayout.LayoutParams digitParams = new LinearLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
            digitParams.SetMargins(DigitSpacing / 2, 0, DigitSpacing / 2, 0);
            digitParams.Gravity = GravityFlags.Center;

            // Add a digit view for each digit
            for (int i = 0; i < DigitCount; i++)
            {
                DigitView digitView = new DigitView(Context);
                digitView.LayoutParameters = digitParams;
                digitView.SetWidth(DigitWidth);
                digitView.SetHeight(DigitHeight);

                digitView.Background = SetupDrawable();

                digitView.SetTextColor(TextColor);
                digitView.TextSize = TextSize;
                digitView.Gravity = GravityFlags.Center;

                _mainLayout.AddView(digitView);
            }

            _pinInputField = new EditText(Context);
            _pinInputField.TextSize = 0;
            _pinInputField.SetBackgroundColor(Color.Transparent);
            _pinInputField.SetTextColor(Color.Transparent);
            _pinInputField.SetCursorVisible(false);
            _pinInputField.SetFilters(new IInputFilter[] { new InputFilterLengthFilter((int)DigitCount) });
            _pinInputField.InputType = InputTypes.ClassNumber;
            _pinInputField.ImeOptions = (ImeAction)ImeFlags.NoExtractUi;
            _pinInputField.MovementMethod = null;

            _pinInputField.FocusChange -= ViewFocused;
            _pinInputField.FocusChange += ViewFocused;

            _pinInputField.TextChanged -= PinChanging;
            _pinInputField.TextChanged += PinChanging;

            _mainLayout.AddView(_pinInputField);
        }

        private void CenterSelectedDigit()
        {
            DigitView selected = null;
            for (int i = 0; i < _mainLayout.ChildCount - 1; i++)
            {
                selected = (DigitView)_mainLayout.GetChildAt(i);
                if (selected.Selected)
                {
                    break;
                }
                selected = null;
            }

            if (selected != null)
            {
                int width = Resources.DisplayMetrics.WidthPixels / 2;
                this.SmoothScrollTo((int)selected.GetX() - width, (int)selected.GetY());
            }
        }

        #region Keyboard Methods
        private void ShowKeyboard()
        {
            InputMethodManager inputMethodManager = (InputMethodManager)Context.GetSystemService(Context.InputMethodService);
            inputMethodManager.ShowSoftInput(_pinInputField, 0);
        }

        private void HideKeyboard()
        {
            InputMethodManager inputMethodManager = (InputMethodManager)Context.GetSystemService(Context.InputMethodService);
            inputMethodManager.HideSoftInputFromWindow(_pinInputField?.WindowToken, HideSoftInputFlags.None);
        }
        #endregion

        #region Listeners
        private void PinChanging(object sender, TextChangedEventArgs e)
        {
            int length = string.Concat(e.Text).Length;

            for (int i = 0; i < DigitCount; i++)
            {
                DigitView digit = (DigitView)_mainLayout.GetChildAt(i);
                if (length > i)
                {
                    digit.Text = MASK;
                }
                else
                {
                    digit.Text = string.Empty;
                }

                if (_pinInputField.HasFocus || _pinInputField.HasWindowFocus)
                {
                    if (i == length)
                    {
                        digit.Selected = true;
                        CenterSelectedDigit();
                    }
                    else
                    {
                        digit.Selected = false;
                    }
                }
            }

            if (length == DigitCount)
            {
                // Finished
                var handler = PinCompleted;
                handler?.Invoke(this, new PinCompletedEventArgs(_pinInputField.Text));
                PinListener?.PinEntered(_pinInputField.Text);
                _pinInputField.ClearFocus();
                HideKeyboard();
            }
        }

        private void ViewFocused(object sender, FocusChangeEventArgs e)
        {
            int length = _pinInputField.Text.Length;
            
            for (int i = 0; i < DigitCount; i++)
            {
                View view = _mainLayout.GetChildAt(i);
                if (view == null)
                {
                    break;
                }
                view.Selected = e.HasFocus && length == i;

                if (view.Selected)
                {
                    CenterSelectedDigit();
                }
            }

            // Push cursor to the end.
            _pinInputField.SetSelection(length);
            ShowKeyboard();
        }
        #endregion
    }

    internal class PrivatePinListener : IOnPinCompletedListener
    {
        private string _pinFinishedMethod;
        private View _view;

        public PrivatePinListener(string pinFinishedMethod, PinWidget view)
        {
            this._pinFinishedMethod = pinFinishedMethod;
            this._view = view;
        }

        public void PinEntered(string pin)
        {
            if (string.IsNullOrWhiteSpace(_pinFinishedMethod))
            {
                return;
            }

            var method = _view.Context.GetType().GetMethod(_pinFinishedMethod, BindingFlags.Public | BindingFlags.Instance);
            method.Invoke(_view.Context, new object[] { pin });
        }
    }
}
