using System;

namespace MALClient.XShared.JsonModels.MAL
{
    /// <summary>
    /// Maps raw official MAL API v2 enum-ish string values (snake_case) to the
    /// human readable strings the rest of the app historically received from Jikan,
    /// so UI and logic comparing those strings keep working unchanged.
    /// </summary>
    internal static class MalApiHelpers
    {
        public static string PrettyAnimeMediaType(string mediaType)
        {
            switch (mediaType)
            {
                case "tv": return "TV";
                case "tv_special": return "TV Special";
                case "ova": return "OVA";
                case "ona": return "ONA";
                case "movie": return "Movie";
                case "special": return "Special";
                case "music": return "Music";
                default: return Capitalize(mediaType?.Replace('_', ' '));
            }
        }

        public static string PrettyMangaMediaType(string mediaType)
        {
            switch (mediaType)
            {
                case "manga": return "Manga";
                case "novel": return "Novel";
                case "light_novel": return "Light Novel";
                case "one_shot": return "One-shot";
                case "doujinshi": return "Doujinshi";
                case "manhwa": return "Manhwa";
                case "manhua": return "Manhua";
                case "oel": return "OEL";
                default: return Capitalize(mediaType?.Replace('_', ' '));
            }
        }

        public static string PrettyAnimeStatus(string status)
        {
            switch (status)
            {
                case "finished_airing": return "Finished Airing";
                case "currently_airing": return "Currently Airing";
                case "not_yet_aired": return "Not yet aired";
                default: return Capitalize(status?.Replace('_', ' '));
            }
        }

        public static string PrettyMangaStatus(string status)
        {
            switch (status)
            {
                case "finished": return "Finished";
                case "currently_publishing": return "Publishing";
                case "not_yet_published": return "Not yet published";
                case "on_hiatus": return "On Hiatus";
                case "discontinued": return "Discontinued";
                default: return Capitalize(status?.Replace('_', ' '));
            }
        }

        private static string Capitalize(string s)
        {
            if (string.IsNullOrEmpty(s))
                return "Unknown";
            return char.ToUpper(s[0]) + s.Substring(1);
        }
    }
}
