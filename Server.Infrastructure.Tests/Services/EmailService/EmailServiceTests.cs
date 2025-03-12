using Microsoft.Extensions.Options;

using Moq;

using Server.Infrastructure.Services.Email;

namespace Server.Infrastructure.Tests.Services.Email;

public class EmailServiceTests
{
    private readonly Mock<IOptions<EmailSettings>> _emailSettingsMock;
    private readonly EmailService _emailService;

    public EmailServiceTests()
    {
        _emailSettingsMock = new Mock<IOptions<EmailSettings>>();
        var emailSettings = new EmailSettings
        {
            Email = "test@example.com",
            Password = "password123",
            Host = "smtp.example.com",
            Port = 587,
            DisplayName = "Test Sender"
        };

        _emailSettingsMock.Setup(x => x.Value).Returns(emailSettings);

        _emailService = new EmailService(_emailSettingsMock.Object);
    }

    [Fact]
    public async Task SendEmailAsync_ValidMailRequest_ShouldSendEmailWithCorrectConfiguration()
    {
        // I still don't know how to unit test it.
        // I think email just can be Integration Test.
    }

    [Fact]
    public async Task SendEmailAsync_NullMailRequest_ShouldThrowArgumentNullException()
    {
        // I still don't know how to unit test it.
        // I think email just can be Integration Test.
    }
}
