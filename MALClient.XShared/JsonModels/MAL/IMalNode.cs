namespace MALClient.XShared.JsonModels.MAL
{
    /// <summary>
    /// Minimal shape shared by official API anime/manga nodes (details entries).
    /// </summary>
    internal interface IMalNode
    {
        long? MalId { get; }
        string Title { get; }
    }
}
