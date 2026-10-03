using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using MALClient.Models.Models.AnimeScrapped;
using MALClient.XShared.JsonModels.MAL;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;

namespace MALClient.XShared.Comm.Anime
{
    public enum TopAnimeType
    {
        General,
        Airing,
        Upcoming,
        Tv,
        Movies,
        Ovas,
        Popular,
        Favourited,
        Manga
    }

    /// <summary>
    /// Top-manga categories, mirroring the official MAL API v2 manga ranking types.
    /// (Previously supplied by JikanDotNet; redefined locally after the official-API migration.)
    /// </summary>
    public enum MangaTopType
    {
        All,
        Manga,
        Novels,
        Oneshots,
        Doujin,
        Manhwa,
        Manhua,
        ByPopularity,
        Favorite
    }


    public class AnimeTopQuery : Query
    {
        private static Dictionary<TopAnimeType, List<TopAnimeData>> _prevQueriesCache = new Dictionary<TopAnimeType, List<TopAnimeData>>();
        private readonly TopAnimeType _type;
        private readonly MangaTopType? _mangaTopType;
        private readonly int _page;

        public AnimeTopQuery(TopAnimeType topType, int page = 0)
        {
            _page = page;
            _type = topType;
        }

        public AnimeTopQuery(MangaTopType mangaTopType, int page = 0)
        {
            _page = page;
            _type = TopAnimeType.Manga;
            _mangaTopType = mangaTopType;
        }

        private static string GetRankingType(TopAnimeType type)
        {
            switch (type)
            {
                case TopAnimeType.General:
                    return "all";
                case TopAnimeType.Airing:
                    return "airing";
                case TopAnimeType.Upcoming:
                    return "upcoming";
                case TopAnimeType.Tv:
                    return "tv";
                case TopAnimeType.Movies:
                    return "movie";
                case TopAnimeType.Ovas:
                    return "ova";
                case TopAnimeType.Popular:
                    return "bypopularity";
                case TopAnimeType.Favourited:
                    return "favorite";
                case TopAnimeType.Manga:
                    return "manga";
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private static string GetMangaRankingType(MangaTopType type)
        {
            switch (type)
            {
                case MangaTopType.All:
                    return "all";
                case MangaTopType.Manga:
                    return "manga";
                case MangaTopType.Novels:
                    return "novels";
                case MangaTopType.Oneshots:
                    return "oneshots";
                case MangaTopType.Doujin:
                    return "doujin";
                case MangaTopType.Manhwa:
                    return "manhwa";
                case MangaTopType.Manhua:
                    return "manhua";
                case MangaTopType.ByPopularity:
                    return "bypopularity";
                case MangaTopType.Favorite:
                    return "favorite";
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public async Task<List<TopAnimeData>> GetTopAnimeData(bool force = false)
        {
            if (_mangaTopType.HasValue)
                return await GetTopMangaDataByType(force);

            if (!force)
                if (_prevQueriesCache.ContainsKey(_type))
                    return _prevQueriesCache[_type];

            var output = force ? new List<TopAnimeData>() : (await DataCache.RetrieveTopAnimeData(_type) ?? new List<TopAnimeData>());
            if (output.Count > 0)
            {
                _prevQueriesCache[_type] = output;
                return output;
            }

            try
            {
                var client = await ResourceLocator.MalHttpContextProvider.GetApiHttpContextAsync();
                var rankingType = GetRankingType(_type);
                var offset = _page * 50;

                if (_type == TopAnimeType.Manga)
                {
                    var apiUrl =
                        $"https://api.myanimelist.net/v2/manga/ranking?ranking_type={rankingType}&limit=50&offset={offset}&nsfw=true&fields=id,title,main_picture,mean,num_volumes,num_chapters";
                    var ranking =
                        JsonSerializer.Deserialize<PaginatedMALResponse<ICollection<RankingEntry<MangaEntry>>>>(
                            await client.GetStringAsync(apiUrl));

                    foreach (var entry in ranking.Data)
                    {
                        var node = entry.Node;
                        output.Add(new TopAnimeData
                        {
                            Title = node.Title,
                            Id = (int)node.MalId,
                            ImgUrl = node.Picture?.Medium,
                            Episodes = (node.Volumes ?? 0).ToString(),
                            Score = (float)(node.Score ?? 0),
                            Index = entry.Ranking?.Rank ?? 0
                        });
                    }
                }
                else
                {
                    var apiUrl =
                        $"https://api.myanimelist.net/v2/anime/ranking?ranking_type={rankingType}&limit=50&offset={offset}&nsfw=true&fields=id,title,main_picture,mean,num_episodes";
                    var ranking =
                        JsonSerializer.Deserialize<PaginatedMALResponse<ICollection<RankingEntry<AnimeEntry>>>>(
                            await client.GetStringAsync(apiUrl));

                    foreach (var entry in ranking.Data)
                    {
                        var node = entry.Node;
                        output.Add(new TopAnimeData
                        {
                            Title = node.Title,
                            Id = (int)node.MalId,
                            ImgUrl = node.Picture?.Medium,
                            Episodes = (node.Episodes ?? 0).ToString(),
                            Score = (float)(node.Score ?? 0),
                            Index = entry.Ranking?.Rank ?? 0
                        });
                    }
                }
            }
            catch (Exception)
            {
                return new List<TopAnimeData>();
            }

            if (_page != 0 && _prevQueriesCache.ContainsKey(_type)) //merge data
                output = _prevQueriesCache[_type].Union(output).Distinct().ToList();

            DataCache.SaveTopAnimeData(output, _type);
            _prevQueriesCache[_type] = output;
            return output;
        }

        private async Task<List<TopAnimeData>> GetTopMangaDataByType(bool force = false)
        {
            var mangaType = _mangaTopType.Value;
            var output = force
                ? new List<TopAnimeData>()
                : (await DataCache.RetrieveTopMangaData(mangaType) ?? new List<TopAnimeData>());
            if (output.Count > 0)
                return output;

            try
            {
                var client = await ResourceLocator.MalHttpContextProvider.GetApiHttpContextAsync();
                var rankingType = GetMangaRankingType(mangaType);
                var offset = _page * 50;
                var apiUrl =
                    $"https://api.myanimelist.net/v2/manga/ranking?ranking_type={rankingType}&limit=50&offset={offset}&nsfw=true&fields=id,title,main_picture,mean,num_volumes,num_chapters";
                var ranking =
                    JsonSerializer.Deserialize<PaginatedMALResponse<ICollection<RankingEntry<MangaEntry>>>>(
                        await client.GetStringAsync(apiUrl));

                foreach (var entry in ranking.Data)
                {
                    var node = entry.Node;
                    output.Add(new TopAnimeData
                    {
                        Title = node.Title,
                        Id = (int)node.MalId,
                        ImgUrl = node.Picture?.Medium,
                        Episodes = (node.Volumes ?? 0).ToString(),
                        Score = (float)(node.Score ?? 0),
                        Index = entry.Ranking?.Rank ?? 0
                    });
                }
            }
            catch (Exception)
            {
                return new List<TopAnimeData>();
            }

            DataCache.SaveTopMangaData(output, mangaType);
            return output;
        }
    }
}
