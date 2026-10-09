namespace SecurityApplicationPresentation.DataTransferObjects
{
    public class RedirectInfo
    {
        public RedirectInfo() { }
        public RedirectInfo(string url) { 
            Url = url;
        }
        public string Url { get; set; } = string.Empty;
    }
}
