namespace extractor.files
{
    internal interface IChecker
    {
        bool Exists();
    }
    internal interface ISaver<T>: IChecker
    {
        void Save(T data);
    }

    internal interface ILoader<T>: IChecker
    {
        T? Load();
    }

    internal interface ISaveLoader<T> : ISaver<T>, ILoader<T>;
}