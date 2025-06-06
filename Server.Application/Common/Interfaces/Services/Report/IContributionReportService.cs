﻿using Server.Application.Common.Dtos.Content.Report.Contributions;
using Server.Application.Wrapper.Report;

namespace Server.Application.Common.Interfaces.Services.Report;

public interface IContributionReportService
{
    Task<ReportResponseWrapper<AcademicYearReportResponseWrapper<TotalContributionsInEachFacultyInEachAcademicYearReportDto>>> GetTotalContributionsInEachFacultyInEachAcademicYearReport();
    Task<ReportResponseWrapper<AcademicYearReportResponseWrapper<GetTotalContributionsInEachFacultyForAnyAcademicYearReportDto>>> GetTotalContributionsInEachFacultyForAnyAcademicYearReport(string academicYearName);
    Task<ReportResponseWrapper<AcademicYearReportResponseWrapper<GetPercentageOfTotalContributionsInEachFacultyInEachAcademicYearReportDto>>> GetPercentageOfTotalContributionsInEachFacultyInEachAcademicYearReport();
    Task<ReportResponseWrapper<AcademicYearReportResponseWrapper<PercentageTotalContributionsPerFacultyPerAcademicYearReportDto>>> GetPercentageOfTotalContributionsByEachFacultyForAnyAcademicYearReport(string academicYearName);
    Task<ReportResponseWrapper<AcademicYearReportResponseWrapper<GetTotalContributorsInEachFacultyInEachAcademicYearReportDto>>> GetTotalContributorsInEachFacultyInEachAcademicYearReport();
    Task<ReportResponseWrapper<AcademicYearReportResponseWrapper<GetTotalContributorsByEachFacultyForAnyAcademicYearReportDto>>> GetTotalContributorsByEachFacultyForAnyAcademicYearReport(string academicYearName);
    Task<ReportResponseWrapper<AcademicYearReportResponseWrapper<GetPercentageOfTotalContributorsInEachFacultyInEachAcademicYearReportDto>>> GetPercentageOfTotalContributorsInEachFacultyInEachAcademicYearReport();
    Task<ReportResponseWrapper<AcademicYearReportResponseWrapper<GetPercentageOfTotalContributorsByEachFacultyForAnyAcademicYearReportDto>>> GetPercentageOfTotalContributorsByEachFacultyForAnyAcademicYearReport(string academicYearName);
    Task<ReportResponseWrapper<AcademicYearReportResponseWrapper<GetTotalAcceptRejectContributionsInEachFacultyForAnyAcademicYearReportDto>>> GetTotalAcceptRejectContributionsInEachFacultyForAnyAcademicYearReport(string academicYearName);
}
