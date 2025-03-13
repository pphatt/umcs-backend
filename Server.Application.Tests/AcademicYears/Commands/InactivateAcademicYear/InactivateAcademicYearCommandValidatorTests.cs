using Server.Application.Features.AcademicYearsApp.Commands.InactivateAcademicYear;

namespace Server.Application.Tests.AcademicYears.Commands.InactivateAcademicYear;

[Trait("Academic Year", "Inactivate")]
public class InactivateAcademicYearCommandValidatorTests : BaseTest
{
    private readonly InactivateAcademicYearCommandValidator _validator;

    public InactivateAcademicYearCommandValidatorTests()
    {
        _validator = new InactivateAcademicYearCommandValidator();
    }
}
