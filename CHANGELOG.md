# Changelog — `mal-api-v2-fix`

Branch based on `Drutol/MALClient` @ `37fb71f`. Every fix lands here first under
`[Unreleased]`, then moves into a dated section.

## [Unreleased]

_(nothing else pending)_

## [2026-10-03] — feature push (calendar seasonal view, follow-system theme, Material You)

APK version: **1.5.15.0** (versionCode 171), package `com.dmda.malclient`.

### Changed

- Calendar page: new "Show current season airing anime" toggle in
  Settings → Calendar (on by default). When on, the calendar shows anime
  currently airing in the current season instead of just the watchlist;
  entries already in the user's library reuse their data so status/progress
  keep showing, others render like seasonal-page cards. Still limited to
  40 items, user's own list entries first. Toggling rebuilds the calendar
  in the background. When off, the previous watchlist-only view is used.
  The old "Include watching / plan to watch" toggles were removed from the
  Android settings page (their stored values are kept for other platforms).
- Seasonal anime page: the season dropdown now lists the most recent season
  first (current season, then walking back 3 years) instead of Winter-first
  per year; the current season is marked "(Current)".

### Added

- Theme: new "Follow system" option in Settings → General. When selected,
  the app follows the Android system dark/light theme (re-applied whenever
  the activity restarts, e.g. on system theme change); all theme checks now
  go through the resolved `Settings.IsDarkTheme`/`EffectiveTheme`.
- Accent: new "Material You" dynamic-color accent option (Android 12+; hidden
  on older versions). When selected, the app accent follows the wallpaper-
  derived system palette via `values-v31` theme overlays
  (`system_accent1/2_*`); the settings button previews the live system
  accent. Falls back to Orange on older Android.

### Fixed

- Calendar airing data going stale: `AiringInfoProvider` previously loaded the
  airing feed only once per app process, so if the app stayed alive in the
  background for days the episode timestamps all ended up in the past and the
  calendar showed wrong info (e.g. "Aired today!" on wrong days). The provider
  now re-downloads when no show in the feed has an upcoming episode left, and
  `CalendarPageViewModel` refreshes the feed every time the calendar is opened
  (no-op while the data is still current).

- Build fix: removed four dead `using AoLibs.Adapters.Core[_Interfaces];`
  directives (`AnimeDetailsPageFragment`, `AnimeDetailsPageStaffTabFragment`,
  `PersonDetailsPageProdTabFragment`, `PersonDetailsPageVaTabFragment`). The
  vendored `AoLibsCompat.cs` only provides `AoLibs.Adapters.Android.Recycler`,
  which is the only AoLibs namespace this code actually uses — the `Core`
  usings referenced a namespace that no longer exists and broke the Release
  build with CS0234.

- Material You: added the missing `Resources\values-v31\styles.xml`
  `AndroidResource` entry to the Android csproj (explicit includes, not
  wildcarded), and fixed `ResolveThemeColor` to resolve `reference`-type
  theme attributes to real colors instead of reading the raw resource id.

_(nothing else pending)_

## [2026-10-03] — 2026-10-03

APK version: **1.5.14.0** (versionCode 170), package `com.dmda.malclient`.

### Added

- `MalApiClient.GetJsonAsync`: API responses now deserialize straight from the
  response stream instead of buffering the whole payload into a string first.
- `MangaTopType` redefined locally (was supplied by the old JikanDotNet); each
  top-manga category is served by `/v2/manga/ranking` with the matching
  `ranking_type`.
- `DataCache.SaveTopMangaData` / `RetrieveTopMangaData` (per-category cache files).
- AoLibs adapter sources vendored locally (`AoLibsCompat.cs`) — the NuGet
  packages are gone from nuget.org.
- `build-apk.yml` GitHub Actions workflow for APK builds.
- This changelog.

### Changed

- Android package renamed to `com.dmda.malclient`.
- Seasonal list: fixed O(n²) `FindIndex`-in-a-loop → indexed loop,
  `OrderByDescending` instead of `OrderBy(...).Reverse()`, hoisted a
  per-comparison `ToLower()` out of the sort key.
- Top lists: dropped redundant `Distinct()` after `Union()`; single-lookup
  `TryGetValue` for the in-memory cache.

### Fixed

- **Empty data in release builds** — the new JSON models were stripped by the
  Android linker (`Full` mode), leaving every page blank. All MAL API model
  classes now carry `[Preserve(AllMembers = true)]`.
- **Blank season dropdown** — the season list came from Jikan's `/seasons`
  endpoint and failures were silently swallowed. The last 3 years are now
  generated locally (same order and names as before).
- **Build errors (code)** — `await using` → `using` (`Stream` has no
  `IAsyncDisposable` on netstandard2.0, CS8417); added the missing
  `DataCache.SaveTopMangaData` / `RetrieveTopMangaData` methods (CS0117).
- **Build errors (CI)** — `dotnet pack -f` → `-p:TargetFrameworks=netstandard2.0`
  (`dotnet pack` doesn't accept `-f`, MSB1001).
- Restored `JikanClient` wrapper and `using JikanDotNet;` dropped by the
  re-forked base so the branch compiles.

### Removed

- Deprecated `PackageTargetFallback` (broke restore on modern NuGet).
- AoLibs NuGet package references (replaced by vendored sources).
- Unused usings left over from the Jikan migration.

## [2026-10-02] — 2026-10-02

### Added

- Official MyAnimeList API v2 migration (`api.myanimelist.net/v2`):
  - Anime/manga details (`GET /v2/anime/{id}`, `GET /v2/manga/{id}`)
  - Seasonal anime (`GET /v2/anime/season/{year}/{season}`)
  - Anime/manga search (`GET /v2/anime?q=…`, `GET /v2/manga?q=…`)
  - Related entries via official `related_anime` / `related_manga` fields
  - Top anime/manga via `GET /v2/anime/ranking`, `GET /v2/manga/ranking`
  - Reviews via the fixed JikanDotNet client (no official reviews endpoint)
- New `JsonModels/MAL/` DTOs for official API responses, plus `MalApiHelpers`
  mapping raw `media_type` / `status` values to the display strings the app used
  before (keeps UI and the `Type == "Movie"` logic working unchanged).
- `JikanDotNet` bumped `2.6.3` → `2.10.4-gatewayerrorfix` (temporary fork build;
  upstream fix merged but not yet released on NuGet).

### Fixed

- **Entries not loading (#359)** — fixed by the API v2 migration.
- **Missing sequels/prequels in related (#355)** — relation labels now come from
  `relation_type_formatted`, so sequels/prequels reliably appear.

## Still on Jikan (no official-API equivalent)

Episodes, genre/studio browse, profile favorites and friends — via the fixed
JikanDotNet build.

## Still website scraping (no official endpoint, or migration would regress features)

Recommendation texts, personalized suggestions, recent recommendations page,
promotional videos, characters & staff.
