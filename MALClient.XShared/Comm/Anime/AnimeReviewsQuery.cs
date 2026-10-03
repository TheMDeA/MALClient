using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MALClient.Models.Models.AnimeScrapped;
using MALClient.XShared.Utils;

namespace MALClient.XShared.Comm.Anime
{
    public class AnimeReviewsQuery : Query
    {
        private readonly bool _anime;
        private readonly int _targetId;

        public AnimeReviewsQuery(int id, bool anime = true)
        {
            Request =
                WebRequest.Create(
                    Uri.EscapeUriString($"https://myanimelist.net/{(anime ? "anime" : "manga")}/{id}/whatever/reviews"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
            _targetId = id;
            _anime = anime;
        }

        public async Task<List<AnimeReviewData>> GetAnimeReviews(bool force = false)
        {
            var output = force
                ? new List<AnimeReviewData>()
                : await DataCache.RetrieveReviewsData(_targetId, _anime) ?? new List<AnimeReviewData>();
            if (output.Count != 0) return output;

            try
            {
                var jikan = JikanClient.Jikan;
                // includeSpoiler: true preserves the previous behaviour of fetching every review;
                // spoiler reviews are still flagged via HasSpoilers so the UI can hide them.
                // Routing through JikanDotNet (instead of a raw HttpClient) applies the
                // HTTP/1.1 + Accept-Encoding workaround for Jikan's 504 issues.
                var reviews = _anime
                    ? await jikan.GetAnimeReviewsAsync(_targetId, true, true)
                    : await jikan.GetMangaReviewsAsync(_targetId, true, true);

                foreach (var review in reviews.Data)
                {
                    var reactions = review.Reactions;
                    output.Add(new AnimeReviewData
                    {
                        AuthorAvatar = review.User?.Images?.JPG?.ImageUrl ?? review.User?.Images?.WebP?.ImageUrl,
                        Author = review.User?.Username,
                        Date = review.Date?.ToString("d") ?? "N/A",
                        EpisodesSeen = (review.EpisodesWatched ?? review.ChaptersRead)?.ToString() ?? "N/A",
                        HelpfulCount = reactions?.Informative.ToString() ?? "N/A",
                        Id = review.MalId.ToString(),
                        OverallRating = review.Score == 0 ? "N/A" : review.Score.ToString(),
                        Review = review.Content,
                        HasSpoilers = review.IsSpoiler,
                        IsPreliminary = false, // not exposed by JikanDotNet's Review model
                        Score = new List<ReviewScore>
                        {
                            new ReviewScore
                            {
                                Field = "Informative",
                                Score = reactions?.Informative.ToString() ?? "N/A"
                            },
                            new ReviewScore
                            {
                                Field = "Confusing",
                                Score = reactions?.Confusing.ToString() ?? "N/A"
                            },
                            new ReviewScore
                            {
                                Field = "Creative",
                                Score = reactions?.Creative.ToString() ?? "N/A"
                            },
                            new ReviewScore
                            {
                                Field = "Funny",
                                Score = reactions?.Funny.ToString() ?? "N/A"
                            },
                            new ReviewScore
                            {
                                Field = "Love It",
                                Score = reactions?.LoveIt.ToString() ?? "N/A"
                            },
                            new ReviewScore
                            {
                                Field = "Well Written",
                                Score = reactions?.WellWritten.ToString() ?? "N/A"
                            },
                        }
                    });
                }

                DataCache.SaveAnimeReviews(_targetId, output, _anime);
            }
            catch (Exception)
            {
            }

            return output;
        }
    }
}
