namespace GK.Models
{
    public record WebBrowser(string Name, int MajorVersion)
    {
        public static class BrowserName
        {
            public const string InternetExplorer = "Internet Explorer";
            public const string GoogleChrome = "Google Chrome";
        }
    }
}
