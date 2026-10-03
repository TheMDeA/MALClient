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
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Activities;
using MALClient.Android.BindingConverters;
using MALClient.Android.Flyouts;
using MALClient.Android.Listeners;
using MALClient.Android.Resources;
using MALClient.Android.ViewModels;
using MALClient.Models.Enums;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.Fragments.SettingsFragments
{
    public partial class SettingsCalendarFragment : MalFragmentBase
    {
        private SettingsViewModel ViewModel;

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = AndroidViewModelLocator.Settings;
        }

        protected override void InitBindings()
        {
            Bindings.Add(
                this.SetBinding(() => ViewModel.CalendarSeasonalView,
                    () => SettingsPageCalendarSeasonalViewSwitch.Checked, BindingMode.TwoWay));
            // Rebuild the calendar in the background when the user changes the
            // content mode, so it applies the next time the page is opened.
            // (WhenSourceChanges can't be used here: MVVM Light forbids it on a
            // binding that already has a target expression.)
            var lastBuiltSeasonalView = ViewModel.CalendarSeasonalView;
            SettingsPageCalendarSeasonalViewSwitch.CheckedChange += (sender, args) =>
            {
                if (args.IsChecked == lastBuiltSeasonalView)
                    return; // programmatic set from the binding above
                lastBuiltSeasonalView = args.IsChecked;
                ViewModelLocator.CalendarPage.Init(true);
            };
            //
            SettingsPageCalendarStartPageRadioGroup.Check(Settings.CalendarStartOnToday
                ? SettingsPageCalendarStartPageRadioToday.Id
                : SettingsPageCalendarStartPageRadioSummary.Id);
            SettingsPageCalendarStartPageRadioGroup.SetOnCheckedChangeListener(new OnCheckedListener(i =>
            {
                Settings.CalendarStartOnToday = i == SettingsPageCalendarStartPageRadioToday.Id;
            }));
            //
            
            Bindings.Add(
                this.SetBinding(() => ViewModel.CalendarSwitchMonSun,
                    () => SettingsPageCalendarMiscFirstDaySwitch.Checked, BindingMode.TwoWay));
            Bindings.Add(
                this.SetBinding(() => ViewModel.CalendarRemoveEmptyDays,
                    () => SettingsPageCalendarMiscRemoveEmptyDaysSwitch.Checked, BindingMode.TwoWay));
            //Bindings.Add(
            //    this.SetBinding(() => ViewModel.CalendarPullExactAiringTime,
            //        () => SettingsPageCalendarMiscExactAiringTimeSwitch.Checked, BindingMode.TwoWay));
            
        }

        public override int LayoutResourceId => Resource.Layout.SettingsPageCalendar;
    }
}