
using System.Text.Json;

namespace extractor.files.serialiser
{
    interface ISerialiser<T> : ISaveLoader<T>;

    internal class JsonSerialiser<T> : ISerialiser<T>
    {
        private Uri path;

        public JsonSerialiser(Uri path) { this.path = path; }

        public bool Exists()
        {
            return File.Exists(path.AbsolutePath);
        }

        public T? Load()
        {
            using (var reader = new StreamReader(new FileStream(path.AbsolutePath, FileMode.OpenOrCreate)))
            {
                return JsonSerializer.Deserialize<T>(reader.ReadToEnd());
            }
        }

        public void Save(T data)
        {
            using (var writer = new StreamWriter(new FileStream(path.AbsolutePath, FileMode.OpenOrCreate)))
            {
                writer.Write(JsonSerializer.Serialize(data));
            }
        }
    }
}
