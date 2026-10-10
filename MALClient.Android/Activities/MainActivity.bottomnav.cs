using Android.Content.Res;
using Android.Graphics;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Views;
using MALClient.Android.Resources;
using MALClient.Models.Enums;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.Activities
{
    /// <summary>
    /// Redesign: five-tab bottom navigation shell. Tabs drive the existing
    /// MainViewModel.Navigate() so all navigation logic stays in one place.
    /// The hamburger drawer is kept for the remaining features
    /// (forums, clubs, articles, ...).
    /// </summary>
    public partial class MainActivity
    {
        private BottomNavigationView _bottomNav;
        private bool _bottomNavSyncing;

        private void InitBottomNav()
        {
            _bottomNav = FindViewById<BottomNavigationView>(Resource.Id.MainPageBottomNav);
            if (_bottomNav == null)
                return;

            // Note: global:: qualifier is required — inside the MALClient.Android
            // namespaces the unqualified Android.Resource resolves to the app's
            // own generated Resource class (CS0117).
            var states = new int[][]
            {
                new[] { global::Android.Resource.Attribute.StateChecked },
                new int[0]
            };
            var colors = new[]
            {
                new Color(ContextCompat.GetColor(this, Resource.Color.RedesignAccent)).ToArgb(),
                new Color(ResourceExtension.BrushText).ToArgb()
            };
            var tint = new ColorStateList(states, colors);
            _bottomNav.ItemIconTintList = tint;
            _bottomNav.ItemTextColor = tint;

            _bottomNav.SetOnNavigationItemSelectedListener(new BottomNavListener(this));

            ViewModel.PropertyChanged += OnMainViewModelPropertyChanged;
            SyncBottomNavSelection();
        }

        /// <summary>
        /// Listener instead of the NavigationItemSelected event: the event's
        /// nested NavigationItemSelectedEventArgs type breaks the Mono linker
        /// (XALNK7000 resolution failure), the interface does not.
        /// </summary>
        private class BottomNavListener : Java.Lang.Object, BottomNavigationView.IOnNavigationItemSelectedListener
        {
            private readonly MainActivity _activity;

            public BottomNavListener(MainActivity activity)
            {
                _activity = activity;
            }

            public bool OnNavigationItemSelected(IMenuItem item)
            {
                return _activity.OnBottomNavItemSelected(item.ItemId);
            }
        }

        private void OnMainViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.CurrentMainPage))
                SyncBottomNavSelection();
        }

        private bool OnBottomNavItemSelected(int itemId)
        {
            if (_bottomNavSyncing)
                return true;
            var target = BottomNavTarget(itemId);
            if (target == null)
                return false;
            if (ViewModel.CurrentMainPage == target.Value)
                return true; // already there
            ViewModel.Navigate(target.Value);
            return true;
        }

        private PageIndex? BottomNavTarget(int itemId)
        {
            if (itemId == Resource.Id.BottomNavHomeButton)
                return PageIndex.PageHome;
            if (itemId == Resource.Id.BottomNavDiscoverButton)
                return PageIndex.PageSearch;
            if (itemId == Resource.Id.BottomNavScheduleButton)
                return PageIndex.PageCalendar;
            if (itemId == Resource.Id.BottomNavMyListButton)
                return MALClient.XShared.Utils.Settings.DefaultMenuTab == "anime"
                    ? PageIndex.PageAnimeList
                    : PageIndex.PageMangaList;
            if (itemId == Resource.Id.BottomNavProfileButton)
                return PageIndex.PageProfile;
            return null;
        }

        private void SyncBottomNavSelection()
        {
            if (_bottomNav == null || _bottomNavSyncing)
                return;
            int itemId;
            switch (ViewModel.CurrentMainPage)
            {
                case PageIndex.PageHome:
                    itemId = Resource.Id.BottomNavHomeButton;
                    break;
                case PageIndex.PageSearch:
                case PageIndex.PageSeasonal:
                case PageIndex.PageTopAnime:
                case PageIndex.PageTopManga:
                case PageIndex.PageMangaSearch:
                case PageIndex.PageSearchEverywhere:
                    itemId = Resource.Id.BottomNavDiscoverButton;
                    break;
                case PageIndex.PageCalendar:
                    itemId = Resource.Id.BottomNavScheduleButton;
                    break;
                case PageIndex.PageAnimeList:
                case PageIndex.PageMangaList:
                    // Seasonal / top / genre / studio lists share the list page:
                    // keep them under Discover, plain anime/manga lists under My List.
                    switch (ViewModelLocator.AnimeList.WorkMode)
                    {
                        case AnimeListWorkModes.SeasonalAnime:
                        case AnimeListWorkModes.TopAnime:
                        case AnimeListWorkModes.TopManga:
                        case AnimeListWorkModes.AnimeByGenre:
                        case AnimeListWorkModes.AnimeByStudio:
                            itemId = Resource.Id.BottomNavDiscoverButton;
                            break;
                        default:
                            itemId = Resource.Id.BottomNavMyListButton;
                            break;
                    }
                    break;
                case PageIndex.PageProfile:
                    itemId = Resource.Id.BottomNavProfileButton;
                    break;
                default:
                    return; // other pages keep the current tab highlighted
            }
            try
            {
                _bottomNavSyncing = true;
                _bottomNav.SelectedItemId = itemId;
            }
            finally
            {
                _bottomNavSyncing = false;
            }
        }
    }
}
