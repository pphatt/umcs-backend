namespace Server.Application.Wrapper.Report;

public class ReportResponseWrapper<T> where T : class
{
    public List<T> Response { get; set; } = new();
}

public class AcademicYearReportResponseWrapper<T> where T : class
{
    public string AcademicYear { get; set; }

    public List<T> DataSets { get; set; } = new List<T>();
}

