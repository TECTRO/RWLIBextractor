namespace extractor.data.domcontroller
{
    interface IDOMController
    {
        Uri[] GetContentPageUris();

        Uri? GetNextPage();

        Exercise? TryExtractExercise();

        string? TryExtractTheme();
    }
}
