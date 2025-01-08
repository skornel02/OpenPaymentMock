using System.ComponentModel.DataAnnotations;

namespace OpenPaymentMock.Server.Options;

public sealed class ApplicationOptions
{
    public const string SectionName = "Application";

    [Required(ErrorMessage = "ApplicationUrl must be provided!")]
    public string ApplicationUrl { get; set; } = string.Empty;

    public int PaymentAttemptTimeout { get; set; } = 300;
}
