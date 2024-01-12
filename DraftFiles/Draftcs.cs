The Downloads folder is a so called "known" folder, together with Documents, Videos, and others.

Do NOT:
combine hardcoded path segments to retrieve known folder paths
assume known folders are children of the user folder
abuse a long deprecated registry key storing outdated paths
Known folders can be redirected anywhere in their property sheets. I've gone into more detail on this several years ago in my CodeProject article.

Do:
use the WinAPI method SHGetKnownFolderPath as it is the intended and only correct method to retrieve those paths.
You can p/invoke it as follows (I've provided only a few GUIDs which cover the new user folders):

public enum KnownFolder
{
    Contacts,
    Downloads,
    Favorites,
    Links,
    SavedGames,
    SavedSearches
}

public static class KnownFolders
{
    private static readonly Dictionary<KnownFolder, Guid> _guids = new()
    {
        [KnownFolder.Contacts] = new("56784854-C6CB-462B-8169-88E350ACB882"),
        [KnownFolder.Downloads] = new("374DE290-123F-4565-9164-39C4925E467B"),
        [KnownFolder.Favorites] = new("1777F761-68AD-4D8A-87BD-30B759FA33DD"),
        [KnownFolder.Links] = new("BFB9D5E0-C6A9-404C-B2B2-AE6DB6AF4968"),
        [KnownFolder.SavedGames] = new("4C5C32FF-BB9D-43B0-B5B4-2D72E54EAAA4"),
        [KnownFolder.SavedSearches] = new("7D1D3A04-DEBB-4115-95CF-2F29DA2920DA")
    };

    public static string GetPath(KnownFolder knownFolder)
    {
        return SHGetKnownFolderPath(_guids[knownFolder], 0);
    }

    [DllImport("shell32",
        CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = false)]
    private static extern string SHGetKnownFolderPath(
        [MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags,
        nint hToken = 0);
}
Here's an example of retrieving the path of the Downloads folder:

string downloadsPath = KnownFolders.GetPath(KnownFolder.Downloads);
Console.WriteLine($"Downloads folder path: {downloadsPath}");
NuGet Package

If you don't want to p/invoke yourself, have a look at my NuGet package (note that the usage is different, please check its README).