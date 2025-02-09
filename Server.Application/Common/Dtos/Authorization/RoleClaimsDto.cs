﻿namespace Server.Application.Common.Dtos.Authorization;

public class RoleClaimsDto
{
    public string? Type { get; set; }

    public string? Value { get; set; }

    public string? DisplayName { get; set; }

    public bool? Selected { get; set; } = false;
}
