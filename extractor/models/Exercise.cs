class Exercise
{
    public required string Question { get; set; }
    public required ExerciseAnswer[] Answers { get; set; }
}

class ExerciseAnswer
{
    public required string Text { get; set; }
    public required bool IsCorrect { get; set; }

}