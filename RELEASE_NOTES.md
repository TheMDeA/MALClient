# MALClient v1.5.15.1

APK version: **1.5.15.1** (versionCode 172), package `com.dmda.malclient`.

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
  wildcarded); fixed `ResolveThemeColor` to resolve `reference`-type
  theme attributes to real colors instead of reading the raw resource id
  (via `TypedValue.ResourceId`, since this Xamarin binding lacks the
  `TypeFirstColorInt`/`TypeReference` constants); qualified framework
  colors as `global::Android.Resource.Color.SystemAccent*` — inside the
  `MALClient.Android.*` namespaces the unqualified name resolved to the
  app's own `Resource` class (CS0117).
