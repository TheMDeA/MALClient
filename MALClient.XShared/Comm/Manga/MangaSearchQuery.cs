using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using MALClient.Models.Models.Anime;
using MALClient.XShared.Comm;
using MALClient.XShared.JsonModels.MAL;
using MALClient.XShared.ViewModels;

namespace MALClient.XShared.Comm.Manga
{
    public class MangaSearchQuery : Query
    {
        private readonly string _query;

        public MangaSearchQuery(string query)
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
                    $"https://api.myanimelist.net/v2/manga?q={Uri.EscapeDataString(_query)}&limit=50&nsfw=true&fields=id,title,main_picture,mean,media_type,num_volumes,num_chapters,synopsis,status";
                var searchResult =
                    JsonSerializer.Deserialize<PaginatedMALResponse<ICollection<AnimeNode<MangaEntry>>>>(
                        await client.GetStringAsync(apiUrl));

                foreach (var result in searchResult.Data)
                {
                    var node = result.Node;
                    output.Add(new AnimeGeneralDetailsData
                    {
                        Id = (int)node.MalId,
                        MalId = (int)node.MalId,
                        AllEpisodes = (int)(node.Chapters ?? 0),
                        AllVolumes = (int)(node.Volumes ?? 0),
                        Title = node.Title,
                        ImgUrl = node.Picture?.Medium,
                        Type = MalApiHelpers.PrettyMangaMediaType(node.Type),
                        Synopsis = node.Synopsis,
                        GlobalScore = (float)(node.Score ?? 0),
                        Status = MalApiHelpers.PrettyMangaStatus(node.Status)
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
