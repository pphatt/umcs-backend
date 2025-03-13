using Server.Application.Features.AcademicYearsApp.Commands.ActivateAcademicYear;

namespace Server.Application.Tests.AcademicYears.Commands.ActivateAcademicYear;

[Trait("Academic Year", "Activate")]
public class ActivateAcademicYearCommandValidatorTests : BaseTest
{
    private readonly ActivateAcademicYearCommandValidator _validator;

    public ActivateAcademicYearCommandValidatorTests()
    {
        _validator = new ActivateAcademicYearCommandValidator();
    }
}
