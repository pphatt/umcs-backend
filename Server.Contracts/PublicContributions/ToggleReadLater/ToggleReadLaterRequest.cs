using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Server.Contracts.PublicContributions.ToggleReadLater;

public class ToggleReadLaterRequest
{
    public Guid ContributionId { get; set; }
}
