
namespace extractor.data.domcontroller.provider
{
    internal class DOMControllerProvider : IDOMControllerProvider
    {
        public IDOMController GetDOMController(string html)
        {
            return new AngleSharpDOMController(html);
        }
    }
}
