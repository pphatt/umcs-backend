using Server.Application.Features.AcademicYearsApp.Commands.DeleteAcademicYear;

namespace Server.Application.Tests.AcademicYears.Commands.DeleteAcademicYear;

[Trait("Academic Year", "Delete")]
public class DeleteAcademicYearCommandValidatorTests : BaseTest
{
    private readonly DeleteAcademicYearCommandValidator _validator;

    public DeleteAcademicYearCommandValidatorTests()
    {
        _validator = new DeleteAcademicYearCommandValidator();
    }
}
