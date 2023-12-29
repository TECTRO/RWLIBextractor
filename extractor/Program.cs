using extractor.data.domcontroller.provider;
using extractor.files;
using extractor.files.excercisessaver;
using extractor.files.serialiser.provider;
using extractor.models;
using extractor.network.hemlfetcher;


Console.WriteLine("""

        ____ _       ____    ________            __                  __            
       / __ \ |     / / /   /  _/ __ )___  _  __/ /__________ ______/ /_____  _____
      / /_/ / | /| / / /    / // __  / _ \| |/_/ __/ ___/ __ `/ ___/ __/ __ \/ ___/
     / _, _/| |/ |/ / /____/ // /_/ /  __/>  </ /_/ /  / /_/ / /__/ /_/ /_/ / /    
    /_/ |_| |__/|__/_____/___/_____/\___/_/|_|\__/_/   \__,_/\___/\__/\____/_/     
    By TECTRO
                                                                                  
    """);

#region declare dependences 
///////////////////////////////////////////////////////////////////////

IDOMControllerProvider controllerProvider = new DOMControllerProvider();

IHTMLFetcher fetcher = new FlareSolverHttpFetcher();

ISaver<ExerciseContainer> excerciseSaver =
    new WordPadExcercisesSaver(
        new Uri($"{Environment.CurrentDirectory}\\excercises.docx")
    );

ISaveLoader<Exercise[]> excerciseSerialiser =
    new SerialiserProvider<Exercise[]>()
        .GetSerialiser(
            new Uri($"{Environment.CurrentDirectory}\\excercises.json")
        );

ISaveLoader<UrisContainer> testContainerSerialiser =
    new SerialiserProvider<UrisContainer>()
        .GetSerialiser(
            new Uri($"{Environment.CurrentDirectory}\\pagesContainer.json")
        );

#endregion///////////////////////////////////////////////////////////////////////

#region pages loading

string? topic = null;
var contentPageUrls = new List<Uri>();

async Task LoadContentPagesFromWeb()
{
    Console.WriteLine("enter required RWlib URl with required topic");

    var url = Console.ReadLine() ?? string.Empty;

    Console.WriteLine();
    Console.WriteLine("network downloading started");

    Uri? nextPageUri = new Uri(url);
    var index = 0;
    do
    {
        var topicPage = await fetcher.GetPageHtml(nextPageUri);
        var pageController = controllerProvider.GetDOMController(topicPage);

        if (topic == null)
        {
            topic = pageController.TryExtractTopic();
        }

        contentPageUrls?.AddRange(pageController.GetContentPageUris());

        nextPageUri = pageController.GetNextPage();

        index++;
        Console.WriteLine($"fetched page {index} ...");
    }
    while (nextPageUri != null);

    Console.WriteLine($"All pages downloaded.");
    Console.WriteLine();

    if (topic != null)
    {
        Console.WriteLine();
        Console.WriteLine(topic);
        Console.WriteLine();
    }

    Console.WriteLine("Do you want to cache test data? y/n");
    if (Console.ReadKey().Key == ConsoleKey.Y)
    {
        testContainerSerialiser.Save(
            new UrisContainer()
            {
                PageUrls = contentPageUrls?.ToArray() ?? Array.Empty<Uri>(),
                Topic = topic ?? string.Empty
            });
    }
    Console.WriteLine();
}
void LoadContentPagesFromCache()
{
    Console.WriteLine("content uris restoration started");
    var container = testContainerSerialiser.Load();
    topic = container?.Topic;
    contentPageUrls?.AddRange(container?.PageUrls ?? Array.Empty<Uri>());
    Console.WriteLine("content uris restoration completed");
    Console.WriteLine();

    if (topic != null)
    {
        Console.WriteLine();
        Console.WriteLine(topic);
        Console.WriteLine();
    }

}

if (testContainerSerialiser.Exists())
{
    Console.WriteLine("Do you want to use cached theme content uris? y/n");
    var keyInfo1 = Console.ReadKey();
    Console.WriteLine();
    if (keyInfo1.Key == ConsoleKey.Y)
        LoadContentPagesFromCache();
    else
        await LoadContentPagesFromWeb();
}
else
{
    await LoadContentPagesFromWeb();
}


#endregion///////////////////////////////////////////////////////////////////////

#region excercises loading

var excercises = new List<Exercise>();

async Task LoadExcercisesFromWeb()
{
    
    Console.WriteLine("network download started");

    for (int i = 0; i < contentPageUrls.Count; i++)
    {
        Uri? contentUrl = contentPageUrls[i];
        var contentPage = await fetcher.GetPageHtml(contentUrl);
        var pageController = controllerProvider.GetDOMController(contentPage);
        var excercise = pageController.TryExtractExercise();
        if (excercise != null)
        {
            excercises.Add(excercise);
        }
        Console.WriteLine($"fetched excercise {i + 1} of {contentPageUrls.Count} ...");
    }

    Console.WriteLine($"All excercises downloaded.");
    Console.WriteLine();

    Console.WriteLine("Do you want to cache excercises? y/n");
    if (Console.ReadKey().Key == ConsoleKey.Y)
    {
        excerciseSerialiser.Save(excercises?.ToArray() ?? Array.Empty<Exercise>());
    }
    Console.WriteLine();
}
void LoadExcercisesFromCache()
{
    Console.WriteLine("excercises restoration started");
    excercises.AddRange(excerciseSerialiser.Load() ?? Array.Empty<Exercise>());
    Console.WriteLine("excercises restoration completed");
    Console.WriteLine();

}

if (excerciseSerialiser.Exists())
{
    Console.WriteLine("Do you want to use cached exercises? y/n");
    var keyInfo1 = Console.ReadKey().Key;
    Console.WriteLine();

    if (keyInfo1 == ConsoleKey.Y)
    {
        LoadExcercisesFromCache();
    }
    else
    {
        await LoadExcercisesFromWeb();
    }
}
else
{
    await LoadExcercisesFromWeb();
}


#endregion///////////////////////////////////////////////////////////////////////

#region docx exporting

Console.WriteLine("Do you want to generate and export docx file? y/n");
var keyInfo = Console.ReadKey();
Console.WriteLine();
if (keyInfo.Key == ConsoleKey.Y)
{
    Console.WriteLine("Export started...");
    excerciseSaver.Save(
        new ExerciseContainer()
        {
            Exercises = excercises.ToArray(),
            Theme = topic ?? string.Empty
        }
    );
    Console.WriteLine("Export finished");
}

Console.WriteLine();
Console.WriteLine("Program finished...");
Console.ReadKey();

#endregion
