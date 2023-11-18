using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;

namespace extractor.data.domcontroller
{
    internal class AngleSharpDOMController : IDOMController
    {
        private IDocument document;

        public AngleSharpDOMController(string html)
        {
            IConfiguration config = Configuration.Default;
            IBrowsingContext context = BrowsingContext.New(config);

            var parser = context.GetService<IHtmlParser>()!;
            document = parser.ParseDocument(html);
        }

        public Uri[] GetContentPageUris()
        {
            var pages = document
                .QuerySelector(".tag_ul")
                ?.ChildNodes
                .QuerySelectorAll("a[href]")
                .Select(t => t.GetAttribute("href"))
                .Where(t => t is not null)
                .Select(t => new Uri(t!))
                .ToArray();

            return pages ?? Array.Empty<Uri>();
        }


        public Uri? GetNextPage()
        {
            var nextPage = document
               .QuerySelector(".pagination")
               ?.ChildNodes
               .QuerySelector("li.active")
               ?.NextElementSibling
               ?.QuerySelector("a[href]")
               ?.GetAttribute("href");

            return nextPage != null ? new Uri(nextPage) : null;
        }

        public Exercise? TryExtractExercise()
        {
            var question = document.QuerySelector("h1[itemprop='name']")?.Text();

            var answers = document
                .QuerySelector("div.a")
                ?.QuerySelectorAll("div.q-a[itemprop]")
                .Select(t =>
                   new ExerciseAnswer() { Text = t.Text(), IsCorrect = t.GetAttribute("itemprop") == "acceptedAnswer" }
                )
                .ToArray();


            if (question == null || answers == null || answers.Count() == 0)
            {
                return null;
            }

            return new Exercise { Question = question, Answers = answers };
        }

        public string? TryExtractTheme()
        {
            var theme = document.QuerySelector("div.content.tag h1")?.Text();
            return theme;
        }
    }
}
