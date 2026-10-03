using MALClient.Models.Enums;
using MALClient.Models.Models.Anime;
using MALClient.XShared.Comm.Manga;
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
    public class AnimeGeneralDetailsQuery : Query
    {
        public async Task<AnimeGeneralDetailsData> GetAnimeDetails(bool force, string id, string title, bool animeMode,
            ApiType? apiOverride = null)
        {
            var output = force ? null : await DataCache.RetrieveAnimeSearchResultsData(id, animeMode);
            if (output != null)
                return output;

            var requestedApiType = apiOverride ?? CurrentApiType;
            var response = string.Empty;
            var client = await ResourceLocator.MalHttpContextProvider.GetApiHttpContextAsync();
            try
            {
                switch (requestedApiType)
                {
                    case ApiType.Mal:

                            if (animeMode)
                            {
                                var apiUrl = $"https://api.myanimelist.net/v2/anime/{id}?fields=num_episodes,status,media_type,alternative_titles,start_date,end_date,main_picture,pictures,mean,id,synopsis,title";
                                var result = await MalApiClient.GetJsonAsync<AnimeEntry>(client, apiUrl);
                                output = new AnimeGeneralDetailsData
                                {
                                    AllEpisodes = (int)(result.Episodes ?? 0),
                                    Status = MalApiHelpers.PrettyAnimeStatus(result.Status),
                                    Type = MalApiHelpers.PrettyAnimeMediaType(result.Type),
                                    AlternateTitle = result.AlternativeTitle?.Japanese,
                                    StartDate = result.StartDate ?? "N/A",
                                    EndDate = result.EndDate ?? "N/A",
                                    ImgUrl = result.Picture?.Medium,
                                    GlobalScore = (float) (result.Score ?? 0),
                                    Id = (int)result.MalId,
                                    MalId = (int)result.MalId,
                                    Synopsis = result.Synopsis,
                                    Title = result.Title,
                                    Synonyms = result.AlternativeTitle?.Synonyms?.ToList() ?? new List<string>(),
                                };

                                if ((output.Type == "Movie" || output.AllEpisodes == 1) && output.EndDate == "N/A" &&
                                    output.Status == "Finished Airing")
                                {
                                    output.EndDate = output.StartDate;
                                }

                                ResourceLocator.EnglishTitlesProvider.AddOrUpdate(int.Parse(id), true,
                                    result.AlternativeTitle?.English);
                            }
                            else
                            {
                                var apiUrl = $"https://api.myanimelist.net/v2/manga/{id}?fields=id,title,main_picture,alternative_titles,start_date,end_date,synopsis,mean,status,media_type,num_volumes,num_chapters";
                                var result = await MalApiClient.GetJsonAsync<MangaEntry>(client, apiUrl);

                                output = new AnimeGeneralDetailsData
                                {
                                    AllEpisodes = (int)(result.Chapters ?? 0),
                                    AllVolumes = (int)(result.Volumes ?? 0),
                                    Status = MalApiHelpers.PrettyMangaStatus(result.Status),
                                    Type = MalApiHelpers.PrettyMangaMediaType(result.Type),
                                    AlternateTitle = result.AlternativeTitle?.Japanese,
                                    StartDate = result.StartDate ?? "N/A",
                                    EndDate = result.EndDate ?? "N/A",
                                    ImgUrl = result.Picture?.Medium,
                                    GlobalScore = (float)(result.Score ?? 0),
                                    Id = (int)result.MalId,
                                    MalId = (int)result.MalId,
                                    Synopsis = result.Synopsis,
                                    Title = result.Title,
                                    Synonyms = result.AlternativeTitle?.Synonyms?.ToList() ?? new List<string>(),
                                };

                                ResourceLocator.EnglishTitlesProvider.AddOrUpdate(int.Parse(id), false,
                                    result.AlternativeTitle?.English);
                            }



                            DataCache.SaveAnimeSearchResultsData(id, output, animeMode);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception e)
            {
                //ResourceLocator.ClipboardProvider.SetText($"{e}\n{response}");
                //ResourceLocator.SnackbarProvider.ShowText("Error copied to clipboard.");
                // todo android notification nav bug
                // probably MAl garbled response
            }

            return output;
        }
    }
}