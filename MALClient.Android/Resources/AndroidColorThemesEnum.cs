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
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.Resources
{
    public enum AndroidColorThemes
    {
        Orange,
        Purple,
        Blue,
        Lime,
        Pink,
        Cyan,
        SkyBlue,
        Red
    }

    public static class AndroidColourThemeHelper
    {
        public static AndroidColorThemes CurrentTheme
        {
            get
            {
                var raw = (int)(ResourceLocator.ApplicationDataService[nameof(AndroidColorThemes)] ?? 0);
                // Clamp out-of-range values (e.g. the removed MaterialYou option)
                // back to Orange instead of crashing theme switches.
                if (raw < 0 || raw > (int)AndroidColorThemes.Red)
                    raw = (int)AndroidColorThemes.Orange;
                return (AndroidColorThemes)raw;
            }
            set { ResourceLocator.ApplicationDataService[nameof(AndroidColorThemes)] = (int) value; }
        }

        /// <summary>
        /// True when the Material You dynamic-color override is active
        /// (enabled in settings and running on Android 12+).
        /// </summary>
        public static bool MaterialYouActive =>
            Settings.MaterialYouEnabled && Build.VERSION.SdkInt >= BuildVersionCodes.S;
    }
}