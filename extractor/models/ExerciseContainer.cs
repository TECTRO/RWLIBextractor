namespace extractor.models
{
    internal class ExerciseContainer
    {
        public required string Theme { get; set; }
        public required Exercise[] Exercises { get; set; }
    }
}
