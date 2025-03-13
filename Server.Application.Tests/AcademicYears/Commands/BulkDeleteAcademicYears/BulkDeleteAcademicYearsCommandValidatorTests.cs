using Server.Application.Features.AcademicYearsApp.Commands.BulkDeleteAcademicYears;

namespace Server.Application.Tests.AcademicYears.Commands.BulkDeleteAcademicYears;

[Trait("Academic Year", "Bulk Delete")]
public class BulkDeleteAcademicYearsCommandValidatorTests : BaseTest
{
    private readonly BulkDeleteAcademicYearsCommandValidator _validator;

    public BulkDeleteAcademicYearsCommandValidatorTests()
    {
        _validator = new BulkDeleteAcademicYearsCommandValidator();
    }
}
