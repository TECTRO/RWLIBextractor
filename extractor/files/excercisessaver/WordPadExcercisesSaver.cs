using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using extractor.models;

namespace extractor.files.excercisessaver
{
    internal class WordPadExcercisesSaver : ISaver<ExerciseContainer>
    {
        private Uri path;

        public WordPadExcercisesSaver(Uri path) { this.path = path; }

        class StringInserter
        {
            Paragraph Paragraph;

            public StringInserter(Paragraph Paragraph) { this.Paragraph = Paragraph; }

            public void Insert(string line, int fontSize = 14, bool isBald = false, bool preserveSpaces = false)
            {
                var props = new RunProperties(
                            new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman", },
                            new FontSize() { Val = $"{fontSize * 2}" }
                        );

                if (isBald)
                {
                    props.AppendChild(new Bold());
                }

                Paragraph.AppendChild(new Run(
                        props,
                        new Text(line)
                        {
                            Space = preserveSpaces ? SpaceProcessingModeValues.Preserve : SpaceProcessingModeValues.Default,
                        }
                    ));
            }
        }

        delegate void insertParts(StringInserter inserter);

        void InsertLine(Body body, insertParts insert)
        {
            var paragraph = new Paragraph();
            insert(new StringInserter(paragraph));
            body.AppendChild(paragraph);
        }

        void InsertLine(Body body, string line, int fontSize = 14, bool isBald = false)
        {
            InsertLine(body, (inserter) => { inserter.Insert(line, fontSize, isBald); });
        }

        public void Save(ExerciseContainer container)
        {
            var exercises = container.Exercises;
            var theme = container.Theme;

            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(path.AbsolutePath, WordprocessingDocumentType.Document))
            {

                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();

                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());

                InsertLine(body, theme, fontSize: 18, isBald: true);
                InsertLine(body, "\n");

                for (int i = 0; i < exercises.Length; i++)
                {
                    Task? excercise = exercises[i];
                    InsertLine(body, (inserter) => {
                        inserter.Insert($"Вопрос {i+1}: ", fontSize: 16, isBald: true, preserveSpaces: true);
                        inserter.Insert(excercise.Question, fontSize: 16);
                    });
                    
                    for (int i1 = 0; i1 < excercise.Answers.Length; i1++)
                    {
                        var answer = excercise.Answers[i1];
                        InsertLine(body, (inserter) => {
                            inserter.Insert($"{i1 + 1}) ", fontSize: 16, isBald: true, preserveSpaces: true);
                            if (answer.IsCorrect)
                            {
                                inserter.Insert("Правильный ответ: ", isBald: true, preserveSpaces: true);
                            }
                            inserter.Insert(answer.Text);
                        });
                    }
                    InsertLine(body,"\n");
                }

            }
        }

        public bool Exists()
        {
            return File.Exists(path.AbsolutePath);
        }
    }
}
