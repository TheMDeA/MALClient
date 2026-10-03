using MALClient.Models.Models.Anime;
using MALClient.Models.Models.AnimeScrapped;
using MALClient.XShared.JsonModels.MAL;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MALClient.XShared.Comm.Anime
{
    public class AnimeSeasonalQuery : Query
    {
        private readonly AnimeSeason _season;

        public AnimeSeasonalQuery(AnimeSeason season)
        {
            _season = season;
        }

        public async Task<List<SeasonalAnimeData>> GetSeasonalAnime(bool force = false)
        {
            var output = force /*|| DataCache.SeasonalUrls?.Count == 0*/ //either force or urls are empty after update
                ? new List<SeasonalAnimeData>()
                : await DataCache.RetrieveSeasonalData(_season.Name) ?? new List<SeasonalAnimeData>();
            //current season without suffix
            if (output.Count != 0) return output;

            var client = await ResourceLocator.MalHttpContextProvider.GetApiHttpContextAsync();
            try
            {
                while (true)
                {
                    var requestedYear = _season.Year != 0 ? _season.Year : DateTime.UtcNow.Year;

                    var requestedSeason = _season.Year != 0
                        ? _season.Season
                        : DateTime.UtcNow.Month switch
                        {
                            <= 3 => JikanDotNet.Season.Winter,
                            <= 6 => JikanDotNet.Season.Spring,
                            <= 9 => JikanDotNet.Season.Summer,
                            _ => JikanDotNet.Season.Fall
                        };
                    // Hoisted out of the sort key: previously ToString().ToLower() ran
                    // on every comparison (O(n log n) throwaway strings).
                    var requestedSeasonName = requestedSeason.ToString().ToLower();

                    try
                    {
                        var apiUrl = $"https://api.myanimelist.net/v2/anime/season/{requestedYear}/{requestedSeasonName}?limit=500&nsfw=true&fields=id,title,main_picture,num_episodes,mean,genres,num_list_users,start_season";
                        var season = await MalApiClient.GetJsonAsync<PaginatedMALResponse<ICollection<AnimeNode<SeasonEntry>>>>(client, apiUrl);
                        // OrderByDescending instead of OrderBy(...).Reverse(): single pass,
                        // no extra enumerator allocation.
                        var orderedData = season.Data
                            .OrderByDescending(seasonEntry =>
                                ((seasonEntry.Node.StartSeason?.Name == requestedSeasonName
                                  && seasonEntry.Node.StartSeason?.Year == requestedYear) ? 100000000 : 0)
                                + (seasonEntry.Node.MembersCount ?? 0))
                            .ToList();

                        // Indexed loop instead of foreach + FindIndex extension (which
                        // re-scanned the list per item: O(n^2) for up to 500 entries).
                        for (var i = 0; i < orderedData.Count; i++)
                        {
                            var seasonEntry = orderedData[i];
                            output.Add(new SeasonalAnimeData
                            {
                                Title = seasonEntry.Node.Title,
                                Id = (int)(seasonEntry.Node.MalId ?? -1),
                                ImgUrl = seasonEntry.Node.Picture?.Medium,
                                Episodes = (seasonEntry.Node.Episodes ?? 0).ToString(),
                                Score = (float)(seasonEntry.Node.Score ?? 0),
                                Genres = (seasonEntry.Node.Genres ?? new List<Genre>()).Select(item => item.Name).ToList(),
                                Index = i
                            });
                        }

                        break;
                    }
                    catch (HttpRequestException e)
                    {
                        if (e.Message.Contains("429"))
                            await Task.Delay(TimeSpan.FromSeconds(1));
                        else
                            throw;
                    }
                }

                DataCache.SaveSeasonalData(output, _season.Name);

                //We are done.
                return output;
            }
            catch (Exception e)
            {
                return output;
            }
        }
    }
}
