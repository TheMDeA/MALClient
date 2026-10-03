using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using MALClient.Models.Enums;
using MALClient.Models.Models.Anime;
using MALClient.XShared.JsonModels.MAL;
using MALClient.XShared.ViewModels;

namespace MALClient.XShared.Comm.Anime
{
    public class AnimeSearchQuery : Query
    {
        private readonly string _query;

        public AnimeSearchQuery(string query, ApiType? apiOverride = null)
        {
            _query = query;
        }

        public async Task<List<AnimeGeneralDetailsData>> GetSearchResults()
        {
            var output = new List<AnimeGeneralDetailsData>();

            try
            {
                var client = await ResourceLocator.MalHttpContextProvider.GetApiHttpContextAsync();
                var apiUrl =
                    $"https://api.myanimelist.net/v2/anime?q={Uri.EscapeDataString(_query)}&limit=50&nsfw=true&fields=id,title,main_picture,mean,media_type,num_episodes,synopsis,status";
                var searchResult =
                    JsonSerializer.Deserialize<PaginatedMALResponse<ICollection<AnimeNode<AnimeEntry>>>>(
                        await client.GetStringAsync(apiUrl));

                foreach (var result in searchResult.Data)
                {
                    var node = result.Node;
                    output.Add(new AnimeGeneralDetailsData
                    {
                        Id = (int)node.MalId,
                        MalId = (int)node.MalId,
                        AllEpisodes = (int)(node.Episodes ?? 0),
                        Title = node.Title,
                        ImgUrl = node.Picture?.Medium,
                        Type = MalApiHelpers.PrettyAnimeMediaType(node.Type),
                        Synopsis = node.Synopsis,
                        GlobalScore = (float)(node.Score ?? 0),
                        Status = MalApiHelpers.PrettyAnimeStatus(node.Status)
                    });
                }
            }
            catch (Exception)
            {
                return output;
            }

            return output;
        }
    }
}
