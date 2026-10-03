using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using MALClient.Models.Enums;
using MALClient.Models.Models.AnimeScrapped;
using MALClient.XShared.JsonModels.MAL;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;

namespace MALClient.XShared.Comm.Anime
{
    public class AnimeRelatedQuery : Query
    {
        private readonly int _animeId;
        private readonly bool _animeMode;

        public AnimeRelatedQuery(int id, bool anime = true)
        {
            _animeId = id;
            _animeMode = anime;
        }

        public async Task<List<RelatedAnimeData>> GetRelatedAnime(bool force = false)
        {
            var output = force
                ? new List<RelatedAnimeData>()
                : await DataCache.RetrieveRelatedAnimeData(_animeId, _animeMode) ?? new List<RelatedAnimeData>();
            if (output.Count != 0) return output;

            try
            {
                var client = await ResourceLocator.MalHttpContextProvider.GetApiHttpContextAsync();
                var endpoint = _animeMode ? "anime" : "manga";
                var apiUrl =
                    $"https://api.myanimelist.net/v2/{endpoint}/{_animeId}?fields=related_anime,related_manga";

                if (_animeMode)
                {
                    var result = JsonSerializer.Deserialize<AnimeEntry>(await client.GetStringAsync(apiUrl));
                    AddRelated(output, result.RelatedAnime, RelatedItemType.Anime);
                    AddRelated(output, result.RelatedManga, RelatedItemType.Manga);
                }
                else
                {
                    var result = JsonSerializer.Deserialize<MangaEntry>(await client.GetStringAsync(apiUrl));
                    AddRelated(output, result.RelatedAnime, RelatedItemType.Anime);
                    AddRelated(output, result.RelatedManga, RelatedItemType.Manga);
                }
            }
            catch (Exception)
            {
                return output;
            }

            DataCache.SaveRelatedAnimeData(_animeId, output, _animeMode);

            return output;
        }

        private static void AddRelated<TNode>(List<RelatedAnimeData> output, ICollection<RelatedEntry<TNode>> entries,
            RelatedItemType type) where TNode : IMalNode
        {
            if (entries == null)
                return;

            foreach (var entry in entries)
            {
                var node = entry.Node;
                if (node?.MalId == null || string.IsNullOrEmpty(node.Title))
                    continue;

                output.Add(new RelatedAnimeData
                {
                    WholeRelation = entry.RelationTypeFormatted ?? entry.RelationType ?? "Related",
                    Id = (int)node.MalId,
                    Title = node.Title,
                    Type = type
                });
            }
        }
    }
}
