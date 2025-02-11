namespace Server.Application.Wrapper.Pagination;

public class PaginationResult<T> : PaginationResultBase where T : class
{
    public List<T> Results { get; set; }

    public PaginationResult()
    {
        Results = new();
    }
}
