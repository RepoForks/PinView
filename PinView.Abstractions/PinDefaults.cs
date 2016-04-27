using Android.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinView.Abstractions
{
    public static class PinDefaults
    {
        public const int DIGIT_COUNT = 4;
        public const float DIGIT_WIDTH = 50;
        public const float DIGIT_HEIGHT = 50;
        public const float DIGIT_SPACING = 20;
        public const float TEXT_SIZE = 15;
        public const float ACCENT_HEIGHT = 4;

        public const double DIGIT_ELEVATION = 5;

        public static readonly Color DIGIT_BACKGROUND = Color.Transparent;
        public static readonly Color DIGIT_ACCENT = Color.Transparent;
        public static readonly Color DIGIT_TEXT = Color.Transparent;
    }
}
