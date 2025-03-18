using FluentValidation;

namespace Server.Application.Features.AcademicYearsApp.Commands.CreateAcademicYear;

public class CreateAcademicYearCommandValidator : AbstractValidator<CreateAcademicYearCommand>
{
    public CreateAcademicYearCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Academic year name is required.")
            .NotNull()
            .WithMessage("Academic year name is required.")
            .Matches(@"^\d{4}-\d{4}$")
            .WithMessage("Academic year name must be in the format 'XXXX-YYYY'.")
            .Must(IsConsecutive)
            .WithMessage("The academic year must consist of two consecutive years (e.g., 2024-2025).");

        RuleFor(x => x.StartClosureDate)
            .NotEmpty()
            .WithMessage("StartClosureDate is required.")
            .NotNull()
            .WithMessage("StartClosureDate is required.")
            .Must((request, date) => IsValidStartYear(date, request.Name))
            .WithMessage("StartClosureDate must be within the academic year.");

        RuleFor(x => x.EndClosureDate)
            .NotEmpty()
            .WithMessage("EndClosureDate is required.")
            .NotNull()
            .WithMessage("EndClosureDate is required.")
            .Must((request, date) => IsWithinOrAtEndOfAcademicYear(date, request.Name))
            .WithMessage("EndClosureDate must be within the academic year or exactly at its end.")
            .GreaterThan(x => x.StartClosureDate)
            .WithMessage("EndClosureDate have to be after StartClosureDate.");

        RuleFor(x => x.FinalClosureDate)
            .NotEmpty()
            .WithMessage("FinalClosureDate is required.")
            .NotNull()
            .WithMessage("FinalClosureDate is required.")
            .Must((request, date) => IsWithinOrAtEndOfAcademicYear(date, request.Name))
            .WithMessage("FinalClosureDate must be within the academic year or exactly at its end.")
            .GreaterThan(x => x.StartClosureDate)
            .WithMessage("FinalClosureDate have to be after StartClosureDate.")
            .GreaterThan(x => x.EndClosureDate)
            .WithMessage("FinalClosureDate have to be after EndClosureDate.");
    }

    private bool IsConsecutive(string? name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return false;
        }

        var years = name.Split('-');

        if (years.Length != 2)
        {
            return false;
        }

        if (int.TryParse(years[0], out int startYear) && int.TryParse(years[1], out int endYear))
        {
            return endYear - startYear == 1;
        }

        return false;
    }

    private bool IsValidStartYear(DateTime date, string? academicYearName)
    {
        if (string.IsNullOrEmpty(academicYearName))
        {
            return false;
        }

        var years = academicYearName.Split("-");

        if (years.Length != 2)
        {
            return false;
        }

        if (!int.TryParse(years[0], out int startYear) || !int.TryParse(years[1], out int endYear))
        {
            return false;
        }

        var year = date.Year;

        return year >= startYear && year < endYear;
    }

    private bool IsWithinOrAtEndOfAcademicYear(DateTime date, string? academicYearName)
    {
        if (string.IsNullOrEmpty(academicYearName))
        {
            return false;
        }

        var years = academicYearName.Split("-");

        if (years.Length != 2)
        {
            return false;
        }

        if (!int.TryParse(years[0], out int startYear) || !int.TryParse(years[1], out int endYear))
        {
            return false;
        }

        var endOfTheYear = new DateTime(startYear, 12, 31);

        return date.Year == startYear;
    }
}
