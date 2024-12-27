using System.ComponentModel.DataAnnotations;

namespace OpenPaymentMock.Server.Options;

public sealed class AdminOptions
{
    public const string SectionName = "Admin";

    [Required(ErrorMessage = "Secret ApiKey must be provided!")]
    public string ApiKey { get; set; } = string.Empty;
}
