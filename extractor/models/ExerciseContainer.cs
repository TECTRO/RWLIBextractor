namespace extractor.models
{
    internal class ExerciseContainer
    {
        public required string Topic { get; set; }
        public required Exercise[] Exercises { get; set; }
    }
}
