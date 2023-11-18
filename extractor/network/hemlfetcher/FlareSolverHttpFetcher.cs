using FlareSolverrSharp;

namespace extractor.network.hemlfetcher
{
    internal class FlareSolverHttpFetcher : IHTMLFetcher
    {
        ClearanceHandler handler;
        public FlareSolverHttpFetcher() {
            handler = new ClearanceHandler("http://localhost:8191/")
            {
                MaxTimeout = 60000
            };
        }
        public void Dispose()
        {
            handler.Dispose();
        }

        public async Task<string> GetPageHtml(Uri url)
        {
            var client = new HttpClient(handler);
            var content = await client.GetStringAsync(url);
            return content;
        }
    }
}
