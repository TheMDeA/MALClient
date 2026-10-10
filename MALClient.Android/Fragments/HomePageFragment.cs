using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AoLibs.Adapters.Android.Recycler;
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Listeners;
using MALClient.Android.Resources;
using MALClient.Android.Utilities;
using MALClient.Models.Enums;
using MALClient.Models.Models.AnimeScrapped;
using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Fragments
{
    /// <summary>
    /// Redesign home screen: seasonal hero, continue-watching rail,
    /// airing spotlight rail and quick links.
    /// </summary>
    public partial class HomePageFragment : MalFragmentBase
    {
        private HomePageViewModel ViewModel;

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = ViewModelLocator.HomePage;
            InitAsync();
        }

        private async void InitAsync()
        {
            await ViewModel.Init();
        }

        protected override void InitBindings()
        {
            HomeContinueRail.SetLayoutManager(
                new LinearLayoutManager(Activity, LinearLayoutManager.Horizontal, false));
            HomeSpotlightRail.SetLayoutManager(
                new LinearLayoutManager(Activity, LinearLayoutManager.Horizontal, false));

            // Adapters subscribe to the collections' change events, so rails
            // populate themselves as data arrives.
            HomeContinueRail.SetAdapter(new ObservableRecyclerAdapter<AnimeItemViewModel, RailHolder>(
                ViewModel.ContinueWatching,
                (item, holder, position) =>
                {
                    holder.Title.Text = item.Title;
                    holder.Subtitle.Text = item.MyEpisodesBindShort;
                    holder.Image.AnimeInto(item.ImgUrl);
                    holder.ItemView.SetOnClickListener(new OnClickListener(_ => item.NavigateDetails()));
                },
                LayoutInflater,
                Resource.Layout.HomeRailItem));
            HomeSpotlightRail.SetAdapter(new ObservableRecyclerAdapter<SeasonalAnimeData, RailHolder>(
                ViewModel.AiringSpotlight,
                (item, holder, position) =>
                {
                    holder.Title.Text = item.Title;
                    holder.Subtitle.Text = item.Score > 0 ? $"★ {item.Score:N2}" : string.Empty;
                    holder.Image.AnimeInto(item.ImgUrl);
                    holder.ItemView.SetOnClickListener(new OnClickListener(_ =>
                        ViewModelLocator.GeneralMain.Navigate(PageIndex.PageAnimeDetails,
                            new AnimeDetailsPageNavigationArgs(item.Id, item.Title, null, null))));
                },
                LayoutInflater,
                Resource.Layout.HomeRailItem));

            ViewModel.ContinueWatching.CollectionChanged += (s, e) => UpdateContinueVisibility();
            UpdateContinueVisibility();

            Bindings.Add(this.SetBinding(() => ViewModel.HeroItem).WhenSourceChanges(() =>
            {
                var hero = ViewModel.HeroItem;
                if (hero == null)
                    return;
                HomeHeroTitle.Text = hero.Title;
                HomeHeroMeta.Text = hero.Score > 0 ? $"★ {hero.Score:N2}" : string.Empty;
                HomeHeroImage.AnimeInto(hero.ImgUrl);
            }));

            HomeHeroCta.SetOnClickListener(new OnClickListener(_ =>
            {
                var hero = ViewModel.HeroItem;
                if (hero == null)
                    return;
                ViewModelLocator.GeneralMain.Navigate(PageIndex.PageAnimeDetails,
                    new AnimeDetailsPageNavigationArgs(hero.Id, hero.Title, null, null));
            }));

            HomeQuickDiscover.SetOnClickListener(new OnClickListener(_ => ViewModel.OpenDiscoverCommand.Execute(null)));
            HomeQuickSchedule.SetOnClickListener(new OnClickListener(_ => ViewModel.OpenScheduleCommand.Execute(null)));
            HomeQuickMyList.SetOnClickListener(new OnClickListener(_ => ViewModel.OpenMyListCommand.Execute(null)));
        }

        public override int LayoutResourceId => Resource.Layout.HomePageFragment;

        public static HomePageFragment Instance => new HomePageFragment();

        private void UpdateContinueVisibility()
        {
            var visible = ViewModel.ContinueWatching.Count > 0;
            HomeContinueTitle.Visibility = visible ? ViewStates.Visible : ViewStates.Gone;
            HomeContinueRail.Visibility = visible ? ViewStates.Visible : ViewStates.Gone;
        }

        private class RailHolder : RecyclerView.ViewHolder
        {
            private readonly View _view;

            public RailHolder(View view) : base(view)
            {
                _view = view;
            }

            private ImageViewAsync _image;
            public ImageViewAsync Image =>
                _image ?? (_image = _view.FindViewById<ImageViewAsync>(Resource.Id.HomeRailItemImage));

            private TextView _title;
            public TextView Title =>
                _title ?? (_title = _view.FindViewById<TextView>(Resource.Id.HomeRailItemTitle));

            private TextView _subtitle;
            public TextView Subtitle =>
                _subtitle ?? (_subtitle = _view.FindViewById<TextView>(Resource.Id.HomeRailItemSubtitle));
        }
    }
}
