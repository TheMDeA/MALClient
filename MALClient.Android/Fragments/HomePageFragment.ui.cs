using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading.Views;
using MALClient.Android.Resources;

namespace MALClient.Android.Fragments
{
    public partial class HomePageFragment
    {
        private ImageViewAsync _homeHeroImage;
        public ImageViewAsync HomeHeroImage =>
            _homeHeroImage ?? (_homeHeroImage = FindViewById<ImageViewAsync>(Resource.Id.HomeHeroImage));

        private TextView _homeHeroTitle;
        public TextView HomeHeroTitle =>
            _homeHeroTitle ?? (_homeHeroTitle = FindViewById<TextView>(Resource.Id.HomeHeroTitle));

        private TextView _homeHeroMeta;
        public TextView HomeHeroMeta =>
            _homeHeroMeta ?? (_homeHeroMeta = FindViewById<TextView>(Resource.Id.HomeHeroMeta));

        private Button _homeHeroCta;
        public Button HomeHeroCta =>
            _homeHeroCta ?? (_homeHeroCta = FindViewById<Button>(Resource.Id.HomeHeroCta));

        private Button _homeQuickDiscover;
        public Button HomeQuickDiscover =>
            _homeQuickDiscover ?? (_homeQuickDiscover = FindViewById<Button>(Resource.Id.HomeQuickDiscover));

        private Button _homeQuickSchedule;
        public Button HomeQuickSchedule =>
            _homeQuickSchedule ?? (_homeQuickSchedule = FindViewById<Button>(Resource.Id.HomeQuickSchedule));

        private Button _homeQuickMyList;
        public Button HomeQuickMyList =>
            _homeQuickMyList ?? (_homeQuickMyList = FindViewById<Button>(Resource.Id.HomeQuickMyList));

        private TextView _homeContinueTitle;
        public TextView HomeContinueTitle =>
            _homeContinueTitle ?? (_homeContinueTitle = FindViewById<TextView>(Resource.Id.HomeContinueTitle));

        private RecyclerView _homeContinueRail;
        public RecyclerView HomeContinueRail =>
            _homeContinueRail ?? (_homeContinueRail = FindViewById<RecyclerView>(Resource.Id.HomeContinueRail));

        private RecyclerView _homeSpotlightRail;
        public RecyclerView HomeSpotlightRail =>
            _homeSpotlightRail ?? (_homeSpotlightRail = FindViewById<RecyclerView>(Resource.Id.HomeSpotlightRail));
    }
}
