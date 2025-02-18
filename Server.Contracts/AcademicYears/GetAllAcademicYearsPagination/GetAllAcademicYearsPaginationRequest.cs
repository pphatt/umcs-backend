namespace Server.Contracts.AcademicYears.GetAllAcademicYearsPagination;

public class GetAllAcademicYearsPaginationRequest
{
    public string? Keyword { get; set; }

    public int PageIndex { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}
