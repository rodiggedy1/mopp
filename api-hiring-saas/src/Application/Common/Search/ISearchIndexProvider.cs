namespace Application.Common.Search
{
    public interface ISearchIndexProvider
    {
        string GetIndex<T>() where T: ISearchable;
    }
}
