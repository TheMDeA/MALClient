using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using JikanDotNet;
using MALClient.Models.Enums;
using MALClient.Models.Models.Anime;
using MALClient.Models.Models.AnimeScrapped;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.Utils;

namespace MALClient.XShared.ViewModels.Main
{
    /// <summary>
    /// Redesign home screen: cinematic seasonal hero, continue-watching rail
    /// and airing spotlight. Reuses the existing anime list and seasonal query
    /// layers — no new data plumbing.
    /// </summary>
    public class HomePageViewModel : ViewModelBase
    {
        public ObservableCollection<AnimeItemViewModel> ContinueWatching { get; } =
            new ObservableCollection<AnimeItemViewModel>();

        public ObservableCollection<SeasonalAnimeData> AiringSpotlight { get; } =
            new ObservableCollection<SeasonalAnimeData>();

        private SeasonalAnimeData _heroItem;
        public SeasonalAnimeData HeroItem
        {
            get => _heroItem;
            private set
            {
                _heroItem = value;
                RaisePropertyChanged(() => HeroItem);
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            private set
            {
                _isLoading = value;
                RaisePropertyChanged(() => IsLoading);
            }
        }

        public bool IsAuthenticated => Credentials.Authenticated;

        public ICommand RefreshCommand { get; }
        public ICommand OpenMyListCommand { get; }
        public ICommand OpenScheduleCommand { get; }
        public ICommand OpenDiscoverCommand { get; }

        private bool _initialized;

        public HomePageViewModel()
        {
            RefreshCommand = new RelayCommand(async () => await LoadAsync(true));
            OpenMyListCommand = new RelayCommand(() => ViewModelLocator.GeneralMain.Navigate(PageIndex.PageAnimeList));
            OpenScheduleCommand = new RelayCommand(() => ViewModelLocator.GeneralMain.Navigate(PageIndex.PageCalendar));
            OpenDiscoverCommand = new RelayCommand(() => ViewModelLocator.GeneralMain.Navigate(PageIndex.PageSearch));
        }

        public async Task Init()
        {
            if (_initialized)
                return;
            _initialized = true;
            await LoadAsync(false);
        }

        private async Task LoadAsync(bool force)
        {
            if (IsLoading)
                return;
            IsLoading = true;
            try
            {
                if (Credentials.Authenticated)
                {
                    var listVm = ViewModelLocator.AnimeList;
                    if (!listVm.AnimeItems.Any() && !listVm.Initializing)
                        await listVm.Init(null); // same as first navigation: default mode + filters
                    var watching = listVm.AnimeItems
                        .Where(i => i.MyStatus == AnimeStatus.Watching)
                        .Take(10)
                        .ToList();
                    ContinueWatching.Clear();
                    foreach (var item in watching)
                        ContinueWatching.Add(item);
                }

                var seasonal = await new AnimeSeasonalQuery(GetCurrentSeason()).GetSeasonalAnime(force);
                var ordered = (seasonal ?? new List<SeasonalAnimeData>())
                    .OrderByDescending(s => s.Score)
                    .Take(6)
                    .ToList();
                AiringSpotlight.Clear();
                foreach (var item in ordered)
                    AiringSpotlight.Add(item);
                HeroItem = ordered.FirstOrDefault();

                RaisePropertyChanged(() => IsAuthenticated);
            }
            catch (Exception)
            {
                // Home stays resilient: sections simply remain empty.
            }
            finally
            {
                IsLoading = false;
            }
        }

        private static AnimeSeason GetCurrentSeason()
        {
            var now = DateTime.UtcNow;
            var allSeasons = new[] { Season.Winter, Season.Spring, Season.Summer, Season.Fall };
            var seasonIndex = now.Month <= 3 ? 0 : now.Month <= 6 ? 1 : now.Month <= 9 ? 2 : 3;
            var season = allSeasons[seasonIndex];
            return new AnimeSeason
            {
                Name = $"{season} {now.Year}",
                Year = now.Year,
                Season = season,
                IsCurrentSeason = true
            };
        }
    }
}
