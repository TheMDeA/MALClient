# MAL API v2 migration & loading-issue fix

Based on upstream PR [#359](https://github.com/Drutol/MALClient/pull/359)
("Fix most entries that are currently not loading"), extended with further
migrations from Jikan to the official MyAnimeList API v2.

## Why

The app fetched anime/manga details, seasonal lists and search results through
Jikan (api.jikan.moe). Jikan has ongoing stability problems (HTTP/2 not
supported, intermittent 504s without a specific `Accept-Encoding`), which left
most entries not loading (issues #359, #360, #361). The official MAL API v2
(`api.myanimelist.net/v2`) is now used wherever it offers an equivalent.

## What changed

- `Comm/Anime/AnimeGeneralDetailsQuery.cs` — anime details now come from
  `GET /v2/anime/{id}` (official API) instead of Jikan. Fixed the
  `Type == "Movie"` end-date workaround to keep working by mapping
  `media_type` to Jikan-style display strings (`MalApiHelpers`) instead of
  upper-casing; added null-safety on optional fields.
- `Comm/Anime/AnimeGeneralDetailsQuery.cs` (manga branch) — manga details now
  come from `GET /v2/manga/{id}` instead of Jikan.
- `Comm/Anime/AnimeSeasonalQuery.cs` — seasonal anime now comes from
  `GET /v2/anime/season/{year}/{season}?limit=500&nsfw=true` instead of Jikan;
  entries are still ordered so exact-season matches come first, then by member
  count. Fixed `&` → `&&` and null-safety on `start_season`.
- `Comm/Anime/AnimeSearchQuery.cs` — anime search now uses
  `GET /v2/anime?q=...&limit=50&nsfw=true` instead of Jikan.
- `Comm/Manga/MangaSearchQuery.cs` — manga search now uses
  `GET /v2/manga?q=...&limit=50&nsfw=true` instead of Jikan.
- `JsonModels/MAL/` — new DTOs for official API responses (`AnimeEntry`,
  `MangaEntry`, `AnimeNode<T>`, `SeasonEntry`, `Season`, `Genre`, `Picture`,
  `AlternativeTitles`, `PaginatedMALResponse`) plus `MalApiHelpers` which maps
  raw `media_type`/`status` values to the display strings the app used before.
- `JikanDotNet` bumped `2.6.3` → `2.10.4-gatewayerrorfix` (temp fork, see below).

## What changed (round 2 — remaining Jikan / scraped endpoints)

- `Comm/Anime/AnimeRelatedQuery.cs` — related entries now come from the
  official `related_anime`/`related_manga` details fields instead of scraping
  `myanimelist.net` HTML. Relation labels use `relation_type_formatted`
  ("Sequel", "Prequel", …), so sequels/prequels now reliably appear
  (also fixes issue #355).
- `Comm/Anime/AnimeTopQuery.cs` — top anime/manga now come from the official
  `/v2/anime/ranking` and `/v2/manga/ranking` endpoints
  (`all/airing/upcoming/tv/movie/ova/bypopularity/favorite`, manga `manga`)
  instead of scraping `topanime.php`/`topmanga.php`. Rank comes straight from
  the API; 50-item offset pagination preserved.
- `Comm/Anime/AnimeReviewsQuery.cs` — reviews still come from Jikan (the
  official API has no reviews endpoint), but now go through the fixed
  `JikanDotNet` client instead of a raw `HttpClient`, so they benefit from the
  HTTP/1.1 + `Accept-Encoding` workaround. As a side effect manga reviews now
  show chapters read (was "N/A"). Note: the "Preliminary Review" suffix is
  gone — `is_preliminary` is not exposed by JikanDotNet's `Review` model.
- `JsonModels/MAL/` — new DTOs: `RelatedEntry<T>`, `RankingEntry<T>` (+
  `RankingInfo`), `IMalNode` (shared `MalId`/`Title` shape); `AnimeEntry` and
  `MangaEntry` gained `RelatedAnime`/`RelatedManga` collections.

## Still on Jikan (no official-API equivalent exists)

Episodes, genre/studio browse, profile favorites and friends. These use the
fixed JikanDotNet build below.

## Still website scraping (official API has no equivalent, or migration would regress features)

- User-written recommendation texts (`AnimeDirectRecommendationsQuery`) — the
  official `recommendations` field has no description text, so scraping stays.
- Personalized suggestions, recent recommendations page, promotional videos,
  characters & staff — no official endpoints.

## Building: the temporary JikanDotNet fork

Upstream fix [Ervie/jikan.net#69](https://github.com/Ervie/jikan.net/pull/69)
(forces HTTP/1.1, sets `Accept-Encoding`, handles gzip/deflate) is merged but
not yet released on NuGet, so the projects temporarily reference
`2.10.4-gatewayerrorfix` from
`https://nuget.pkg.github.com/stefan9999991/index.json`:

1. Copy `nuget.config.template` → `nuget.config` (git-ignored).
2. Fill in your GitHub username and a classic PAT with `read:packages` scope.
3. Once an official JikanDotNet release contains the fix, revert both
   `.csproj` entries to the official version and delete `nuget.config`.

All new/changed official-API calls were verified live against
`api.myanimelist.net/v2` (details, season, anime search, manga search).
